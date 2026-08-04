using Application.Commons.Bases.Request;
using Application.Dtos.Request.Supplier;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Supplier
{
    public class SupplierServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SupplierServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterSupplier_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.RegisterSupplier(1, new SupplierRequestDto()
            {
                CompanyName = "",
                Contact = "",
                PhoneNumber = "",
                Email = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterSupplier_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.RegisterSupplier(1, new SupplierRequestDto()
            {
                CompanyName = "Proveedor Test",
                Contact = "Juan Perez",
                PhoneNumber = "77712345",
                Email = "proveedor@test.com",
            });

            var list = await context.ListSuppliers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Proveedor Test"
            });

            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.CompanyName == "Proveedor Test");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListSuppliers_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.ListSuppliers(new BaseFiltersRequest()
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
        public async Task ListSuppliers_WhenFilteringByCompanyName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.ListSuppliers(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.CompanyName!.ToLower()));
        }

        [Fact]
        public async Task ListSuppliers_WhenFilteringByContact_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.ListSuppliers(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.Contact!.ToLower()));
        }

        [Fact]
        public async Task ListSuppliers_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.ListSuppliers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusSupplier));
        }

        // ===================== SELECT LIST =====================

        [Fact]
        public async Task SelectListSuppliers_WhenCalled_ReturnsActiveSuppliers()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.SelectListSuppliers();

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
        }

        // ===================== SUPPLIER BY ID =====================

        [Fact]
        public async Task SupplierById_WhenIdExists_ReturnsSupplier()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var supplierId = 1;

            var result = await context.SupplierById(supplierId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(supplierId, result.Data!.IdSupplier);
        }

        [Fact]
        public async Task SupplierById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.SupplierById(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditSupplier_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.EditSupplier(1, 1, new SupplierRequestDto()
            {
                CompanyName = "",
                Contact = "",
                PhoneNumber = "",
                Email = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditSupplier_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.EditSupplier(1, 999999, new SupplierRequestDto()
            {
                CompanyName = "Proveedor Editado",
                Contact = "Juan Perez",
                PhoneNumber = "77712345",
                Email = "editado@test.com",
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditSupplier_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            // Arrange
            await context.RegisterSupplier(1, new SupplierRequestDto()
            {
                CompanyName = "Proveedor Para Editar",
                Contact = "Contacto Original",
                PhoneNumber = "77700001",
                Email = "original@test.com",
            });

            var list = await context.ListSuppliers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Proveedor Para Editar"
            });
            var supplierId = list.Data!.First().IdSupplier;

            // Act
            var result = await context.EditSupplier(1, supplierId, new SupplierRequestDto()
            {
                CompanyName = "Proveedor Editado",
                Contact = "Contacto Editado",
                PhoneNumber = "77700002",
                Email = "editado@test.com",
            });

            var updated = await context.SupplierById(supplierId);

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Proveedor Editado", updated.Data!.CompanyName);
            Assert.Equal("Contacto Editado", updated.Data!.Contact);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableSupplier_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.EnableSupplier(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableSupplier_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            // Arrange
            var supplierId = 1;
            await context.DisableSupplier(1, supplierId);

            // Act
            var result = await context.EnableSupplier(1, supplierId);

            var supplier = await context.SupplierById(supplierId);

            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), supplier.Data!.StatusSupplier);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableSupplier_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.DisableSupplier(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableSupplier_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            // Arrange
            var supplierId = 1;
            await context.EnableSupplier(1, supplierId);

            // Act
            var result = await context.DisableSupplier(1, supplierId);

            var supplier = await context.SupplierById(supplierId);

            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), supplier.Data!.StatusSupplier);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveSupplier_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            var result = await context.RemoveSupplier(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveSupplier_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ISupplierService>();

            // Arrange
            await context.RegisterSupplier(1, new SupplierRequestDto()
            {
                CompanyName = "Proveedor Para Eliminar",
                Contact = "Contacto Eliminar",
                PhoneNumber = "77799999",
                Email = "eliminar@test.com",
            });

            var list = await context.ListSuppliers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Proveedor Para Eliminar"
            });
            var supplierId = list.Data!.First().IdSupplier;

            // Act
            var result = await context.RemoveSupplier(1, supplierId);

            var deleted = await context.SupplierById(supplierId);

            Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusSupplier);
        }
    }
}
