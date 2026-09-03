using RestaurantServer.Constants;
using RestaurantServer.ModelStateValidator;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class OnboardRestaurantOwnerRequest
{
    [Required(ErrorMessage = ValidationMessages.EmailRequired)]
    [EmailListAttribute]
    [CollectionMinLength]
    public List<string> Emails { get; set; } 
}
