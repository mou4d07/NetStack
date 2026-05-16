namespace NetMapManager.API.Models
{
    public class Link
    {
        public int Id { get; set; }
        
        public int SourceSwitchId { get; set; }
        public NetworkSwitch? SourceSwitch { get; set; }
        public string SourcePort { get; set; } = string.Empty;
        public string SourcePortType { get; set; } = "ETHERNET";
        
        public int DestinationSwitchId { get; set; }
        public NetworkSwitch? DestinationSwitch { get; set; }
        public string DestinationPort { get; set; } = string.Empty;
        public string DestinationPortType { get; set; } = "ETHERNET";
        
        public string MediaType { get; set; } = string.Empty; // SFP, Copper
    }
}
