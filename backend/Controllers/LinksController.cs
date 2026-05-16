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
    public class LinksController : ControllerBase
    {
        private readonly NetMapDbContext _dbContext;

        public LinksController(NetMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Link>>> GetLinks()
        {
            return await _dbContext.Links
                .Include(l => l.SourceSwitch)
                .Include(l => l.DestinationSwitch)
                .ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLink(int id, [FromBody] Link updatedLink)
        {
            if (id != updatedLink.Id) return BadRequest("ID mismatch");

            var existingLink = await _dbContext.Links.FindAsync(id);
            if (existingLink == null) return NotFound();

            existingLink.SourcePort = updatedLink.SourcePort;
            existingLink.SourcePortType = updatedLink.SourcePortType;
            existingLink.DestinationPort = updatedLink.DestinationPort;
            existingLink.DestinationPortType = updatedLink.DestinationPortType;
            existingLink.MediaType = updatedLink.MediaType;
            existingLink.SourceSwitchId = updatedLink.SourceSwitchId;
            existingLink.DestinationSwitchId = updatedLink.DestinationSwitchId;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _dbContext.Links.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Link>> CreateLink([FromBody] Link newLink)
        {
            // Validate that the switches exist
            var sourceExists = await _dbContext.NetworkSwitches.AnyAsync(s => s.Id == newLink.SourceSwitchId);
            var destExists = await _dbContext.NetworkSwitches.AnyAsync(s => s.Id == newLink.DestinationSwitchId);

            if (!sourceExists || !destExists) return BadRequest("Source or Destination switch does not exist.");

            _dbContext.Links.Add(newLink);
            await _dbContext.SaveChangesAsync();

            return Ok(newLink);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLink(int id)
        {
            var link = await _dbContext.Links.FindAsync(id);
            if (link == null) return NotFound();

            _dbContext.Links.Remove(link);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
