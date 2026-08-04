using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 

namespace RestaurantServer.Models
{
    public class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }
        [Key]
        public long Id { get; set; }
        public long RestaurantId { get; set; }
        public long UserId { get; set; }
        public int Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Navigation
        public virtual User User { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
