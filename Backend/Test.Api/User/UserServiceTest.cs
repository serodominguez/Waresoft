using Application.Commons.Bases.Request;
using Application.Dtos.Request.User;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.User
{
    public class UserServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UserServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterUser_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.RegisterUser(1, new UserRequestDto()
            {
                UserName = "",
                Names = "",
                LastNames = "",
                Password = "",
                IdRole = 0,
                IdStore = 0,
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterUser_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.RegisterUser(1, new UserRequestDto()
            {
                UserName = "usuarioTest",
                Names = "Nombre Test",
                LastNames = "Apellido Test",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
            });

            var list = await context.ListUsers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "usuarioTest"
            });

            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.UserName == "usuarioTest");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListUsers_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.ListUsers(new BaseFiltersRequest()
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
        public async Task ListUsers_WhenFilteringByUserName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.ListUsers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "admin"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("admin", x.UserName!.ToLower()));
        }

        [Fact]
        public async Task ListUsers_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.ListUsers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusUser));
        }

        // ===================== SELECT LIST =====================

        [Fact]
        public async Task SelectListUsers_WhenCalled_ReturnsActiveUsers()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.SelectListUsers();

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.Data!.Any());
        }

        // ===================== USER BY ID =====================

        [Fact]
        public async Task UserById_WhenIdExists_ReturnsUser()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var userId = 1;

            var result = await context.UserById(userId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data!.IdUser);
        }

        [Fact]
        public async Task UserById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.UserById(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditUser_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.EditUser(1, 1, new UserRequestDto()
            {
                UserName = "",
                Names = "",
                LastNames = "",
                Password = "",
                IdRole = 0,
                IdStore = 0,
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.EditUser(1, 999999, new UserRequestDto()
            {
                UserName = "usuarioEditado",
                Names = "Nombre Editado",
                LastNames = "Apellido Editado",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditUser_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            // Arrange
            await context.RegisterUser(1, new UserRequestDto()
            {
                UserName = "usuarioParaEditar",
                Names = "Nombre Original",
                LastNames = "Apellido Original",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
            });

            var list = await context.ListUsers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "usuarioParaEditar"
            });
            var userId = list.Data!.First().IdUser;

            // Act
            var result = await context.EditUser(1, userId, new UserRequestDto()
            {
                UserName = "usuarioEditado",
                Names = "Nombre Editado",
                LastNames = "Apellido Editado",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
                UpdatePassword = false,
            });

            var updated = await context.UserById(userId);

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Nombre Editado", updated.Data!.Names);
            Assert.Equal("Apellido Editado", updated.Data!.LastNames);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.EnableUser(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableUser_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            // Arrange
            var userId = 5;
            await context.DisableUser(1, userId);

            // Act
            var result = await context.EnableUser(1, userId);

            var user = await context.UserById(userId);

            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), user.Data!.StatusUser);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.DisableUser(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableUser_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            // Arrange
            var userId = 5;
            await context.EnableUser(1, userId);

            // Act
            var result = await context.DisableUser(1, userId);

            var user = await context.UserById(userId);

            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), user.Data!.StatusUser);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var result = await context.RemoveUser(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveUser_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            // Arrange
            await context.RegisterUser(1, new UserRequestDto()
            {
                UserName = "usuarioParaEliminar",
                Names = "Nombre Eliminar",
                LastNames = "Apellido Eliminar",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
            });

            var list = await context.ListUsers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "usuarioParaEliminar"
            });
            var userId = list.Data!.First().IdUser;

            // Act
            var result = await context.RemoveUser(1, userId);

            var deleted = await context.UserById(userId);

            Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusUser);
        }
    }
}
