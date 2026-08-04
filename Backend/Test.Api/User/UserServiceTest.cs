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

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.RegisterUser(1, new UserRequestDto()
            {
                UserName = "",
                Names = "",
                LastNames = "",
                Password = "",
                IdRole = 0,
                IdStore = 0,
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterUser_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_SAVE;

            var result = await context.RegisterUser(1, new UserRequestDto()
            {
                UserName = "usuarioTest",
                Names = "Nombre Test",
                LastNames = "Apellido Test",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListUsers_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.ListUsers(new BaseFiltersRequest()
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

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.SelectListUsers();

            Assert.Equal(expected, result.Message);
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
            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.UserById(userId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data!.IdUser);
        }

        [Fact]
        public async Task UserById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.UserById(999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditUser_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.EditUser(1, 1, new UserRequestDto()
            {
                UserName = "",
                Names = "",
                LastNames = "",
                Password = "",
                IdRole = 0,
                IdStore = 0,
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EditUser(1, 999999, new UserRequestDto()
            {
                UserName = "usuarioEditado",
                Names = "Nombre Editado",
                LastNames = "Apellido Editado",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditUser_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var userId = 1;
            var expected = ReplyMessage.MESSAGE_UPDATE;

            var result = await context.EditUser(1, userId, new UserRequestDto()
            {
                UserName = "admin",
                Names = "Administrador",
                LastNames = "Sistema",
                Password = "Password123!",
                IdRole = 1,
                IdStore = 1,
                UpdatePassword = false,
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EnableUser(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableUser_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var userId = 5; // usuario con IsActive = false en tu BD
            var expected = ReplyMessage.MESSAGE_ACTIVATE;

            var result = await context.EnableUser(1, userId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.DisableUser(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableUser_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var userId = 1; // usuario activo en tu BD
            var expected = ReplyMessage.MESSAGE_INACTIVATE;

            var result = await context.DisableUser(1, userId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveUser_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.RemoveUser(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveUser_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IUserService>();

            var userId = 2;
            var expected = ReplyMessage.MESSAGE_DELETE;

            var result = await context.RemoveUser(1, userId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }
    }
}
