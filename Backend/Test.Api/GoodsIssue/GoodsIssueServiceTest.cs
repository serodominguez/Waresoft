using Application.Commons.Bases.Request;
using Application.Dtos.Request.GoodsIssue;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.GoodsIssue
{
    public class GoodsIssueServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public GoodsIssueServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListGoodsIssueByStore_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
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
        public async Task ListGoodsIssueByStore_WhenFilteringByCode_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "GI"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("GI", x.Code!.ToUpper()));
        }

        [Fact]
        public async Task ListGoodsIssueByStore_WhenFilteringByStoreName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.StoreName!.ToLower()));
        }

        [Fact]
        public async Task ListGoodsIssueByStore_WhenFilteringByUserName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
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
        }

        [Fact]
        public async Task ListGoodsIssueByStore_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Equal("Completado", x.StatusIssue));
        }

        [Fact]
        public async Task ListGoodsIssueByStore_WhenFilteringByDateRange_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StartDate = "2024-01-01",
                EndDate = "2025-12-31"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        // ===================== GOODS ISSUE BY ID =====================

        [Fact]
        public async Task GoodsIssueById_WhenIdExists_ReturnsIssue()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var issueId = 8;

            var result = await context.GoodsIssueById(issueId, 1);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(issueId, result.Data!.IdIssue);
            Assert.NotEmpty(result.Data!.GoodsIssueDetails);
        }

        [Fact]
        public async Task GoodsIssueById_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.GoodsIssueById(999999, 1);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GoodsIssueById_WhenStoreDoesNotMatch_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            // issueId=1 pertenece a store=1, se consulta con store=999
            var result = await context.GoodsIssueById(1, 999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterGoodsIssue_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.RegisterGoodsIssue(1, 1, new GoodsIssueRequestDto()
            {
                Type = "",
                IdStore = 0,
                TotalAmount = 0,
                GoodsIssueDetails = new List<GoodsIssueDetailsRequestDto>()
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterGoodsIssue_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            int issueId;

            // Arrange + Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

                var result = await context.RegisterGoodsIssue(1, 1, new GoodsIssueRequestDto()
                {
                    Type = "Baja",
                    TotalAmount = 100,
                    IdStore = 1,
                    GoodsIssueDetails = new List<GoodsIssueDetailsRequestDto>()
                    {
                        new GoodsIssueDetailsRequestDto
                        {
                            Item = 1,
                            IdProduct = 1,
                            Quantity = 1,
                            UnitPrice = 100,
                            TotalPrice = 100
                        }
                    }
                });

                Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);

                var list = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    StateFilter = 1
                });

                issueId = list.Data!.OrderByDescending(x => x.IdIssue).First().IdIssue;
            }

            // Verify
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

                var registered = await context.GoodsIssueById(issueId, 1);

                Assert.Equal(ReplyMessage.MESSAGE_QUERY, registered.Message);
                Assert.True(registered.IsSuccess);
                Assert.NotNull(registered.Data);
                Assert.NotEmpty(registered.Data!.GoodsIssueDetails);
                Assert.Equal(1, registered.Data!.GoodsIssueDetails.First().IdProduct);
                Assert.Equal(1, registered.Data!.GoodsIssueDetails.First().Quantity);
            }
        }

        [Fact]
        public async Task RegisterGoodsIssue_WhenNoOpenPeriod_ReturnsError()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            // Se usa una tienda que no tiene período abierto
            var result = await context.RegisterGoodsIssue(1, 999, new GoodsIssueRequestDto()
            {
                Type = "Baja",
                TotalAmount = 100,
                IdStore = 999,
                GoodsIssueDetails = new List<GoodsIssueDetailsRequestDto>()
        {
            new GoodsIssueDetailsRequestDto
            {
                Item = 1,
                IdProduct = 1,
                Quantity = 1,
                UnitPrice = 100,
                TotalPrice = 100
            }
        }
            });

            Assert.False(result.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_PERIOD_NOT_FOUND, result.Message);
        }

        // ===================== CANCEL =====================

        [Fact]
        public async Task CancelGoodsIssue_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

            var result = await context.CancelGoodsIssue(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CancelGoodsIssue_WhenIdExists_CancelledSuccessfully()
        {
            int issueId;

            // Arrange - registrar una salida para cancelarla
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

                await context.RegisterGoodsIssue(1, 1, new GoodsIssueRequestDto()
                {
                    Type = "Baja",
                    TotalAmount = 50,
                    IdStore = 1,
                    GoodsIssueDetails = new List<GoodsIssueDetailsRequestDto>()
                    {
                        new GoodsIssueDetailsRequestDto
                        {
                            Item = 1,
                            IdProduct = 1,
                            Quantity = 1,
                            UnitPrice = 50,
                            TotalPrice = 50
                        }
                    }
                });

                var list = await context.ListGoodsIssueByStore(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    StateFilter = 1
                });

                issueId = list.Data!.OrderByDescending(x => x.IdIssue).First().IdIssue;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsIssueService>();

                var result = await context.CancelGoodsIssue(1, issueId);

                var cancelled = await context.GoodsIssueById(issueId, 1);

                Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.Equal("Cancelado", cancelled.Data!.StatusIssue);
            }
        }
    }
}
