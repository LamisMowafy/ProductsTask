namespace Infrastructure.Common
{
    public class SearchCriteria
    {
        public string SearchField { get; set; } 
        public string SearchText { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SearchCriteria()
        {
            SearchField = "";  // Initial value for SearchField
            SearchText = "";    // Initial value for SearchText
            PageNumber = 1;                // Initial value for PageNumber
            PageSize = 10;                 // Initial value for PageSize
        }
    }
}
