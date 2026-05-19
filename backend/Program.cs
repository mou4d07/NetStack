using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NetMapManager.API.Data;
using NetMapManager.API.Security;
using NetMapManager.API.Services;
using NetMapManager.API.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Context
builder.Services.AddDbContext<NetMapDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Windows Authentication (Negotiate)
// builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//    .AddNegotiate();

// 3. Custom Authorization Policy
// builder.Services.AddAuthorization(options =>
// {
//     options.FallbackPolicy = options.DefaultPolicy; // Require authenticated user by default
//     
//     // Add custom policy that checks database
//     options.AddPolicy("DbAuthorizedUserPolicy", policy =>
//     {
//         policy.RequireAuthenticatedUser();
//         policy.Requirements.Add(new DbAuthorizedUserRequirement());
//     });
// });

// Register the custom handler
// builder.Services.AddScoped<IAuthorizationHandler, DbAuthorizedUserHandler>();

// 4. CORS configuration (crucial for passing Windows credentials from Next.js)
builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsCors", corsBuilder =>
    {
        corsBuilder.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002") // Next.js Dev Server
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // REQUIRED: For Windows Auth / NTLM over CORS
    });
});

// Register custom services
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<TerminalService>();

// Register background ping monitor
builder.Services.AddHostedService<PingMonitorService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("NextJsCors");
app.UseStaticFiles();

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value != null && context.Request.Path.Value.EndsWith("/ws/terminal", StringComparison.OrdinalIgnoreCase))
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var terminalService = context.RequestServices.GetRequiredService<TerminalService>();
            await terminalService.HandleTerminalSession(context, webSocket);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }
});

// app.UseAuthentication();
// app.UseAuthorization();

// Apply the custom policy globally to all controllers
app.MapControllers(); // .RequireAuthorization("DbAuthorizedUserPolicy");

app.Run();
