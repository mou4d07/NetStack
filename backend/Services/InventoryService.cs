using Microsoft.EntityFrameworkCore;
using NetMapManager.API.Data;
using NetMapManager.API.Models;
using System.Threading.Tasks;

namespace NetMapManager.API.Services
{
    public interface IInventoryService
    {
        Task ProcessMaintenanceActionAsync(MaintenanceLog log);
    }

    public class InventoryService : IInventoryService
    {
        private readonly NetMapDbContext _dbContext;

        public InventoryService(NetMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ProcessMaintenanceActionAsync(MaintenanceLog log)
        {
            if (log.Action.Equals("Replacement", StringComparison.OrdinalIgnoreCase))
            {
                // Simple logic: find inventory items matching media/patch type and decrement
                // In a real app, you'd probably link the log to a specific InventoryItem ID.
                
                var items = await _dbContext.InventoryItems
                    .Where(i => i.StockLevel > 0)
                    .ToListAsync();

                // Logic here would depend on business rules for item matching
                // For demonstration, let's assume we find an item and decrement
                if (items.Any())
                {
                    items[0].StockLevel--;
                    await _dbContext.SaveChangesAsync();
                }
            }
            
            _dbContext.MaintenanceLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
