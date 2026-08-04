using Application.Commons.Bases.Request;
using Application.Dtos.Request.Customer;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Customer
{
    public class CustomerServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CustomerServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterCustomer_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.RegisterCustomer(1, new CustomerRequestDto()
            {
                Names = "",
                LastNames = "",
                IdentificationNumber = "",
                PhoneNumber = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterCustomer_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.RegisterCustomer(1, new CustomerRequestDto()
            {
                Names = "Juan",
                LastNames = "Perez",
                IdentificationNumber = "12345678",
                PhoneNumber = "77712345",
            });

            var list = await context.ListCustomers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 3,
                TextFilter = "12345678"
            });

            Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Contains(list.Data!, x => x.IdentificationNumber == "12345678");
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListCustomers_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.ListCustomers(new BaseFiltersRequest()
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
        public async Task ListCustomers_WhenFilteringByNames_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.ListCustomers(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.Names!.ToLower()));
        }

        [Fact]
        public async Task ListCustomers_WhenFilteringByLastNames_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.ListCustomers(new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.LastNames!.ToLower()));
        }

        [Fact]
        public async Task ListCustomers_WhenFilteringByIdentificationNumber_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.ListCustomers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 3,
                TextFilter = "1"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("1", x.IdentificationNumber!));
        }

        [Fact]
        public async Task ListCustomers_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.ListCustomers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Equal(States.Activo.ToString(), x.StatusCustomer));
        }

        // ===================== CUSTOMER BY ID =====================

        [Fact]
        public async Task CustomerById_WhenIdExists_ReturnsCustomer()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var customerId = 1;

            var result = await context.CustomerById(customerId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(customerId, result.Data!.IdCustomer);
        }

        [Fact]
        public async Task CustomerById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.CustomerById(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditCustomer_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.EditCustomer(1, 1, new CustomerRequestDto()
            {
                Names = "",
                LastNames = "",
                IdentificationNumber = "",
                PhoneNumber = "",
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.EditCustomer(1, 999999, new CustomerRequestDto()
            {
                Names = "Juan",
                LastNames = "Perez",
                IdentificationNumber = "12345678",
                PhoneNumber = "77712345",
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditCustomer_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            // Arrange
            await context.RegisterCustomer(1, new CustomerRequestDto()
            {
                Names = "Cliente Para Editar",
                LastNames = "Apellido Original",
                IdentificationNumber = "99991111",
                PhoneNumber = "77700001",
            });

            var list = await context.ListCustomers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 3,
                TextFilter = "99991111"
            });
            var customerId = list.Data!.First().IdCustomer;

            // Act
            var result = await context.EditCustomer(1, customerId, new CustomerRequestDto()
            {
                Names = "Cliente Editado",
                LastNames = "Apellido Editado",
                IdentificationNumber = "99991111",
                PhoneNumber = "77700002",
            });

            var updated = await context.CustomerById(customerId);

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal("Cliente Editado", updated.Data!.Names);
            Assert.Equal("Apellido Editado", updated.Data!.LastNames);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.EnableCustomer(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableCustomer_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            // Arrange
            var customerId = 1;
            await context.DisableCustomer(1, customerId);

            // Act
            var result = await context.EnableCustomer(1, customerId);

            var customer = await context.CustomerById(customerId);

            Assert.Equal(ReplyMessage.MESSAGE_ACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Activo.ToString(), customer.Data!.StatusCustomer);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.DisableCustomer(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableCustomer_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            // Arrange
            var customerId = 1;
            await context.EnableCustomer(1, customerId);

            // Act
            var result = await context.DisableCustomer(1, customerId);

            var customer = await context.CustomerById(customerId);

            Assert.Equal(ReplyMessage.MESSAGE_INACTIVATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), customer.Data!.StatusCustomer);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var result = await context.RemoveCustomer(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveCustomer_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            // Arrange
            await context.RegisterCustomer(1, new CustomerRequestDto()
            {
                Names = "Cliente Para Eliminar",
                LastNames = "Apellido Eliminar",
                IdentificationNumber = "99999999",
                PhoneNumber = "77799999",
            });

            var list = await context.ListCustomers(new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 100,
                Sort = "Id",
                Download = false,
                NumberFilter = 3,
                TextFilter = "99999999"
            });
            var customerId = list.Data!.First().IdCustomer;

            // Act
            var result = await context.RemoveCustomer(1, customerId);

            var deleted = await context.CustomerById(customerId);

            Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(States.Inactivo.ToString(), deleted.Data!.StatusCustomer);
        }
    }
}
