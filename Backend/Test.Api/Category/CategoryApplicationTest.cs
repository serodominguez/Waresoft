using Application.Commons.Bases.Request;
using Application.Dtos.Request.Category;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Category
{
    public class CategoryApplicationTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CategoryApplicationTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterCategory_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_VALIDATE;

            // Act
            var result = await context.RegisterCategory(1, new CategoryRequestDto()
            {
                CategoryName = "",
                Description = "",
            });

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterCategory_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_SAVE;

            // Act
            var result = await context.RegisterCategory(1, new CategoryRequestDto()
            {
                CategoryName = "Categoria Test",
                Description = "Descripcion Test",
            });

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListCategories_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_QUERY;

            // Act
            var result = await context.ListCategories(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false
            });

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListCategories_WhenFilteringByName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.ListCategories(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "montura"
            });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListCategories_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.ListCategories(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusCategory));
        }

        // ===================== SELECT LIST =====================

        [Fact]
        public async Task SelectListCategories_WhenCalled_ReturnsActiveCategories()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_QUERY;

            // Act
            var result = await context.SelectListCategories();

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
        }

        // ===================== CATEGORY BY ID =====================

        [Fact]
        public async Task CategoryById_WhenIdExists_ReturnsCategory()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var categoryId = 1;
            var expected = ReplyMessage.MESSAGE_QUERY;

            // Act
            var result = await context.CategoryById(categoryId);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(categoryId, result.Data!.IdCategory);
        }

        [Fact]
        public async Task CategoryById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            // Act
            var result = await context.CategoryById(999999);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditCategory_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_VALIDATE;

            // Act
            var result = await context.EditCategory(1, 1, new CategoryRequestDto()
            {
                CategoryName = "",
                Description = "",
            });

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            // Act
            var result = await context.EditCategory(1, 999999, new CategoryRequestDto()
            {
                CategoryName = "Nombre editado",
                Description = "Descripcion editada",
            });

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditCategory_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var categoryId = 1;
            var expected = ReplyMessage.MESSAGE_UPDATE;

            // Act
            var result = await context.EditCategory(1, categoryId, new CategoryRequestDto()
            {
                CategoryName = "Montura Adulto",
                Description = "Monturas para adultos",
            });

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            // Act
            var result = await context.EnableCategory(1, 999999);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableCategory_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var categoryId = 5; // pinia store — IsActive = 0 en tu BD
            var expected = ReplyMessage.MESSAGE_ACTIVATE;

            // Act
            var result = await context.EnableCategory(1, categoryId);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            // Act
            var result = await context.DisableCategory(1, 999999);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableCategory_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var categoryId = 1; // montura adulto — IsActive = 1 en tu BD
            var expected = ReplyMessage.MESSAGE_INACTIVATE;

            // Act
            var result = await context.DisableCategory(1, categoryId);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            // Act
            var result = await context.RemoveCategory(1, 999999);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveCategory_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var categoryId = 14; // prueba hoy — el que registraste manualmente
            var expected = ReplyMessage.MESSAGE_DELETE;

            // Act
            var result = await context.RemoveCategory(1, categoryId);

            // Assert
            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }
    }
}
