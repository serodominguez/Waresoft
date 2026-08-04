using Application.Dtos.Request.Permission;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Permissions
{
    public class PermissionServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PermissionServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== USER PERMISSIONS =====================

        [Fact]
        public async Task UserPermissions_WhenUserExistsAndHasPermission_ReturnsTrue()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.UserPermissions(1, "Permisos", "Leer");

            Assert.True(result);
        }

        [Fact]
        public async Task UserPermissions_WhenUserNotExists_ReturnsFalse()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.UserPermissions(999999, "Categorias", "Listar");

            Assert.False(result);
        }

        [Fact]
        public async Task UserPermissions_WhenUserExistsButNoPermission_ReturnsFalse()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.UserPermissions(1, "ModuloInexistente", "AccionInexistente");

            Assert.False(result);
        }

        // ===================== LIST USER PERMISSIONS =====================

        [Fact]
        public async Task ListUserPermissions_WhenUserExists_ReturnsPermissions()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.ListUserPermissions(1);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
            Assert.All(result.Data!, x =>
            {
                Assert.False(string.IsNullOrEmpty(x.Module));
                Assert.False(string.IsNullOrEmpty(x.Action));
            });
        }

        [Fact]
        public async Task ListUserPermissions_WhenUserNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.ListUserPermissions(999999);

            Assert.Equal(ReplyMessage.MESSAGE_USER_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== PERMISSIONS BY ROLE =====================

        [Fact]
        public async Task PermissionsByRole_WhenRoleExists_ReturnsPermissions()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var roleId = 1;

            var result = await context.PermissionsByRole(roleId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
            Assert.All(result.Data!, x =>
            {
                Assert.Equal(roleId, x.IdRole);
                Assert.True(x.IdPermission > 0);
                Assert.True(x.IdModule > 0);
                Assert.True(x.IdAction > 0);
                Assert.False(string.IsNullOrEmpty(x.ModuleName));
                Assert.False(string.IsNullOrEmpty(x.ActionName));
            });
        }

        [Fact]
        public async Task PermissionsByRole_WhenRoleNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.PermissionsByRole(999999);

            Assert.Equal(ReplyMessage.MESSAGE_USER_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== UPDATE PERMISSIONS =====================

        [Fact]
        public async Task UpdatePermissions_WhenSendingEmptyList_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.UpdatePermissions(7, new List<PermissionRequestDto>());

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdatePermissions_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var result = await context.UpdatePermissions(7, new List<PermissionRequestDto>()
            {
                new PermissionRequestDto { IdPermission = 999999, Status = true }
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdatePermissions_WhenStatusUnchanged_ReturnsQueryEmpty()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            // Arrange - leer el estado actual del permiso 7
            var current = (await context.PermissionsByRole(1))
                .Data!.First(x => x.IdPermission == 7);

            // Act - enviar el mismo estado que ya tiene
            var result = await context.UpdatePermissions(7, new List<PermissionRequestDto>()
            {
                new PermissionRequestDto { IdPermission = 7, Status = current.Status }
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, result.Message);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task UpdatePermissions_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            // Arrange - leer estado actual para poder invertirlo y restaurarlo
            var current = (await context.PermissionsByRole(1))
                .Data!.First(x => x.IdPermission == 7);

            var newStatus = !current.Status;

            // Act
            var result = await context.UpdatePermissions(7, new List<PermissionRequestDto>()
            {
                new PermissionRequestDto { IdPermission = 7, Status = newStatus }
            });

            var updated = (await context.PermissionsByRole(1))
                .Data!.First(x => x.IdPermission == 7);

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(newStatus, updated.Status);

            // Teardown - restaurar estado original
            await context.UpdatePermissions(7, new List<PermissionRequestDto>()
            {
                new PermissionRequestDto { IdPermission = 7, Status = current.Status }
            });
        }
    }
}
