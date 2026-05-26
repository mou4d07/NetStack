using System.Collections.Generic;

namespace NetMapManager.API.Models
{
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#ffffff"; // Hex color
        public string LabelPosition { get; set; } = "top"; // "top", "bottom", "left", "right"
        
        // Logical Topology Coordinates and Dimensions
        public double XCoordinate { get; set; } = 100;
        public double YCoordinate { get; set; } = 100;
        public double Width { get; set; } = 400;
        public double Height { get; set; } = 400;
        
        // Navigation Property
        public ICollection<NetworkSwitch> Switches { get; set; } = new List<NetworkSwitch>();
    }
}
