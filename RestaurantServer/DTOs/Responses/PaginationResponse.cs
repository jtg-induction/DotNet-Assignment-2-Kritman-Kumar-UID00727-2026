namespace RestaurantServer.DTOs.Responses
{
    public class PaginationResponse
    {
        public PaginationResponse(int currentPage, int pageSize, int totalRecords, int totalPages)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;

        public int? NextPage => HasNext ? CurrentPage + 1 : (int?)null;
        public int? PreviousPage => HasPrevious ? CurrentPage - 1 : (int?)null;
    }
}
