using System.Collections.Generic;

namespace RestaurantServer.DTOs.Responses
{
    public class OnboardRestaurantResponses
    {
        public long RestaurantId { get; set; }
        public string Message { get; set; }
        public List<OwnerDto> Owners { get; set; }
    }
}
