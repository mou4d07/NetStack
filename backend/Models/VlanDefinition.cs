namespace NetMapManager.API.Models
{
    public class VlanDefinition
    {
        public int Id { get; set; }
        public int VlanId { get; set; } // The actual VLAN number (e.g., 10)
        public string Name { get; set; } = string.Empty; // e.g., "Management", "Production"
        public string Subnet { get; set; } = string.Empty; // e.g., "192.168.10.0/24"
        public string Color { get; set; } = "#8b5cf6"; // Default purple
    }
}
