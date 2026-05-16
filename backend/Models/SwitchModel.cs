using System.Collections.Generic;

namespace NetMapManager.API.Models
{
    public class SwitchModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g. Catalyst 9300
        public string Brand { get; set; } = string.Empty; // e.g. Cisco
        public string ImagePath { get; set; } = string.Empty; // Path to product image
        public int PortCount { get; set; } = 24;
        public string Description { get; set; } = string.Empty;

        public ICollection<NetworkSwitch> Switches { get; set; } = new List<NetworkSwitch>();
    }
}
