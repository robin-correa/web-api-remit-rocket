namespace web_api_remit_rocket.Helpers
{
    public class PaginationDto<RowData>
    {
        public int currentPage { get; set; }

        public List<RowData>? data { get; set; }

        public string? firstPageUrl { get; set; } = null;

        public int? from { get; set; }

        public int lastPage { get; set; }

        public string? lastPageUrl { get; set; } = null;

        public string? nextPageUrl { get; set; }

        public string? path { get; set; } = null;

        public int perPage { get; set; }

        public string? previousPageUrl { get; set; }

        public int? to { get; set; }
        
        public int total { get; set; }
    }
}
