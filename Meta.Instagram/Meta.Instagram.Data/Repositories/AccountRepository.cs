using Meta.Instagram.Data.Context;
using Meta.Instagram.Infrastructure.Entities;
using Meta.Instagram.Infrastructure.Exceptions;
using Meta.Instagram.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meta.Instagram.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(ApplicationDbContext db, ILogger<AccountRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            try
            {
                var entry = await _db.Accounts.AddAsync(account);

                await _db.SaveChangesAsync();

                return entry.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task<Account> GetAccountAsync(string accountId)
        {
            try
            {
                var account = await _db.Accounts
                    .FirstOrDefaultAsync(x => x.AccountId == accountId);

                return account!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new DatabaseException(ex.Message);
            }
        }

        public async Task UpdateAccountAsync(Account account)
        {
            try
            {
                _db.Accounts.Update(account); 
                
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new DatabaseException(ex.Message);
            }
        }
    }
}
