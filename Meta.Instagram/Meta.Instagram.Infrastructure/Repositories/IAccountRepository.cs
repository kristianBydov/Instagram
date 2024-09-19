using Meta.Instagram.Infrastructure.Entities;

namespace Meta.Instagram.Infrastructure.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> GetAccountAsync(string accountId);
        Task UpdateAccountAsync(Account account);
    }
}
