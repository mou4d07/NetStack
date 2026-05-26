using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetMapManager.API.Data;
using NetMapManager.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetMapManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonesController : ControllerBase
    {
        private readonly NetMapDbContext _dbContext;

        public ZonesController(NetMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Zone>>> GetZones()
        {
            return await _dbContext.Zones.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Zone>> GetZone(int id)
        {
            var zone = await _dbContext.Zones.FindAsync(id);
            if (zone == null) return NotFound();
            return zone;
        }

        [HttpPost]
        public async Task<ActionResult<Zone>> CreateZone([FromBody] Zone zone)
        {
            _dbContext.Zones.Add(zone);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetZone), new { id = zone.Id }, zone);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateZone(int id, [FromBody] Zone updatedZone)
        {
            if (id != updatedZone.Id) return BadRequest("ID mismatch");

            var existingZone = await _dbContext.Zones.FindAsync(id);
            if (existingZone == null) return NotFound();

            existingZone.Name = updatedZone.Name;
            existingZone.Color = updatedZone.Color;
            existingZone.LabelPosition = updatedZone.LabelPosition;
            existingZone.XCoordinate = updatedZone.XCoordinate;
            existingZone.YCoordinate = updatedZone.YCoordinate;
            existingZone.Width = updatedZone.Width;
            existingZone.Height = updatedZone.Height;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _dbContext.Zones.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZone(int id)
        {
            var zone = await _dbContext.Zones.FindAsync(id);
            if (zone == null) return NotFound();

            // Handle switches in this zone
            var switches = await _dbContext.NetworkSwitches.Where(s => s.ZoneId == id).ToListAsync();
            foreach (var sw in switches)
            {
                sw.ZoneId = null;
            }

            _dbContext.Zones.Remove(zone);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
