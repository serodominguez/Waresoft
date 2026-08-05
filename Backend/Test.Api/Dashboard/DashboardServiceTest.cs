using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Static;

namespace Test.Api.Dashboard
{
    public class DashboardServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DashboardServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== GOODS ISSUE STATS =====================

        [Fact]
        public async Task GetGoodsIssueStats_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetGoodsIssueStats(1, CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.TotalIssues >= 0);
        }

        // ===================== INVENTORY STATS =====================

        [Fact]
        public async Task GetInventoryStats_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetInventoryStats(1, CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.BelowMinimum >= 0);
        }

        // ===================== MOVEMENTS STATS =====================

        [Fact]
        public async Task GetMovementsStats_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetMovementsStats(1, CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetMovementsStats_WhenCalled_ReturnsAtMost6Months()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetMovementsStats(1, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.True(result.Data!.Count() <= 6);
        }

        [Fact]
        public async Task GetMovementsStats_WhenCalled_EachItemHasMonth()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetMovementsStats(1, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.All(result.Data!, x => Assert.False(string.IsNullOrEmpty(x.Month)));
        }

        // ===================== PRODUCT REPLENISHMENT =====================

        [Fact]
        public async Task GetProductReplenishment_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetProductReplenishment(1, CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Available >= 0);
            Assert.True(result.Data!.NotAvailable >= 0);
            Assert.True(result.Data!.Discontinued >= 0);
        }

        // ===================== PRODUCT STATS =====================

        [Fact]
        public async Task GetProductStats_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetProductStats(CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.TotalActive >= 0);
            Assert.True(result.Data!.NewThisMonth >= 0);
        }

        // ===================== TRANSFERS BY STORE =====================

        [Fact]
        public async Task GetTransfersByStore_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetTransfersByStore(1, CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetTransfersByStore_WhenCalled_EachItemHasStoreName()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetTransfersByStore(1, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.All(result.Data!, x => Assert.False(string.IsNullOrEmpty(x.StoreName)));
        }

        [Fact]
        public async Task GetTransfersByStore_WhenCalled_TotalsAreNonNegative()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetTransfersByStore(1, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.All(result.Data!, x => Assert.True(x.TotalTransfers >= 0));
        }

        // ===================== TRANSFER PENDING =====================

        [Fact]
        public async Task GetTransferPending_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetTransferPending(1, CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.TotalPending >= 0);
        }

        // ===================== TRANSFER STATUS =====================

        [Fact]
        public async Task GetTransferStatus_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetTransferStatus(1, CancellationToken.None);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetTransferStatus_WhenCalled_ReturnsAtMost6Months()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetTransferStatus(1, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.True(result.Data!.Count() <= 6);
        }

        [Fact]
        public async Task GetTransferStatus_WhenCalled_EachItemHasMonthAndNonNegativeValues()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDashboardService>();

            var result = await context.GetTransferStatus(1, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.All(result.Data!, x =>
            {
                Assert.False(string.IsNullOrEmpty(x.Month));
                Assert.True(x.Sent >= 0);
                Assert.True(x.Pending >= 0);
                Assert.True(x.Received >= 0);
            });
        }
    }
}
