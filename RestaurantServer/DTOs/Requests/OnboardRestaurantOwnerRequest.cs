using RestaurantServer.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class OnboardRestaurantOwnerRequest
{
    [Required(ErrorMessage = ValidationMessages.EmailRequired)]
    public List<string> Emails { get; set; }
}
