using Application.Commons.Bases.Request;
using Application.Dtos.Request.StoreInventory;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.StoreInventory
{
    public class StoreInventoryServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public StoreInventoryServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== LIST INVENTORY =====================

        [Fact]
        public async Task ListInventory_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListInventory(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "IdProduct",
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListInventory_WhenFilteringByCode_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListInventory(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "IdProduct",
                Download = false,
                NumberFilter = 1,
                TextFilter = "001"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("001", x.Code!));
        }

        [Fact]
        public async Task ListInventory_WhenFilteringByDateRange_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListInventory(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "IdProduct",
                Download = false,
                StartDate = "2024-01-01",
                EndDate = "2025-12-31"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        // ===================== LIST INVENTORY CALCULATED =====================

        [Fact]
        public async Task ListInventoryCalculated_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListInventoryCalculated(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListInventoryCalculated_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListInventoryCalculated(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        // ===================== LIST INVENTORY PIVOT =====================

        [Fact]
        public async Task ListInventoryPivot_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListInventoryPivot(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        // ===================== LIST KARDEX INVENTORY =====================

        [Fact]
        public async Task ListKardexInventory_WhenProductExists_ReturnsKardex()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var productId = 1;

            var result = await context.ListKardexInventory(1, productId, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListKardexInventory_WhenProductNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListKardexInventory(1, 999999, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task ListKardexInventory_WhenFilteringByDateRange_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.ListKardexInventory(1, 1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Download = false,
                StartDate = "2024-01-01",
                EndDate = "2025-12-31"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        // ===================== UPDATE MINIMUM AND PRICE =====================

        [Fact]
        public async Task UpdateMinimumAndPriceByProduct_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.UpdateMinimumAndPriceByProduct(1, 1, new StoreInventoryRequestDto()
            {
                IdProduct = 0,
                Price = 0,
                MinimumStock = 0,
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task UpdateMinimumAndPriceByProduct_WhenProductNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            var result = await context.UpdateMinimumAndPriceByProduct(1, 1, new StoreInventoryRequestDto()
            {
                IdProduct = 999999,
                Price = 10.50m,
                MinimumStock = 5,
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateMinimumAndPriceByProduct_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreInventoryService>();

            // Arrange - leer valores actuales para restaurarlos luego
            var current = (await context.ListInventoryCalculated(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Download = false
            })).Data!.First(x => x.IdProduct == 1);

            // Act
            var result = await context.UpdateMinimumAndPriceByProduct(1, 1, new StoreInventoryRequestDto()
            {
                IdProduct = 1,
                Price = 10.50m,
                MinimumStock = 5,
            });

            var updatedItem = (await context.ListInventoryCalculated(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Download = false
            })).Data!.First(x => x.IdProduct == 1);

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(10.50m, updatedItem.Price);
            Assert.Equal(5, updatedItem.MinimumStock);

            // Teardown - restaurar valores originales
            await context.UpdateMinimumAndPriceByProduct(1, 1, new StoreInventoryRequestDto()
            {
                IdProduct = 1,
                Price = current.Price,
                MinimumStock = current.MinimumStock,
            });
        }
    }
}
