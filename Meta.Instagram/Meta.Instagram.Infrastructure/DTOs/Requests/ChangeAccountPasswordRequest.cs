namespace Meta.Instagram.Infrastructure.DTOs.Requests
{
    public class ChangeAccountPasswordRequest
    {
        public string? AccountId { get; set; }
        public string? NewPassword { get; set; }
    }
}
