using RestaurantServer.Constants;
using System;
using System.Collections.Generic;

namespace RestaurantServer.DTOs.Requests
{
    public class TopOrderedItemsRequest
    {
        public int TopItems { get; set; } = ValidationConstants.DefaultTopItems;

        public List<long> ExcludeItemIds { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
