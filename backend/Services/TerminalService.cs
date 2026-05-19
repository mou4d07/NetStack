using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetMapManager.API.Data;
using NetMapManager.API.Models;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace NetMapManager.API.Services
{
    public class TerminalService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TerminalService> _logger;

        private class SimulatorState
        {
            public string CurrentMode { get; set; } = "EXEC"; // EXEC, CONFIG, CONFIG_IF
            public string Hostname { get; set; } = string.Empty;
            public string CurrentInterface { get; set; } = string.Empty;
        }

        public TerminalService(IServiceScopeFactory scopeFactory, ILogger<TerminalService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task HandleTerminalSession(HttpContext context, WebSocket webSocket)
        {
            var query = context.Request.Query;
            if (!query.TryGetValue("switchId", out var switchIdStr) || !int.TryParse(switchIdStr, out int switchId))
            {
                await SendTextAsync(webSocket, "\r\nError: Missing or invalid switchId parameter.\r\n");
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid switchId", CancellationToken.None);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NetMapDbContext>();
            var sw = await db.NetworkSwitches.Include(s => s.SwitchModel).FirstOrDefaultAsync(s => s.Id == switchId);

            if (sw == null)
            {
                await SendTextAsync(webSocket, $"\r\nError: Switch with ID {switchId} not found in inventory.\r\n");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Switch not found", CancellationToken.None);
                return;
            }

            // Print Banner
            await SendTextAsync(webSocket, "\x1b[1;36m"); // Cyan Bold
            await SendTextAsync(webSocket, "============================================================\r\n");
            await SendTextAsync(webSocket, "               Alsolb NetStack Switch Console               \r\n");
            await SendTextAsync(webSocket, "============================================================\r\n");
            await SendTextAsync(webSocket, "\x1b[0m"); // Reset
            await SendTextAsync(webSocket, $"Device: {sw.SwitchModel?.Brand ?? "Generic"} {sw.SwitchModel?.Name ?? "Switch"} ({sw.IPAddress})\r\n");
            await SendTextAsync(webSocket, $"Location: {sw.Location ?? "N/A"} | Serial: {sw.SerialNumber ?? "N/A"}\r\n");
            await SendTextAsync(webSocket, $"Status: {sw.Status} | Role: {sw.Role} Tier\r\n\r\n");

            if (string.IsNullOrEmpty(sw.IPAddress))
            {
                await SendTextAsync(webSocket, "\r\n\x1b[1;31mError:\x1b[0m No IP Address assigned to this switch.\r\n");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "No IP Address", CancellationToken.None);
                return;
            }

            await SendTextAsync(webSocket, "Username: ");
            string username = await ReadLineAsync(webSocket, false);
            if (webSocket.State != WebSocketState.Open || string.IsNullOrEmpty(username)) return;

            await SendTextAsync(webSocket, "Password: ");
            string password = await ReadLineAsync(webSocket, true);
            if (webSocket.State != WebSocketState.Open) return;

            await SendTextAsync(webSocket, $"\r\nAttempting SSH connection to {sw.IPAddress} (port 22)...\r\n");

            try
            {
                await RunSshSession(webSocket, sw.IPAddress, username, password);
            }
            catch (Renci.SshNet.Common.SshAuthenticationException authEx)
            {
                await SendTextAsync(webSocket, $"\r\n\x1b[1;31mAuthentication Failed:\x1b[0m {authEx.Message}\r\n");
            }
            catch (System.Net.Sockets.SocketException sockEx)
            {
                await SendTextAsync(webSocket, $"\r\n\x1b[1;31mNetwork Error:\x1b[0m Failed to reach {sw.IPAddress}:22.\r\nReason: {sockEx.Message} (Error Code: {sockEx.SocketErrorCode})\r\n");
            }
            catch (Exception ex)
            {
                await SendTextAsync(webSocket, $"\r\n\x1b[1;31mSSH Connection Error:\x1b[0m {ex.Message}\r\n");
            }

            await SendTextAsync(webSocket, "\r\nSession closed.\r\n");
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session closed", CancellationToken.None);
            }
        }

        private async Task RunSshSession(WebSocket webSocket, string host, string username, string password)
        {
            var connectionInfo = new Renci.SshNet.ConnectionInfo(host, 22, username,
                new PasswordAuthenticationMethod(username, password))
            {
                Timeout = TimeSpan.FromSeconds(5) // 5 second timeout for quick feedback
            };

            using var client = new SshClient(connectionInfo);
            await Task.Run(() => client.Connect());

            var modes = new Dictionary<TerminalModes, uint>();
            using var stream = client.CreateShellStream("xterm-color", 80, 24, 800, 600, 1024, modes);

            var cts = new CancellationTokenSource();
            
            // Task to read from SSH shell and write to WebSocket
            var sshToWs = Task.Run(async () =>
            {
                var sshBuffer = new byte[1024];
                while (!cts.IsCancellationRequested && webSocket.State == WebSocketState.Open && client.IsConnected)
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = await stream.ReadAsync(sshBuffer, 0, sshBuffer.Length, cts.Token);
                        if (bytesRead > 0)
                        {
                            await webSocket.SendAsync(new ArraySegment<byte>(sshBuffer, 0, bytesRead), WebSocketMessageType.Text, true, cts.Token);
                        }
                    }
                    else
                    {
                        await Task.Delay(10, cts.Token);
                    }
                }
            });

            // Task to read from WebSocket and write to SSH shell
            var wsToSsh = Task.Run(async () =>
            {
                var wsBuffer = new byte[1024];
                while (!cts.IsCancellationRequested && webSocket.State == WebSocketState.Open && client.IsConnected)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(wsBuffer), cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", cts.Token);
                        cts.Cancel();
                        break;
                    }
                    else if (result.Count > 0)
                    {
                        await stream.WriteAsync(wsBuffer, 0, result.Count, cts.Token);
                        await stream.FlushAsync(cts.Token);
                    }
                }
            });

            try
            {
                await Task.WhenAny(sshToWs, wsToSsh);
            }
            finally
            {
                cts.Cancel();
                if (client.IsConnected)
                {
                    client.Disconnect();
                }
            }
        }

        private async Task<string> ReadLineAsync(WebSocket webSocket, bool maskInput)
        {
            var sb = new StringBuilder();
            var buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) break;
                if (result.Count == 0) continue;

                string data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                bool enterPressed = false;

                foreach (char c in data)
                {
                    if (c == '\r' || c == '\n')
                    {
                        await SendTextAsync(webSocket, "\r\n");
                        enterPressed = true;
                        break;
                    }
                    else if (c == '\b' || c == '\u007f') // Backspace
                    {
                        if (sb.Length > 0)
                        {
                            sb.Length--;
                            await SendTextAsync(webSocket, "\b \b");
                        }
                    }
                    else if (!char.IsControl(c))
                    {
                        sb.Append(c);
                        if (maskInput)
                            await SendTextAsync(webSocket, "*");
                        else
                            await SendTextAsync(webSocket, c.ToString());
                    }
                }
                
                if (enterPressed) break;
            }
            return sb.ToString();
        }

        private static async Task SendTextAsync(WebSocket webSocket, string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
