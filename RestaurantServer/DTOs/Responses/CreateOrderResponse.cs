using RestaurantServer.Constants;
using RestaurantServer.Enums;
using RestaurantServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantServer.DTOs.Responses
{
    public class CreateOrderResponse
    {
        public CreateOrderResponse()
        {
            OrderItems = new List<OrderItemDto>();
        }

        public CreateOrderResponse(Order order, string message = SuccessMessages.OrderPlacedSuccessfully)
        {
            OrderId = order.Id;
            RestaurantId = order.RestaurantId;
            UserId = order.UserId;

            Status = (OrderStatus)order.Status;

            TotalPrice = order.TotalPrice;
            AddressLine1 = order.AddressLine1;
            AddressLine2 = order.AddressLine2;
            City = order.City;
            PostalCode = order.PostalCode;
            Country = order.Country;
            CreatedAt = order.CreatedAt;
            Message = message;

            OrderItems = order.OrderItems != null
                ? order.OrderItems.Select(orderItem => new OrderItemDto(orderItem)).ToList()
                : new List<OrderItemDto>();
        }

        public long OrderId { get; set; }
        public long RestaurantId { get; set; }
        public long UserId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
