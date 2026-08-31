using System;
using System.Collections.Generic;

namespace RestaurantServer.DTOs.Requests
{
    public class TopOrderedItemsRequest
    {
        public int TopItems { get; set; } = 10;

        public List<long> ExcludeItemIds { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
