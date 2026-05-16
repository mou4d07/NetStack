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
    public class VlansController : ControllerBase
    {
        private readonly NetMapDbContext _dbContext;

        public VlansController(NetMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VlanDefinition>>> GetVlans()
        {
            return await _dbContext.VlanDefinitions.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<VlanDefinition>> CreateVlan([FromBody] VlanDefinition vlan)
        {
            _dbContext.VlanDefinitions.Add(vlan);
            await _dbContext.SaveChangesAsync();
            return Ok(vlan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVlan(int id, [FromBody] VlanDefinition updatedVlan)
        {
            if (id != updatedVlan.Id) return BadRequest();
            _dbContext.Entry(updatedVlan).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVlan(int id)
        {
            var vlan = await _dbContext.VlanDefinitions.FindAsync(id);
            if (vlan == null) return NotFound();
            _dbContext.VlanDefinitions.Remove(vlan);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
