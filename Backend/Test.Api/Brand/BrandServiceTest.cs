using Application.Commons.Bases.Request;
using Application.Dtos.Request.Brand;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Brand
{
    public class BrandServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BrandServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterBrand_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.RegisterBrand(1, new BrandRequestDto()
            {
                BrandName = "",
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterBrand_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.RegisterBrand(1, new BrandRequestDto()
            {
                BrandName = "Brand Test",
            });

            // Consultamos para verificar que realmente se guardó en BD
            var list = await context.ListBrands(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Brand Test"
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            // Confirma que realmente se guardó en BD
            Assert.Contains(list.Data!, x => x.BrandName == "Brand Test");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListBrands_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.ListBrands(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            // Confirma que realmente devuelve registros
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListBrands_WhenFilteringByName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.ListBrands(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Brand"
            });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            // Confirma que todos los resultados contienen el texto filtrado
            Assert.All(result.Data!, x => Assert.Contains("brand", x.BrandName!.ToLower()));
        }

        [Fact]
        public async Task ListBrands_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.ListBrands(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            // Confirma que todos están activos usando el enum States
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusBrand));
        }

        // ===================== SELECT LIST =====================

        [Fact]
        public async Task SelectListBrands_WhenCalled_ReturnsActiveBrands()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.SelectListBrands();

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            // Confirma que devuelve al menos un registro
            Assert.True(result.Data!.Any());
        }

        // ===================== BRAND BY ID =====================

        [Fact]
        public async Task BrandById_WhenIdExists_ReturnsBrand()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Arrange
            var brandId = 1;

            // Act
            var result = await context.BrandById(brandId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            // Confirma que devuelve el id correcto
            Assert.Equal(brandId, result.Data!.IdBrand);
        }

        [Fact]
        public async Task BrandById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.BrandById(999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditBrand_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.EditBrand(1, 1, new BrandRequestDto()
            {
                BrandName = "",
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.EditBrand(1, 999999, new BrandRequestDto()
            {
                BrandName = "Brand Editada",
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditBrand_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Arrange — creamos una marca para editar
            await context.RegisterBrand(1, new BrandRequestDto()
            {
                BrandName = "Brand Para Editar",
            });

            // Obtenemos el id de la que acabamos de crear
            var list = await context.ListBrands(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Brand Para Editar"
            });
            var brandId = list.Data!.First().IdBrand;

            // Act
            var result = await context.EditBrand(1, brandId, new BrandRequestDto()
            {
                BrandName = "Brand Editada",
            });

            // Consultamos para verificar que realmente cambió en BD
            var updated = await context.BrandById(brandId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            // Confirma que realmente se editó en BD
            // NormalizeString y ToSentenceCase capitalizan solo la primera letra
            Assert.Equal("Brand Editada", updated.Data!.BrandName);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.EnableBrand(1, 999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableBrand_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Arrange — desactivamos primero para tener un estado conocido
            var brandId = 1;
            await context.DisableBrand(1, brandId);

            // Act
            var result = await context.EnableBrand(1, brandId);

            // Consultamos para verificar que realmente cambió en BD
            var brand = await context.BrandById(brandId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            // Confirma que realmente se activó en BD usando el enum States
            Assert.Equal(States.Activo.ToString(), brand.Data!.StatusBrand);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.DisableBrand(1, 999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableBrand_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Arrange — activamos primero para tener un estado conocido
            var brandId = 1;
            await context.EnableBrand(1, brandId);

            // Act
            var result = await context.DisableBrand(1, brandId);

            // Consultamos para verificar que realmente cambió en BD
            var brand = await context.BrandById(brandId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            // Confirma que realmente se desactivó en BD usando el enum States
            Assert.Equal(States.Inactivo.ToString(), brand.Data!.StatusBrand);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Act
            var result = await context.RemoveBrand(1, 999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveBrand_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            // Arrange — creamos una marca para eliminar
            await context.RegisterBrand(1, new BrandRequestDto()
            {
                BrandName = "Brand Para Eliminar",
            });

            // Obtenemos el id de la que acabamos de crear
            var list = await context.ListBrands(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Brand Para Eliminar"
            });
            var brandId = list.Data!.First().IdBrand;

            // Act
            var result = await context.RemoveBrand(1, brandId);

            // Consultamos para verificar que realmente se eliminó (IsActive = false)
            var deleted = await context.BrandById(brandId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            // Confirma que realmente se marcó como inactivo en BD
            Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusBrand);
        }
    }
}
