namespace web_api_remit_rocket.Helpers
{
    public class Paginate<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public IList<T>? Data { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public string NextPageLink { get; set; } = string.Empty;
    }
}
