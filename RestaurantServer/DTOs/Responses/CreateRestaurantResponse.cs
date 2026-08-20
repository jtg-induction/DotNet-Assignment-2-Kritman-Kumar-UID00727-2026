using RestaurantServer.Models;

namespace RestaurantServer.DTOs.Responses
{
    public class CreateRestaurantResponse
    {
        public long Id { get; set; }
        public string RestaurantName { get; set; }
        public string Description { get; set; }
        public string MobileNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public CreateRestaurantResponse()
        {

        }

        public CreateRestaurantResponse(Restaurant restaurant)
        {
            Id = restaurant.Id;
            RestaurantName = restaurant.RestaurantName;
            Description = restaurant.Description;
            MobileNumber = restaurant.MobileNumber;
            AddressLine1 = restaurant.AddressLine1;
            AddressLine2 = restaurant.AddressLine2;
            City = restaurant.City;
            PostalCode = restaurant.PostalCode;
            Country = restaurant.Country;
        }
    }
}
