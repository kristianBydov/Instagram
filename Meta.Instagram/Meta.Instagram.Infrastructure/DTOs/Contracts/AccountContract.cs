namespace Meta.Instagram.Infrastructure.DTOs.Contracts
{
    public class AccountContract
    {
        public string? AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
