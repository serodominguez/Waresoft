using Application.Commons.Bases.Request;
using Application.Dtos.Request.GoodsReceipt;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.GoodsReceipt
{
    public class GoodsReceiptServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public GoodsReceiptServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListGoodsReceiptByStore_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
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
        public async Task ListGoodsReceiptByStore_WhenFilteringByCode_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "GR"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("GR", x.Code!.ToUpper()));
        }

        [Fact]
        public async Task ListGoodsReceiptByStore_WhenFilteringByStoreName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
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
        public async Task ListGoodsReceiptByStore_WhenFilteringByCompanyName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.CompanyName!.ToLower()));
        }

        [Fact]
        public async Task ListGoodsReceiptByStore_WhenFilteringByState_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                StateFilter = 1
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Equal("Completado", x.StatusReceipt));
        }

        [Fact]
        public async Task ListGoodsReceiptByStore_WhenFilteringByDateRange_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
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

        // ===================== GOODS RECEIPT BY ID =====================

        [Fact]
        public async Task GoodsReceiptById_WhenIdExists_ReturnsReceipt()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var receiptId = 1;

            var result = await context.GoodsReceiptById(receiptId, 1);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(receiptId, result.Data!.IdReceipt);
            Assert.NotEmpty(result.Data!.GoodsReceiptDetails);
        }

        [Fact]
        public async Task GoodsReceiptById_WhenIdNotExists_ReturnsQueryEmpty()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.GoodsReceiptById(999999, 1);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GoodsReceiptById_WhenStoreDoesNotMatch_ReturnsQueryEmpty()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            // receiptId=1 pertenece a store=1, se consulta con store=999
            var result = await context.GoodsReceiptById(1, 999);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== REGISTER =====================

        [Fact]
        public async Task RegisterGoodsReceipt_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.RegisterGoodsReceipt(1, 1, new GoodsReceiptRequestDto()
            {
                Type = "",
                IdStore = 0,
                TotalAmount = 0,
                GoodsReceiptDetails = new List<GoodsReceiptDetailsRequestDto>()
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task RegisterGoodsReceipt_WhenSendingCorrectValues_RegisteredSuccessfully()
        {
            int receiptId;

            // Arrange + Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

                var result = await context.RegisterGoodsReceipt(1, 1, new GoodsReceiptRequestDto()
                {
                    Type = "Adquisición",
                    DocumentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    DocumentType = "Factura",
                    DocumentNumber = "001-001-00000001",
                    TotalAmount = 100,
                    IdSupplier = 1,
                    IdStore = 1,
                    GoodsReceiptDetails = new List<GoodsReceiptDetailsRequestDto>()
                    {
                        new GoodsReceiptDetailsRequestDto
                        {
                            Item = 1,
                            IdProduct = 1,
                            Quantity = 10,
                            UnitCost = 10,
                            TotalCost = 100
                        }
                    }
                });

                Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);

                var list = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    StateFilter = 1
                });

                receiptId = list.Data!.OrderByDescending(x => x.IdReceipt).First().IdReceipt;
            }

            // Verify
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

                var registered = await context.GoodsReceiptById(receiptId, 1);

                Assert.Equal(ReplyMessage.MESSAGE_QUERY, registered.Message);
                Assert.True(registered.IsSuccess);
                Assert.NotNull(registered.Data);
                Assert.NotEmpty(registered.Data!.GoodsReceiptDetails);
                Assert.Equal(1, registered.Data!.GoodsReceiptDetails.First().IdProduct);
                Assert.Equal(10, registered.Data!.GoodsReceiptDetails.First().Quantity);
            }
        }

        // ===================== CANCEL =====================

        [Fact]
        public async Task CancelGoodsReceipt_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

            var result = await context.CancelGoodsReceipt(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CancelGoodsReceipt_WhenIdExists_CancelledSuccessfully()
        {
            int receiptId;

            // Arrange - registrar un receipt para cancelarlo
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

                await context.RegisterGoodsReceipt(1, 1, new GoodsReceiptRequestDto()
                {
                    Type = "adquisicion",
                    DocumentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    DocumentType = "Factura",
                    DocumentNumber = "001-001-00000099",
                    TotalAmount = 50,
                    IdSupplier = 1,
                    IdStore = 1,
                    GoodsReceiptDetails = new List<GoodsReceiptDetailsRequestDto>()
                    {
                        new GoodsReceiptDetailsRequestDto
                        {
                            Item = 1,
                            IdProduct = 1,
                            Quantity = 5,
                            UnitCost = 10,
                            TotalCost = 50
                        }
                    }
                });

                var list = await context.ListGoodsReceiptByStore(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    StateFilter = 1
                });

                receiptId = list.Data!.OrderByDescending(x => x.IdReceipt).First().IdReceipt;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IGoodsReceiptService>();

                var result = await context.CancelGoodsReceipt(1, receiptId);

                var cancelled = await context.GoodsReceiptById(receiptId, 1);

                Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.Equal("Cancelado", cancelled.Data!.StatusReceipt);
            }
        }
    }
}
