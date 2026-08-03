using Application.Commons.Bases.Request;
using Application.Dtos.Request.Customer;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.RegisterCustomer(1, new CustomerRequestDto()
            {
                Names = "",
                LastNames = "",
                IdentificationNumber = "",
                PhoneNumber = "",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterCustomer_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_SAVE;

            var result = await context.RegisterCustomer(1, new CustomerRequestDto()
            {
                Names = "Juan",
                LastNames = "Perez",
                IdentificationNumber = "12345678",
                PhoneNumber = "77712345",
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListCustomers_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.ListCustomers(new BaseFiltersRequest()
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
        }

        // ===================== CUSTOMER BY ID =====================

        [Fact]
        public async Task CustomerById_WhenIdExists_ReturnsCustomer()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var customerId = 1; // ID real en tu BD
            var expected = ReplyMessage.MESSAGE_QUERY;

            var result = await context.CustomerById(customerId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task CustomerById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.CustomerById(999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== EDIT =====================

        [Fact]
        public async Task EditCustomer_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_VALIDATE;

            var result = await context.EditCustomer(1, 1, new CustomerRequestDto()
            {
                Names = "",
                LastNames = "",
                IdentificationNumber = "",
                PhoneNumber = "",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task EditCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EditCustomer(1, 999999, new CustomerRequestDto()
            {
                Names = "Juan",
                LastNames = "Perez",
                IdentificationNumber = "12345678",
                PhoneNumber = "77712345",
            });

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EditCustomer_WhenSendingCorrectValues_UpdatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var customerId = 1; // ID real en tu BD
            var expected = ReplyMessage.MESSAGE_UPDATE;

            var result = await context.EditCustomer(1, customerId, new CustomerRequestDto()
            {
                Names = "Juan",
                LastNames = "Perez",
                IdentificationNumber = "12345678",
                PhoneNumber = "77712345",
            });

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== ENABLE =====================

        [Fact]
        public async Task EnableCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.EnableCustomer(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EnableCustomer_WhenIdExists_ActivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var customerId = 2; // Customer con IsActive = 0 en tu BD
            var expected = ReplyMessage.MESSAGE_ACTIVATE;

            var result = await context.EnableCustomer(1, customerId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== DISABLE =====================

        [Fact]
        public async Task DisableCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.DisableCustomer(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DisableCustomer_WhenIdExists_DeactivatedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var customerId = 1; // Customer con IsActive = 1 en tu BD
            var expected = ReplyMessage.MESSAGE_INACTIVATE;

            var result = await context.DisableCustomer(1, customerId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ===================== REMOVE =====================

        [Fact]
        public async Task RemoveCustomer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var expected = ReplyMessage.MESSAGE_NOT_FOUND;

            var result = await context.RemoveCustomer(1, 999999);

            Assert.Equal(expected, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveCustomer_WhenIdExists_DeletedSuccessfully()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ICustomerService>();

            var customerId = 5; // Customer dedicado para remove en tu BD
            var expected = ReplyMessage.MESSAGE_DELETE;

            var result = await context.RemoveCustomer(1, customerId);

            Assert.Equal(expected, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }
    }
}
