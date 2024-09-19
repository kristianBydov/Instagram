using AutoMapper;
using Meta.Instagram.Infrastructure.DTOs.Contracts;
using Meta.Instagram.Infrastructure.DTOs.Requests;
using Meta.Instagram.Infrastructure.Entities;
using Meta.Instagram.Infrastructure.Exceptions;
using Meta.Instagram.Infrastructure.Repositories;
using Meta.Instagram.Infrastructure.Services;

namespace Meta.Instagram.Bussines.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IAuthenticationService authenticationService, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<AccountContract> PostAccountAsync(CreateAccountRequest request)
        {
            try
            {
                var auth0User = await _authenticationService.CreateAuth0UserAsync(request).ConfigureAwait(false);

                var domainAccount = _mapper.Map<Account>(auth0User);
                domainAccount.Phone = request.Phone;

                var createdAccount = await _accountRepository.CreateAccountAsync(domainAccount).ConfigureAwait(false);

                return _mapper.Map<AccountContract>(createdAccount);
            }
            catch (AuthenticationException ex)
            {
                throw new AuthenticationException(ex.Message);
            }
            catch (DatabaseException ex)
            {
                throw new DatabaseException(ex.Message);
            }
        }
    }
}
