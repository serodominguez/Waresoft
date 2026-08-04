using Application.Commons.Bases.Request;
using Application.Dtos.Request.Product;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Product
{
    public class ProductServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ProductServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterProduct_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.RegisterProduct(1, new ProductRequestDto()
            {
                Description = "",
                UnitMeasure = "",
                IdBrand = 0,
                IdCategory = 0,
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterProduct_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_SAVE;

            var result = await context.RegisterProduct(1, new ProductRequestDto()
            {
                Description = "Producto Test",
                Material = "Algodon",
                Color = "Rojo",
                UnitMeasure = "Unidad",
                IdBrand = 1,    // ID real en tu BD
                IdCategory = 1, // ID real en tu BD
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListProducts_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.ListProducts(new BaseFiltersRequest()
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
        public async Task ListProducts_WhenFilteringByCode_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "P"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListProducts_WhenFilteringByDescription_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
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
        public async Task ListProducts_WhenFilteringByMaterial_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
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
        public async Task ListProducts_WhenFilteringByColor_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
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
        public async Task ListProducts_WhenFilteringByBrandName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 5,
                TextFilter = "a"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListProducts_WhenFilteringByCategoryName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 6,
                TextFilter = "a"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListProducts_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
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

        // ===================== PRODUCT BY ID =====================

        [Fact]
        public async Task ProductById_WhenIdExists_ReturnsProduct()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 26;
            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.ProductById(productId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ProductById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.ProductById(999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditProduct_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.EditProduct(1, 1, new ProductRequestDto()
            {
                Description = "",
                UnitMeasure = "",
                IdBrand = 0,
                IdCategory = 0,
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EditProduct(1, 999999, new ProductRequestDto()
            {
                Description = "Producto Editado",
                UnitMeasure = "Unidad",
                IdBrand = 1,
                IdCategory = 1,
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditProduct_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 1; // ID real en tu BD
            var expected = ReplyMessage.MESSAGE_UPDATE;

            var result = await context.EditProduct(1, productId, new ProductRequestDto()
            {
                Description = "Producto Editado",
                Material = "Algodon",
                Color = "Rojo",
                UnitMeasure = "Unidad",
                IdBrand = 1,
                IdCategory = 1,
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EnableProduct(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableProduct_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 2; // Producto con IsActive = false en tu BD
            var expected = ReplyMessage.MESSAGE_ACTIVATE;

            var result = await context.EnableProduct(1, productId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.DisableProduct(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableProduct_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 26;
            var expected = ReplyMessage.MESSAGE_INACTIVATE;

            var result = await context.DisableProduct(1, productId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.RemoveProduct(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveProduct_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 5; // Producto dedicado para remove en tu BD
            var expected = ReplyMessage.MESSAGE_DELETE;

            var result = await context.RemoveProduct(1, productId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== GENERATE BARCODE =====================

        [Fact]
        public async Task GenerateProductBarcode_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.GenerateProductBarcode(new ProductBarcodeRequestDto()
            {
                IdProduct = 999999,
                Quantity = 1

            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GenerateProductBarcode_WhenIdExists_ReturnsProductData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 26;
            var quantity = 5;
            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.GenerateProductBarcode(new ProductBarcodeRequestDto()
            {
                IdProduct = productId,
                Quantity = quantity
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }
    }
}
