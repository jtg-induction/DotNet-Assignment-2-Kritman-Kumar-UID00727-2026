using System.Collections.Generic;

namespace RestaurantServer.DTOs.Responses
{
    public class RestaurantOwnerResult
    {
        public long RestaurantId { get; set; }
        public string Message { get; set; }
        public List<OnboardRestaurantOwnerResponse> Owners { get; set; }
    }
}
