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

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterStore_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

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

            var list = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Tienda Test"
            });

            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.StoreName == "Tienda Test");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListStores_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.StoreName!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.Manager!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.Address!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.City!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusStore));
        }

        // ===================== SELECT LIST =====================

        [Fact]
        public async Task SelectListStores_WhenCalled_ReturnsActiveStores()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.SelectListStores();

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
        }

        // ===================== STORE BY ID =====================

        [Fact]
        public async Task StoreById_WhenIdExists_ReturnsStore()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var storeId = 1;

            var result = await context.StoreById(storeId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(storeId, result.Data!.IdStore);
        }

        [Fact]
        public async Task StoreById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.StoreById(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditStore_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

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

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

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

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditStore_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            // Arrange
            await context.RegisterStore(1, new StoreRequestDto()
            {
                StoreName = "Tienda Para Editar",
                Manager = "Gerente Original",
                Address = "Av. Original 123",
                PhoneNumber = "77700001",
                City = "Cochabamba",
                Email = "original@test.com",
                ProfitMargin = 0.10m,
                Type = "Sucursal",
            });

            var list = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Tienda Para Editar"
            });
            var storeId = list.Data!.First().IdStore;

            // Act
            var result = await context.EditStore(1, storeId, new StoreRequestDto()
            {
                StoreName = "Tienda Editada",
                Manager = "Gerente Editado",
                Address = "Av. Editada 456",
                PhoneNumber = "77700002",
                City = "La Paz",
                Email = "editada@test.com",
                ProfitMargin = 0.15m,
                Type = "Casa Matriz",
            });

            var updated = await context.StoreById(storeId);

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Tienda Editada", updated.Data!.StoreName);
            Assert.Equal("Gerente Editado", updated.Data!.Manager);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.EnableStore(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableStore_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            // Arrange
            var storeId = 1;
            await context.DisableStore(1, storeId);

            // Act
            var result = await context.EnableStore(1, storeId);

            var store = await context.StoreById(storeId);

            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), store.Data!.StatusStore);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.DisableStore(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableStore_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            // Arrange
            var storeId = 1;
            await context.EnableStore(1, storeId);

            // Act
            var result = await context.DisableStore(1, storeId);

            var store = await context.StoreById(storeId);

            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), store.Data!.StatusStore);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveStore_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            var result = await context.RemoveStore(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveStore_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IStoreService>();

            // Arrange
            await context.RegisterStore(1, new StoreRequestDto()
            {
                StoreName = "Tienda Para Eliminar",
                Manager = "Gerente Eliminar",
                Address = "Av. Eliminar 999",
                PhoneNumber = "77799999",
                City = "Cochabamba",
                Email = "eliminar@test.com",
                ProfitMargin = 0.10m,
                Type = "Sucursal",
            });

            var list = await context.ListStores(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Tienda Para Eliminar"
            });
            var storeId = list.Data!.First().IdStore;

            // Act
            var result = await context.RemoveStore(1, storeId);

            var deleted = await context.StoreById(storeId);

            Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusStore);
        }
    }
}
