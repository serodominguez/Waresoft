using Application.Commons.Bases.Request;
using Application.Dtos.Request.Store;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Store
{
    public class StoreServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public StoreServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterStore_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.RegisterStore(1, new StoreRequestDto()
            {
                StoreName = "",
                Manager = "",
                Address = "",
                PhoneNumber = "",
                City = "",
                Email = "",
                ProfitMargin = 0,
                Type = "",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterStore_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_SAVE;

            var result = await context.RegisterStore(1, new StoreRequestDto()
            {
                StoreName = "Tienda Test",
                Manager = "Juan Perez",
                Address = "Av. Test 123",
                PhoneNumber = "77712345",
                City = "Cochabamba",
                Email = "tienda@test.com",
                ProfitMargin = 0.10m,
                Type = "Sucursal",
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListStores_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListStores_WhenFilteringByStoreName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "a"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListStores_WhenFilteringByManager_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 2,
                TextFilter = "a"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListStores_WhenFilteringByAddress_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 3,
                TextFilter = "a"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListStores_WhenFilteringByCity_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 4,
                TextFilter = "a"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListStores_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        // ===================== SELECT LIST =====================

        [Fact]
        public async Task SelectListStores_WhenCalled_ReturnsActiveStores()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.SelectListStores();

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        // ===================== STORE BY ID =====================

        [Fact]
        public async Task StoreById_WhenIdExists_ReturnsStore()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var storeId = 1; // ID real en tu BD
            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.StoreById(storeId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task StoreById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.StoreById(999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditStore_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.EditStore(1, 1, new StoreRequestDto()
            {
                StoreName = "",
                Manager = "",
                Address = "",
                PhoneNumber = "",
                City = "",
                Email = "",
                ProfitMargin = 0,
                Type = "",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EditStore(1, 999999, new StoreRequestDto()
            {
                StoreName = "Tienda Editada",
                Manager = "Juan Perez",
                Address = "Av. Test 123",
                PhoneNumber = "77712345",
                City = "Cochabamba",
                Email = "editada@test.com",
                ProfitMargin = 0.15m,
                Type = "Casa Matriz",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditStore_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var storeId = 1; // ID real en tu BD
            var expected = ReplyMessage.MESSAGE_UPDATE;

            var result = await context.EditStore(1, storeId, new StoreRequestDto()
            {
                StoreName = "Tienda Editada",
                Manager = "Juan Perez",
                Address = "Av. Test 123",
                PhoneNumber = "77712345",
                City = "Cochabamba",
                Email = "editada@test.com",
                ProfitMargin = 0.15m,
                Type = "Casa Matriz",
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EnableStore(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableStore_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var storeId = 2; // Store con IsActive = false en tu BD
            var expected = ReplyMessage.MESSAGE_ACTIVATE;

            var result = await context.EnableStore(1, storeId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.DisableStore(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableStore_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var storeId = 1; // Store con IsActive = true en tu BD
            var expected = ReplyMessage.MESSAGE_INACTIVATE;

            var result = await context.DisableStore(1, storeId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.RemoveStore(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveStore_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var storeId = 5; // Store dedicado para remove en tu BD
            var expected = ReplyMessage.MESSAGE_DELETE;

            var result = await context.RemoveStore(1, storeId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }
    }
}
