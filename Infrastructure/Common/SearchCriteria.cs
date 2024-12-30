namespace Infrastructure.Common
{
    public class SearchCriteria
    {
        public string? SearchField { get; set; }
        public string? SearchText { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
