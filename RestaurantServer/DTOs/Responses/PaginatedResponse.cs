namespace RestaurantServer.DTOs.Responses
{
    public class PaginatedResponse
    {
        public PaginatedResponse(int currentPage, int pageSize, int totalRecords)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }
}
