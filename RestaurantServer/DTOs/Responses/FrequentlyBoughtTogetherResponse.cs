namespace RestaurantServer.DTOs.Responses
{
    public class FrequentlyBoughtTogetherResponse
    {
        public long Item1Id { get; set; }

        public string Item1Name { get; set; }

        public long Item2Id { get; set; }

        public string Item2Name { get; set; }

        public long RestaurantId { get; set; }

        public string RestaurantName { get; set; }

        public int TotalTimesBoughtTogether { get; set; }
    }
}
