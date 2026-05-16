using System;

namespace NetMapManager.API.Models
{
    public class MaintenanceLog
    {
        public int Id { get; set; }
        public int SwitchId { get; set; }
        public NetworkSwitch Switch { get; set; } = null!;
        public string Action { get; set; } = string.Empty; // e.g., "Replacement", "Configuration"
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Details { get; set; } = string.Empty;
    }
}
