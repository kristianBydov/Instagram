using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Meta.Instagram.Infrastructure.DTOs.Requests;
using Meta.Instagram.Infrastructure.Exceptions;
using Meta.Instagram.Infrastructure.Options;
using Meta.Instagram.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Meta.Instagram.Bussines.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationApiClient _authenticationApi;
        private readonly Auth0Options _options;

        public AuthenticationService(IOptions<Auth0Options> options)
        {
            _options = options.Value;
            _authenticationApi = new AuthenticationApiClient(_options.Domain);
        }

        public async Task ChangeAuth0UserPasswordAsync(string userId, string newPassword)
        {
            var managementApiClient = new ManagementApiClient(await GetToken(), _options.Domain);

            var updateUserRequest = new UserUpdateRequest
            {
                Password = newPassword,
                Connection = "Username-Password-Authentication"
            };

            var updatedUser = await managementApiClient.Users.UpdateAsync(userId, updateUserRequest)
                ?? throw new AuthenticationException("There was a problem with the password change.");
        }

        public async Task<User> CreateAuth0UserAsync(CreateAccountRequest request)
        {
            try
            {
                var managementApiClient = new ManagementApiClient(await GetToken(), _options.Domain);

                var userCreateRequest = new UserCreateRequest
                {
                    Email = request.Email,
                    Password = request.Password,
                    Connection = _options.Connection,
                    LastName = request.LastName,
                    FirstName = request.FirstName,
                    UserName = request.Username
                };

                var newUser = await managementApiClient.Users.CreateAsync(userCreateRequest);

                return newUser;
            }
            catch (Exception ex)
            {
                throw new AuthenticationException(ex.Message);
            }
        }
        private async Task<string> GetToken()
        {
            var tokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Audience = _options.Audience
            };

            // Send token request and obtain access token
            var tokenResponse = await _authenticationApi.GetTokenAsync(tokenRequest);
            return tokenResponse.AccessToken;
        }
    }
}
