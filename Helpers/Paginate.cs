namespace web_api_remit_rocket.Helpers
{
    public class Paginate<T>
    {
        public int currentPage { get; set; }
        public List<T>? data { get; set; }
        public string firstPageUrl { get; set; } = string.Empty;
        public int from { get; set; }
        public int lastPage { get; set; }
        public string lastPageUrl { get; set; } = string.Empty;
        public string? nextPageUrl { get; set; }
        public string path { get; set; } = string.Empty;
        public int perPage { get; set; }
        public string? previousPageUrl { get; set; }
        public int to { get; set; }
        public int total { get; set; }
    }
}
