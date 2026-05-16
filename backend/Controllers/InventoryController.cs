using Microsoft.AspNetCore.Mvc;
using NetMapManager.API.Models;
using NetMapManager.API.Services;
using System.Threading.Tasks;

namespace NetMapManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("maintenance")]
        public async Task<IActionResult> LogMaintenance([FromBody] MaintenanceLog log)
        {
            await _inventoryService.ProcessMaintenanceActionAsync(log);
            return Ok(new { message = "Maintenance logged and inventory updated if applicable." });
        }
    }
}
