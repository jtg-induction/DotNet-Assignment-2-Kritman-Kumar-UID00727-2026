using RestaurantServer.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantServer.DTOs.Requests
{
    public class OnboardRestaurantOwnerRequest
    {
        [Required(ErrorMessage = ValidationMessages.EmailRequired)]
        [MinLength(1, ErrorMessage = ValidationMessages.OnboardRestaurantOwnerEmailsMinLength)]
        public List<string> Emails { get; set; }
    }
}
