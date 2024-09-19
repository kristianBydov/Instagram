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

        [HttpPut, Route("accounts/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangeAccountPasswordAsync([BindRequired, FromBody] ChangeAccountPasswordRequest request)
        {
            try
            {
                await _accountService.ChangeAccountPasswordAsync(request).ConfigureAwait(false);

                return NoContent();
            }
            catch (NotFoundException ex)
            {
                var errorContract = new ErrorContract
                {
                    Details = ex.Message,
                    Title = "Resource not found",
                    StatusCode = StatusCodes.Status404NotFound
                };

                return new ObjectResult(errorContract)
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            catch (AuthenticationException ex)
            {
                var errorContract = new ErrorContract
                {
                    Details = ex.Message,
                    Title = "Change Password Failed",
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
                    Title = "Change Password Failed",
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
                    Title = "Change Password Failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                return new ObjectResult(errorContract)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
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
