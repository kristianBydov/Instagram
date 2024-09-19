using Meta.Instagram.Infrastructure.DTOs;
using Meta.Instagram.Infrastructure.DTOs.Contracts;
using Meta.Instagram.Infrastructure.DTOs.Requests;
using Meta.Instagram.Infrastructure.Exceptions;
using Meta.Instagram.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Meta.Instagram.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost, Route("accounts")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountContract>> PostAccountAsync([BindRequired, FromBody] CreateAccountRequest request)
        {
            try
            {
                var createdAccount = await _accountService.PostAccountAsync(request).ConfigureAwait(false);

                return Created(createdAccount.AccountId, createdAccount);
            }
            catch (AuthenticationException ex)
            {
                var errorContract = new ErrorContract
                {
                    Details = ex.Message,
                    Title = "Create Account Failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                return new ObjectResult(errorContract)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            catch (DatabaseException ex)
            {
                var errorContract = new ErrorContract
                {
                    Details = ex.Message,
                    Title = "Create Account Failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                return new ObjectResult(errorContract)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            catch (Exception ex)
            {
                var errorContract = new ErrorContract
                {
                    Details = ex.Message,
                    Title = "Create Account Failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                return new ObjectResult(errorContract)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
