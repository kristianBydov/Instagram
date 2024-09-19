using Auth0.ManagementApi.Models;
using Meta.Instagram.Infrastructure.DTOs.Requests;

namespace Meta.Instagram.Infrastructure.Services
{
    public interface IAuthenticationService
    {
        Task<User> CreateAuth0UserAsync(CreateAccountRequest request);
    }
}
