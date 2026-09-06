using RestaurantServer.Constants;
using RestaurantServer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantServer.Models
{
    public class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            RestaurantOwners = new HashSet<RestaurantOwner>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public User(string name, string email, string passwordHash): this()
        {
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Balance = 1000m;
            Role = (int)UserRole.Customer;
            IsActive = true;
            MobileNumber = null;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(ValidationConstants.EmailMaxLength)]
        [Index("IX_User_Email", IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [MaxLength(ValidationConstants.PasswordHashMaxLength)]
        public string PasswordHash { get; set; }

        [Range(typeof(decimal), "0", ValidationConstants.DecimalMax)]
        public decimal Balance { get; set; }

        [Required]
        public int Role { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(ValidationConstants.MobileNumberMaxLength)]
        public string MobileNumber { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<RestaurantOwner> RestaurantOwners { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
