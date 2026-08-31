namespace RestaurantServer.DTOs.Responses
{
    public class TopOrderedItemResponse
    {
        public int Rank { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public long RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int TotalQuantityOrdered { get; set; }
    }
}
