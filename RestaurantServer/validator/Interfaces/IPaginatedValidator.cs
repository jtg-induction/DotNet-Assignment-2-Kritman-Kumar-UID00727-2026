using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantServer.validator.Interfaces
{
    public interface IPaginatedValidator
    {
        void ValidatePagination(int page, int pageSize);
    }
}