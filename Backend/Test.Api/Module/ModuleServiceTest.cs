using Application.Commons.Bases.Request;
using Application.Dtos.Request.Module;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Module
{
    public class ModuleServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ModuleServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterModule_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.RegisterModule(1, new ModuleRequestDto()
            {
                ModuleName = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterModule_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.RegisterModule(1, new ModuleRequestDto()
            {
                ModuleName = "Modulo Test",
            });

            var list = await context.ListModules(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Modulo Test"
            });

            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.ModuleName == "Modulo Test");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListModules_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.ListModules(new BaseFiltersRequest()
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
        public async Task ListModules_WhenFilteringByModuleName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.ListModules(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.ModuleName!.ToLower()));
        }

        [Fact]
        public async Task ListModules_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.ListModules(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusModule));
        }

        // ===================== MODULE BY ID =====================

        [Fact]
        public async Task ModuleById_WhenIdExists_ReturnsModule()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var moduleId = 1;

            var result = await context.ModuleById(moduleId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(moduleId, result.Data!.IdModule);
        }

        [Fact]
        public async Task ModuleById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.ModuleById(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditModule_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.EditModule(1, 1, new ModuleRequestDto()
            {
                ModuleName = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditModule_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.EditModule(1, 999999, new ModuleRequestDto()
            {
                ModuleName = "Modulo Editado",
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditModule_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            int moduleId;

            // Arrange
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

                await context.RegisterModule(1, new ModuleRequestDto()
                {
                    ModuleName = "Modulo Para Editar",
                });

                var list = await context.ListModules(new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    NumberFilter = 1,
                    TextFilter = "Modulo Para Editar"
                });

                moduleId = list.Data!.First().IdModule;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

                var result = await context.EditModule(1, moduleId, new ModuleRequestDto()
                {
                    ModuleName = "Modulo Editado",
                });

                var updated = await context.ModuleById(moduleId);

                Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);
                Assert.Equal("Modulo Editado", updated.Data!.ModuleName);
            }
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableModule_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.EnableModule(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableModule_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            // Arrange
            var moduleId = 1;
            await context.DisableModule(1, moduleId);

            // Act
            var result = await context.EnableModule(1, moduleId);

            var module = await context.ModuleById(moduleId);

            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), module.Data!.StatusModule);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableModule_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.DisableModule(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableModule_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            // Arrange
            var moduleId = 1;
            await context.EnableModule(1, moduleId);

            // Act
            var result = await context.DisableModule(1, moduleId);

            var module = await context.ModuleById(moduleId);

            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), module.Data!.StatusModule);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveModule_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

            var result = await context.RemoveModule(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveModule_WhenIdExists_DeletedSuccessfully()
        {
            int moduleId;

            // Arrange
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

                await context.RegisterModule(1, new ModuleRequestDto()
                {
                    ModuleName = "Modulo Para Eliminar",
                });

                var list = await context.ListModules(new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    NumberFilter = 1,
                    TextFilter = "Modulo Para Eliminar"
                });

                moduleId = list.Data!.First().IdModule;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IModuleService>();

                var result = await context.RemoveModule(1, moduleId);

                var deleted = await context.ModuleById(moduleId);

                Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);
                Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusModule);
            }
        }
    }
}
