using RestaurantServer.Models;
using System;

namespace RestaurantServer.DTOs.Responses
{
    public class OrderResponse
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
