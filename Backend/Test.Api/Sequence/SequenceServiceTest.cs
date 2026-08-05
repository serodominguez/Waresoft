using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Sequence
{
    public class SequenceServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SequenceServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== VIEW PRODUCT CODE =====================

        [Fact]
        public async Task ViewProductCode_WhenCalled_ReturnsCode()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISequenceService>();

            var result = await context.ViewProductCode();

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.False(string.IsNullOrEmpty(result.Data));
        }
    }
}
