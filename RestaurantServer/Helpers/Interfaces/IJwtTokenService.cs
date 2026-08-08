using RestaurantServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantServer.Helpers.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);

        string GenerateRefreshToken();
    }
}
