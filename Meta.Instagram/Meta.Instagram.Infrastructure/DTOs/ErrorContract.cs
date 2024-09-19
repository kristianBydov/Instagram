namespace Meta.Instagram.Infrastructure.DTOs
{
    public class ErrorContract
    {
        public int StatusCode { get; set; }
        public string? Title { get; set; }
        public string? Details { get; set; }
    }
}
