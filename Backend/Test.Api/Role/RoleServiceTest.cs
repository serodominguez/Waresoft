using Application.Commons.Bases.Request;
using Application.Dtos.Request.Role;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Role
{
    public class RoleServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RoleServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterRole_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.RegisterRole(1, new RoleRequestDto()
            {
                RoleName = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterRole_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.RegisterRole(1, new RoleRequestDto()
            {
                RoleName = "Rol Test",
            });

            var list = await context.ListRoles(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "Rol Test"
            });

            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.RoleName == "Rol test");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListRoles_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.ListRoles(new BaseFiltersRequest()
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
        public async Task ListRoles_WhenFilteringByRoleName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.ListRoles(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.RoleName!.ToLower()));
        }

        [Fact]
        public async Task ListRoles_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.ListRoles(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusRole));
        }

        // ===================== SELECT LIST =====================

        [Fact]
        public async Task SelectListRoles_WhenCalled_ReturnsActiveRoles()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.SelectListRoles();

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
        }

        // ===================== ROLE BY ID =====================

        [Fact]
        public async Task RoleById_WhenIdExists_ReturnsRole()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var roleId = 1;

            var result = await context.RoleById(roleId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(roleId, result.Data!.IdRole);
        }

        [Fact]
        public async Task RoleById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.RoleById(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditRole_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.EditRole(1, 1, new RoleRequestDto()
            {
                RoleName = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditRole_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.EditRole(1, 999999, new RoleRequestDto()
            {
                RoleName = "Rol Editado",
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditRole_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            int roleId;

            // Arrange
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

                await context.RegisterRole(1, new RoleRequestDto()
                {
                    RoleName = "Rol Para Editar",
                });

                var list = await context.ListRoles(new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    NumberFilter = 1,
                    TextFilter = "Rol Para Editar"
                });

                roleId = list.Data!.First().IdRole;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

                var result = await context.EditRole(1, roleId, new RoleRequestDto()
                {
                    RoleName = "Rol Editado",
                });

                var updated = await context.RoleById(roleId);

                Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);
                Assert.Equal("Rol editado", updated.Data!.RoleName);
            }
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableRole_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.EnableRole(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableRole_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            // Arrange
            var roleId = 1;
            await context.DisableRole(1, roleId);

            // Act
            var result = await context.EnableRole(1, roleId);

            var role = await context.RoleById(roleId);

            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), role.Data!.StatusRole);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableRole_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.DisableRole(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableRole_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            // Arrange
            var roleId = 5;
            await context.EnableRole(1, roleId);

            // Act
            var result = await context.DisableRole(1, roleId);

            var role = await context.RoleById(roleId);

            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), role.Data!.StatusRole);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveRole_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var result = await context.RemoveRole(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveRole_WhenIdExists_DeletedSuccessfully()
        {
            int roleId;

            // Arrange
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

                await context.RegisterRole(1, new RoleRequestDto()
                {
                    RoleName = "Rol Para Eliminar",
                });

                var list = await context.ListRoles(new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    NumberFilter = 1,
                    TextFilter = "Rol Para Eliminar"
                });

                roleId = list.Data!.First().IdRole;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IRoleService>();

                var result = await context.RemoveRole(1, roleId);

                var deleted = await context.RoleById(roleId);

                Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);
                Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusRole);
            }
        }
    }
}
