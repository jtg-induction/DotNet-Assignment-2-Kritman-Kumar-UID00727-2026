using RestaurantServer.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.Models
{
    public class RefreshToken
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [Required]
        [MaxLength(ValidationConstants.RefreshTokenMaxLength)]
        public string Token { get; set; }

        [Required]
        public bool IsRevoked { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        // Navigation
        public virtual User User { get; set; }

        public RefreshToken()
        {

        }

        public RefreshToken(long userId)
        {
            UserId = userId;
            IsRevoked = false;
            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
            ExpiresAt = now.AddDays(30);
        }
    }
}
