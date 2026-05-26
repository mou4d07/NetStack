using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetMapManager.API.Data;
using NetMapManager.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetMapManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwitchesController : ControllerBase
    {
        private readonly NetMapDbContext _dbContext;

        public SwitchesController(NetMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NetworkSwitch>>> GetSwitches()
        {
            return await _dbContext.NetworkSwitches
                .Include(s => s.SwitchModel)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NetworkSwitch>> GetSwitch(int id)
        {
            var sw = await _dbContext.NetworkSwitches
                .Include(s => s.SwitchModel)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sw == null) return NotFound();
            return sw;
        }

        [HttpPost]
        public async Task<ActionResult<NetworkSwitch>> CreateSwitch([FromBody] NetworkSwitch newSwitch)
        {
            _dbContext.NetworkSwitches.Add(newSwitch);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSwitch), new { id = newSwitch.Id }, newSwitch);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSwitch(int id, [FromBody] NetworkSwitch updatedSwitch)
        {
            if (id != updatedSwitch.Id) return BadRequest("ID mismatch");

            var existingSwitch = await _dbContext.NetworkSwitches.FindAsync(id);
            if (existingSwitch == null) return NotFound();

            // Update only the properties we want to change
            existingSwitch.IPAddress = updatedSwitch.IPAddress;
            existingSwitch.Role = updatedSwitch.Role;
            existingSwitch.Location = updatedSwitch.Location;
            existingSwitch.XCoordinate = updatedSwitch.XCoordinate;
            existingSwitch.YCoordinate = updatedSwitch.YCoordinate;
            existingSwitch.PhysicalX = updatedSwitch.PhysicalX;
            existingSwitch.PhysicalY = updatedSwitch.PhysicalY;
            existingSwitch.ParentSwitchId = updatedSwitch.ParentSwitchId;
            existingSwitch.SwitchModelId = updatedSwitch.SwitchModelId;
            existingSwitch.TotalPorts = updatedSwitch.TotalPorts;
            existingSwitch.SerialNumber = updatedSwitch.SerialNumber;
            existingSwitch.Status = updatedSwitch.Status;
            existingSwitch.AllowedVlans = updatedSwitch.AllowedVlans;
            existingSwitch.ZoneId = updatedSwitch.ZoneId;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _dbContext.NetworkSwitches.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSwitch(int id)
        {
            var sw = await _dbContext.NetworkSwitches.FindAsync(id);
            if (sw == null) return NotFound();

            _dbContext.NetworkSwitches.Remove(sw);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
