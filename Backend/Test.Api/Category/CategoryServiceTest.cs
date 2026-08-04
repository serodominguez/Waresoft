using Application.Commons.Bases.Request;
using Application.Dtos.Request.Category;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Category
{
    public class CategoryServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CategoryServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterCategory_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.RegisterCategory(1, new CategoryRequestDto()
            {
                CategoryName = "",
                Description = "",
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterCategory_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.RegisterCategory(1, new CategoryRequestDto()
            {
                CategoryName = "Categoria Test",
                Description = "Descripcion Test",
            });

            var list = await context.ListCategories(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Categoria Test"
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.CategoryName == "Categoria test");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListCategories_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.ListCategories(new BaseFiltersRequest()
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
                TextFilter = "Categoria"
            });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("categoria", x.CategoryName!.ToLower()));
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

            // Act
            var result = await context.SelectListCategories();

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
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

            // Act
            var result = await context.CategoryById(categoryId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(categoryId, result.Data!.IdCategory);
        }

        [Fact]
        public async Task CategoryById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.CategoryById(999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditCategory_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.EditCategory(1, 1, new CategoryRequestDto()
            {
                CategoryName = "",
                Description = "",
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.EditCategory(1, 999999, new CategoryRequestDto()
            {
                CategoryName = "Nombre editado",
                Description = "Descripcion editada",
            });

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditCategory_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange — creamos una categoría para editar
            await context.RegisterCategory(1, new CategoryRequestDto()
            {
                CategoryName = "Categoria Para Editar",
                Description = "Descripcion original",
            });

            var list = await context.ListCategories(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Categoria Para Editar"
            });
            var categoryId = list.Data!.First().IdCategory;

            // Act
            var result = await context.EditCategory(1, categoryId, new CategoryRequestDto()
            {
                CategoryName = "Categoria Editada",
                Description = "Descripcion editada",
            });

            var updated = await context.CategoryById(categoryId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Categoria editada", updated.Data!.CategoryName);
            Assert.Equal("Descripcion editada", updated.Data!.Description);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.EnableCategory(1, 999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableCategory_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            var categoryId = 1;
            await context.DisableCategory(1, categoryId);

            // Act
            var result = await context.EnableCategory(1, categoryId);

            var category = await context.CategoryById(categoryId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), category.Data!.StatusCategory);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.DisableCategory(1, 999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableCategory_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            var categoryId = 1;
            await context.EnableCategory(1, categoryId);

            // Act
            var result = await context.DisableCategory(1, categoryId);

            var category = await context.CategoryById(categoryId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), category.Data!.StatusCategory);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveCategory_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Act
            var result = await context.RemoveCategory(1, 999999);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveCategory_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            // Arrange
            await context.RegisterCategory(1, new CategoryRequestDto()
            {
                CategoryName = "Categoria Para Eliminar",
                Description = "Descripcion para eliminar",
            });

            var list = await context.ListCategories(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Categoria Para Eliminar"
            });
            var categoryId = list.Data!.First().IdCategory;

            // Act
            var result = await context.RemoveCategory(1, categoryId);

            var deleted = await context.CategoryById(categoryId);

            // Assert
            Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusCategory);
        }
    }
}
