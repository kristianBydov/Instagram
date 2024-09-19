using Auth0.ManagementApi.Models;
using AutoMapper;
using Meta.Instagram.Bussines.Services;
using Meta.Instagram.Infrastructure.DTOs.Contracts;
using Meta.Instagram.Infrastructure.DTOs.Requests;
using Meta.Instagram.Infrastructure.Entities;
using Meta.Instagram.Infrastructure.Exceptions;
using Meta.Instagram.Infrastructure.Repositories;
using Meta.Instagram.Infrastructure.Services;
using Moq;

namespace Meta.Instagram.UnitTests.Tests
{
    public class AccountsTests
    {
        private readonly Mock<IAuthenticationService> _mockAuth0Service;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IMapper> _mockMapper;

        public AccountsTests()
        {
            _mockAuth0Service = new Mock<IAuthenticationService>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockMapper = new Mock<IMapper>();
        }

        #region POST Account

        [Fact]
        public async Task PostAccountAsync_ShouldReturnAccountContract_WhenSuccessful()
        {
            // Arrange
            var request = new CreateAccountRequest { Phone = "1234567890" };
            var auth0User = new User();
            var domainAccount = new Account { Phone = request.Phone };
            var createdAccount = new Account { /* Initialize properties */ };
            var accountContract = new AccountContract { /* Initialize properties */ };

            _mockAuth0Service
                .Setup(x => x.CreateAuth0UserAsync(request))
                .ReturnsAsync(auth0User);
            _mockMapper
                .Setup(x => x.Map<Account>(auth0User))
                .Returns(domainAccount);
            _mockAccountRepository
                .Setup(x => x.CreateAccountAsync(domainAccount))
                .ReturnsAsync(createdAccount);
            _mockMapper
                .Setup(x => x.Map<AccountContract>(createdAccount))
                .Returns(accountContract);

            // Act
            var service = CreateTestService();
            var result = await service.PostAccountAsync(request);

            // Assert
            Assert.Equal(accountContract, result);
            _mockAuth0Service.Verify(x => x.CreateAuth0UserAsync(request), Times.Once);
            _mockMapper.Verify(x => x.Map<Account>(auth0User), Times.Once);
            _mockAccountRepository.Verify(x => x.CreateAccountAsync(domainAccount), Times.Once);
            _mockMapper.Verify(x => x.Map<AccountContract>(createdAccount), Times.Once);
        }

        [Fact]
        public async Task PostAccountAsync_ShouldThrowAuthenticationException_WhenAuth0Fails()
        {
            // Arrange
            var request = new CreateAccountRequest();
            _mockAuth0Service
                .Setup(x => x.CreateAuth0UserAsync(request))
                .ThrowsAsync(new AuthenticationException("Auth0 error"));

            // Act & Assert
            var service = CreateTestService();
            await Assert.ThrowsAsync<AuthenticationException>(() => service.PostAccountAsync(request));
        }

        [Fact]
        public async Task PostAccountAsync_ShouldThrowDatabaseException_WhenDatabaseFails()
        {
            // Arrange
            var request = new CreateAccountRequest();
            var auth0User = new User();
            _mockAuth0Service
                .Setup(x => x.CreateAuth0UserAsync(request))
                .ReturnsAsync(auth0User);
            _mockMapper
                .Setup(x => x.Map<Account>(auth0User))
                .Returns(new Account());
            _mockAccountRepository
                .Setup(x => x.CreateAccountAsync(It.IsAny<Account>()))
                .ThrowsAsync(new DatabaseException("Database error"));

            // Act & Assert
            var service = CreateTestService();
            await Assert.ThrowsAsync<DatabaseException>(() => service.PostAccountAsync(request));
        }

        #endregion

        #region CHANGE PASSWORD Account

        [Fact]
        public async Task ChangeAccountPasswordAsync_ShouldUpdatePassword_WhenAccountExists()
        {
            // Arrange
            var accountId = "123";
            var externalId = "auth0|externalId";
            var newPassword = "NewSecurePassword!";

            var request = new ChangeAccountPasswordRequest
            {
                AccountId = accountId,
                NewPassword = newPassword
            };

            var account = new Account
            {
                AccountId = accountId,
                ExternalId = externalId,
                UpdatedAt = DateTime.Now.AddDays(-1) // Set to some past date
            };

            _mockAccountRepository.Setup(repo => repo.GetAccountAsync(accountId))
                .ReturnsAsync(account);

            _mockAuth0Service.Setup(auth => auth.ChangeAuth0UserPasswordAsync(externalId, newPassword))
                .Returns(Task.CompletedTask);

            _mockAccountRepository.Setup(repo => repo.UpdateAccountAsync(It.IsAny<Account>()))
                .Returns(Task.CompletedTask);

            // Act
            var service = CreateTestService();
            await service.ChangeAccountPasswordAsync(request);

            // Assert
            _mockAccountRepository.Verify(repo => repo.GetAccountAsync(accountId), Times.Once);
            _mockAuth0Service.Verify(auth => auth.ChangeAuth0UserPasswordAsync(externalId, newPassword), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.Is<Account>(a => a.UpdatedAt > DateTime.Now.AddSeconds(-1))), Times.Once);
        }

        [Fact]
        public async Task ChangeAccountPasswordAsync_ShouldThrowNotFoundException_WhenAccountDoesNotExist()
        {
            // Arrange
            var request = new ChangeAccountPasswordRequest
            {
                AccountId = "123",
                NewPassword = "NewSecurePassword!"
            };

            _mockAccountRepository.Setup(repo => repo.GetAccountAsync(request.AccountId))
                .ReturnsAsync((Account)null);

            // Act & Assert
            var service = CreateTestService();
            await Assert.ThrowsAsync<NotFoundException>(() => service.ChangeAccountPasswordAsync(request));

            _mockAccountRepository.Verify(repo => repo.GetAccountAsync(request.AccountId), Times.Once);
            _mockAuth0Service.Verify(auth => auth.ChangeAuth0UserPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task ChangeAccountPasswordAsync_ShouldThrowAuthenticationException_WhenAuthenticationFails()
        {
            // Arrange
            var accountId = "123";
            var externalId = "auth0|externalId";
            var newPassword = "NewSecurePassword!";

            var request = new ChangeAccountPasswordRequest
            {
                AccountId = accountId,
                NewPassword = newPassword
            };

            var account = new Account
            {
                AccountId = accountId,
                ExternalId = externalId
            };

            _mockAccountRepository.Setup(repo => repo.GetAccountAsync(accountId))
                .ReturnsAsync(account);

            _mockAuth0Service.Setup(auth => auth.ChangeAuth0UserPasswordAsync(externalId, newPassword))
                .ThrowsAsync(new AuthenticationException("Authentication failed."));

            // Act & Assert
            var service = CreateTestService();
            await Assert.ThrowsAsync<AuthenticationException>(() => service.ChangeAccountPasswordAsync(request));

            _mockAccountRepository.Verify(repo => repo.GetAccountAsync(accountId), Times.Once);
            _mockAuth0Service.Verify(auth => auth.ChangeAuth0UserPasswordAsync(externalId, newPassword), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateAccountAsync(It.IsAny<Account>()), Times.Never);
        }

        #endregion

        private AccountService CreateTestService()
        {
            return new AccountService(_mockAccountRepository.Object, _mockAuth0Service.Object, _mockMapper.Object);
        }
    }
}
