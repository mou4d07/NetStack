using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using NetMapManager.API.Data;

namespace NetMapManager.API.Services
{
    public class SwitchPingResult
    {
        public int SwitchId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public bool IsReachable { get; set; }
        public int ConsecutiveFailures { get; set; }
        public long RoundtripMs { get; set; }
        public DateTime LastChecked { get; set; }
    }

    public class PingMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PingMonitorService> _logger;
        
        // Thread-safe dictionary to store latest ping results
        public static ConcurrentDictionary<int, SwitchPingResult> PingResults { get; } = new();

        // How often to run a full ping cycle (seconds)
        private const int PingIntervalSeconds = 240; // 4 minutes
        // Ping timeout per switch (ms)
        private const int PingTimeoutMs = 2000;
        // Number of consecutive failures before marking as DOWN
        public const int FailureThreshold = 3; // Adjusted to 2 failures (8 mins total) since the interval is longer

        public PingMonitorService(IServiceScopeFactory scopeFactory, ILogger<PingMonitorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PingMonitorService started.");

            // Wait briefly for the host to fully start before pinging
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PingAllSwitches(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Normal shutdown — exit gracefully
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during ping cycle.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(PingIntervalSeconds), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _logger.LogInformation("PingMonitorService stopped.");
        }

        private async Task PingAllSwitches(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NetMapDbContext>();

            var switches = await db.NetworkSwitches.ToListAsync(ct);

            // Ping all switches concurrently
            var tasks = switches.Select(sw => PingSwitch(sw.Id, sw.IPAddress, ct));
            await Task.WhenAll(tasks);
        }

        private async Task PingSwitch(int switchId, string ipAddress, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return;

            bool success = false;
            long roundtrip = -1;

            try
            {
                using var pinger = new Ping();
                var reply = await pinger.SendPingAsync(ipAddress, PingTimeoutMs);

                success = reply.Status == IPStatus.Success;
                roundtrip = success ? reply.RoundtripTime : -1;
            }
            catch (PingException)
            {
                success = false;
            }
            catch (Exception)
            {
                success = false;
            }

            PingResults.AddOrUpdate(switchId,
                // Add new entry
                _ => new SwitchPingResult
                {
                    SwitchId = switchId,
                    IpAddress = ipAddress,
                    IsReachable = success,
                    ConsecutiveFailures = success ? 0 : 1,
                    RoundtripMs = roundtrip,
                    LastChecked = DateTime.UtcNow
                },
                // Update existing entry
                (_, existing) =>
                {
                    existing.IsReachable = success;
                    existing.ConsecutiveFailures = success ? 0 : existing.ConsecutiveFailures + 1;
                    existing.RoundtripMs = roundtrip;
                    existing.LastChecked = DateTime.UtcNow;
                    return existing;
                });
        }
    }
}
