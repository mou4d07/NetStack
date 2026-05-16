namespace NetMapManager.API.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty; // SFP, Copper
        public string PatchCordType { get; set; } = string.Empty; // LC, SC, ST, RJ45
        public int StockLevel { get; set; }
    }
}
