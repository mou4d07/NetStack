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
    public class SwitchModelsController : ControllerBase
    {
        private readonly NetMapDbContext _dbContext;

        public SwitchModelsController(NetMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwitchModel>>> GetSwitchModels()
        {
            return await _dbContext.SwitchModels.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SwitchModel>> CreateSwitchModel([FromBody] SwitchModel model)
        {
            _dbContext.SwitchModels.Add(model);
            await _dbContext.SaveChangesAsync();
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSwitchModel(int id)
        {
            var model = await _dbContext.SwitchModels.FindAsync(id);
            if (model == null) return NotFound();

            _dbContext.SwitchModels.Remove(model);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
