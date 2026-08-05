using Application.Dtos.Request.User;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Authorization
{
    public class AuthorizationServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AuthorizationServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        [Fact]
        public async Task GenerateToken_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();

            var result = await context.GenerateToken(new TokenRequestDto
            {
                UserName = null,
                Password = null
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task GenerateToken_WhenUserNotExists_ReturnsIncorrectUser()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();

            var result = await context.GenerateToken(new TokenRequestDto
            {
                UserName = "USUARIONOEXI STE",
                Password = "cualquiera"
            });

            Assert.Equal(ReplyMessage.MESSAGE_INCORRECT_USER, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GenerateToken_WhenPasswordIsWrong_ReturnsIncorrectPassword()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();

            var result = await context.GenerateToken(new TokenRequestDto
            {
                UserName = "SDOMINGUEZ",
                Password = "wrongpassword"
            });

            Assert.Equal(ReplyMessage.MESSAGE_INCORRECT_PASSWORD, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GenerateToken_WhenCredentialsAreCorrect_ReturnsToken()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();

            var result = await context.GenerateToken(new TokenRequestDto
            {
                UserName = "SDOMINGUEZ",
                Password = "123"
            });

            Assert.Equal(ReplyMessage.MESSAGE_TOKEN, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.False(string.IsNullOrEmpty(result.Data));
        }
    }
}
