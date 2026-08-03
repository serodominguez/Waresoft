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

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.RegisterBrand(1, new BrandRequestDto()
            {
                BrandName = "",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterBrand_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_SAVE;

            var result = await context.RegisterBrand(1, new BrandRequestDto()
            {
                BrandName = "Brand Test",
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListBrands_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.ListBrands(new BaseFiltersRequest()
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
        public async Task ListBrands_WhenFilteringByName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var result = await context.ListBrands(new BaseFiltersRequest()
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
        public async Task ListBrands_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var result = await context.ListBrands(new BaseFiltersRequest()
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
        public async Task SelectListBrands_WhenCalled_ReturnsActiveBrands()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.SelectListBrands();

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
        }

        // ===================== BRAND BY ID =====================

        [Fact]
        public async Task BrandById_WhenIdExists_ReturnsBrand()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var brandId = 1; // ID real en tu BD
            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.BrandById(brandId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task BrandById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.BrandById(999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditBrand_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.EditBrand(1, 1, new BrandRequestDto()
            {
                BrandName = "",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EditBrand(1, 999999, new BrandRequestDto()
            {
                BrandName = "Brand Editada",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditBrand_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var brandId = 1; // ID real en tu BD
            var expected = ReplyMessage.MESSAGE_UPDATE;

            var result = await context.EditBrand(1, brandId, new BrandRequestDto()
            {
                BrandName = "Brand Original",
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EnableBrand(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableBrand_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var brandId = 2; // Brand con IsActive = 0 en tu BD
            var expected = ReplyMessage.MESSAGE_ACTIVATE;

            var result = await context.EnableBrand(1, brandId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.DisableBrand(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableBrand_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var brandId = 1; // Brand con IsActive = 1 en tu BD
            var expected = ReplyMessage.MESSAGE_INACTIVATE;

            var result = await context.DisableBrand(1, brandId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveBrand_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.RemoveBrand(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveBrand_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IBrandService>();

            var brandId = 5; // Brand dedicada para remove en tu BD
            var expected = ReplyMessage.MESSAGE_DELETE;

            var result = await context.RemoveBrand(1, brandId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }
    }
}
