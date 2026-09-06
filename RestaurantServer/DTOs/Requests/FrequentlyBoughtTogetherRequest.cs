using RestaurantServer.Constants;

namespace RestaurantServer.DTOs.Requests
{
    public class FrequentlyBoughtTogetherRequest
    {
        public int TopPairs { get; set; } = ValidationConstants.DefaultTopPair;
    }
}
