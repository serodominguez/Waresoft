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

            var result = await context.RegisterProduct(1, new ProductRequestDto()
            {
                Description = "",
                UnitMeasure = "",
                IdBrand = 0,
                IdCategory = 0,
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterProduct_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.RegisterProduct(1, new ProductRequestDto()
            {
                Description = "Producto Test",
                Material = "Algodon",
                Color = "Rojo",
                UnitMeasure = "Unidad",
                IdBrand = 1,
                IdCategory = 1,
            });

            var list = await context.ListProducts(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 2,
                TextFilter = "Producto Test"
            });

            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.Description == "Producto test");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListProducts_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ListProducts(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("P", x.Code!.ToUpper()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.Description!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.Material!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.Color!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.BrandName!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.CategoryName!.ToLower()));
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
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusProduct));
        }

        // ===================== PRODUCT BY ID =====================

        [Fact]
        public async Task ProductById_WhenIdExists_ReturnsProduct()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 1;

            var result = await context.ProductById(productId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(productId, result.Data!.IdProduct);
        }

        [Fact]
        public async Task ProductById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.ProductById(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditProduct_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.EditProduct(1, 1, new ProductRequestDto()
            {
                Description = "",
                UnitMeasure = "",
                IdBrand = 0,
                IdCategory = 0,
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.EditProduct(1, 999999, new ProductRequestDto()
            {
                Description = "Producto Editado",
                UnitMeasure = "Unidad",
                IdBrand = 1,
                IdCategory = 1,
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditProduct_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Arrange
            await context.RegisterProduct(1, new ProductRequestDto()
            {
                Description = "Producto Para Editar",
                Material = "Algodon",
                Color = "Azul",
                UnitMeasure = "Unidad",
                IdBrand = 1,
                IdCategory = 1,
            });

            var list = await context.ListProducts(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 2,
                TextFilter = "Producto Para Editar"
            });
            var productId = list.Data!.First().IdProduct;

            // Act
            var result = await context.EditProduct(1, productId, new ProductRequestDto()
            {
                Description = "Producto Editado",
                Material = "Poliester",
                Color = "Verde",
                UnitMeasure = "Unidad",
                IdBrand = 1,
                IdCategory = 1,
            });

            var updated = await context.ProductById(productId);

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Producto editado", updated.Data!.Description);
            Assert.Equal("Poliester", updated.Data!.Material);
            Assert.Equal("Verde", updated.Data!.Color);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.EnableProduct(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableProduct_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Arrange
            var productId = 2;
            await context.DisableProduct(1, productId);

            // Act
            var result = await context.EnableProduct(1, productId);

            var product = await context.ProductById(productId);

            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), product.Data!.StatusProduct);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.DisableProduct(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableProduct_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Arrange
            var productId = 2;
            await context.EnableProduct(1, productId);

            // Act
            var result = await context.DisableProduct(1, productId);

            var product = await context.ProductById(productId);

            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), product.Data!.StatusProduct);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveProduct_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.RemoveProduct(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveProduct_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Arrange
            await context.RegisterProduct(1, new ProductRequestDto()
            {
                Description = "Producto Para Eliminar",
                Material = "Algodon",
                Color = "Negro",
                UnitMeasure = "Unidad",
                IdBrand = 1,
                IdCategory = 1,
            });

            var list = await context.ListProducts(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 2,
                TextFilter = "Producto Para Eliminar"
            });
            var productId = list.Data!.First().IdProduct;

            // Act
            var result = await context.RemoveProduct(1, productId);

            var deleted = await context.ProductById(productId);

            Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusProduct);
        }

        // ===================== GENERATE BARCODE =====================

        [Fact]
        public async Task GenerateProductBarcode_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var result = await context.GenerateProductBarcode(new ProductBarcodeRequestDto()
            {
                IdProduct = 999999,
                Quantity = 1
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GenerateProductBarcode_WhenIdExists_ReturnsProductData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IProductService>();

            var productId = 1;
            var quantity = 5;

            var result = await context.GenerateProductBarcode(new ProductBarcodeRequestDto()
            {
                IdProduct = productId,
                Quantity = quantity
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data);
        }
    }
}
