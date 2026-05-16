using Microsoft.AspNetCore.Mvc;
using NetMapManager.API.Services;

namespace NetMapManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetPingResults()
        {
            var results = PingMonitorService.PingResults.Values
                .Select(r => new
                {
                    switchId = r.SwitchId,
                    ipAddress = r.IpAddress,
                    isReachable = r.IsReachable,
                    consecutiveFailures = r.ConsecutiveFailures,
                    isDown = r.ConsecutiveFailures >= PingMonitorService.FailureThreshold,
                    roundtripMs = r.RoundtripMs,
                    lastChecked = r.LastChecked
                })
                .ToList();

            return Ok(results);
        }

        [HttpGet("{switchId}")]
        public ActionResult GetPingResult(int switchId)
        {
            if (PingMonitorService.PingResults.TryGetValue(switchId, out var result))
            {
                return Ok(new
                {
                    switchId = result.SwitchId,
                    ipAddress = result.IpAddress,
                    isReachable = result.IsReachable,
                    consecutiveFailures = result.ConsecutiveFailures,
                    isDown = result.ConsecutiveFailures >= PingMonitorService.FailureThreshold,
                    roundtripMs = result.RoundtripMs,
                    lastChecked = result.LastChecked
                });
            }

            return NotFound(new { message = "No ping data available yet for this switch." });
        }
    }
}
