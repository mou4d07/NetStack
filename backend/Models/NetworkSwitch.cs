using System.Collections.Generic;

namespace NetMapManager.API.Models
{
    public class NetworkSwitch
    {
        public int Id { get; set; }
        public int? SwitchModelId { get; set; }
        public SwitchModel? SwitchModel { get; set; }
        
        public string IPAddress { get; set; } = string.Empty;
        public int TotalPorts { get; set; } = 24; // Default to 24 ports
        public string Role { get; set; } = string.Empty; // Core, Distribution, Access
        public string SerialNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Online"; // Online, Offline, Maintenance
        
        public int? ZoneId { get; set; }
        public Zone? Zone { get; set; }
        
        // Self-referencing relationship for hierarchy
        public int? ParentSwitchId { get; set; }
        public NetworkSwitch? ParentSwitch { get; set; }
        public ICollection<NetworkSwitch> ChildSwitches { get; set; } = new List<NetworkSwitch>();
        
        public string Location { get; set; } = string.Empty;
        
        public string AllowedVlans { get; set; } = string.Empty; // e.g., "10,20,30" or "ALL"
        
        // Blueprint Coordinates for React-Leaflet map (Physical)
        public double PhysicalX { get; set; } = 500;
        public double PhysicalY { get; set; } = 500;
        
        // Logical Topology Coordinates (React Flow)
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }
    }
}
