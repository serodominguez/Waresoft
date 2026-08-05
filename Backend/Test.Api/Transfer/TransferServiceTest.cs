using Application.Commons.Bases.Request;
using Application.Dtos.Request.Transfer;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.Transfer
{
    public class TransferServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TransferServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        // ===================== LIST =====================

        [Fact]
        public async Task ListTransferByStore_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.ListTransferByStore(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data!);  
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListTransferByStore_WhenFilteringByCode_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.ListTransferByStore(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "Id",
                Download = false,
                NumberFilter = 1,
                TextFilter = "TR"
            });

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("TR", x.Code!.ToUpper()));
        }

        [Fact]
        public async Task ListTransferByStore_WhenFilteringByStoreOrigin_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.ListTransferByStore(1, new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.StoreOrigin!.ToLower()));
        }

        [Fact]
        public async Task ListTransferByStore_WhenFilteringByStoreDestination_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.ListTransferByStore(1, new BaseFiltersRequest()
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
            Assert.All(result.Data!, x => Assert.Contains("a", x.StoreDestination!.ToLower()));
        }

        [Fact]
        public async Task ListTransferByStore_WhenFilteringByDateRange_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.ListTransferByStore(1, new BaseFiltersRequest()
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

        // ===================== TRANSFER BY ID =====================

        [Fact]
        public async Task TransferById_WhenIdExists_ReturnsTransfer()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var transferId = 1;

            var result = await context.TransferById(1, transferId);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(transferId, result.Data!.IdTransfer);
            Assert.NotEmpty(result.Data!.TransferDetails);
        }

        [Fact]
        public async Task TransferById_WhenIdNotExists_ReturnsQueryEmpty()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.TransferById(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task TransferById_WhenStoreDoesNotMatch_ReturnsQueryEmpty()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            // transferId=1 no pertenece ni como origen ni destino al store=999
            var result = await context.TransferById(999, 1);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== SEND =====================

        [Fact]
        public async Task SendTransfer_WhenSendingEmptyValues_ValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.SendTransfer(1, new TransferRequestDto()
            {
                IdStoreOrigin = 0,
                IdStoreDestination = 0,
                TotalAmount = 0,
                TransferDetails = new List<TransferDetailsRequestDto>()
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task SendTransfer_WhenSendingCorrectValues_SentSuccessfully()
        {
            int transferId;

            // Arrange + Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

                var result = await context.SendTransfer(1, new TransferRequestDto()
                {
                    IdStoreOrigin = 1,
                    IdStoreDestination = 2,
                    TotalAmount = 100,
                    TransferDetails = new List<TransferDetailsRequestDto>()
                    {
                        new TransferDetailsRequestDto
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

                var list = await context.ListTransferByStore(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    StateFilter = 1
                });

                transferId = list.Data!.OrderByDescending(x => x.IdTransfer).First().IdTransfer;
            }

            // Verify
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

                var sent = await context.TransferById(1, transferId);

                Assert.Equal(ReplyMessage.MESSAGE_QUERY, sent.Message);
                Assert.True(sent.IsSuccess);
                Assert.NotNull(sent.Data);
                Assert.Equal(1, sent.Data!.IdStoreOrigin);
                Assert.Equal(2, sent.Data!.IdStoreDestination);
                Assert.NotEmpty(sent.Data!.TransferDetails);
                Assert.Equal(1, sent.Data!.TransferDetails.First().IdProduct);
                Assert.Equal(1, sent.Data!.TransferDetails.First().Quantity);
            }
        }

        // ===================== RECEIVE =====================

        [Fact]
        public async Task ReceiveTransfer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.ReceiveTransfer(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task ReceiveTransfer_WhenIdExists_ReceivedSuccessfully()
        {
            int transferId;

            // Arrange - enviar un traspaso para luego recibirlo
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

                await context.SendTransfer(1, new TransferRequestDto()
                {
                    IdStoreOrigin = 1,
                    IdStoreDestination = 2,
                    TotalAmount = 50,
                    TransferDetails = new List<TransferDetailsRequestDto>()
                    {
                        new TransferDetailsRequestDto
                        {
                            Item = 1,
                            IdProduct = 1,
                            Quantity = 1,
                            UnitPrice = 50,
                            TotalPrice = 50
                        }
                    }
                });

                var list = await context.ListTransferByStore(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    StateFilter = 1
                });

                transferId = list.Data!.OrderByDescending(x => x.IdTransfer).First().IdTransfer;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

                var result = await context.ReceiveTransfer(1, transferId);

                var received = await context.TransferById(2, transferId);

                Assert.Equal(ReplyMessage.MESSAGE_SAVE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.Equal("Recibido", received.Data!.StatusTransfer);
            }
        }

        // ===================== CANCEL =====================

        [Fact]
        public async Task CancelTransfer_WhenIdNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

            var result = await context.CancelTransfer(1, 999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CancelTransfer_WhenIdExists_CancelledSuccessfully()
        {
            int transferId;

            // Arrange - enviar un traspaso para luego cancelarlo
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

                await context.SendTransfer(1, new TransferRequestDto()
                {
                    IdStoreOrigin = 1,
                    IdStoreDestination = 2,
                    TotalAmount = 50,
                    TransferDetails = new List<TransferDetailsRequestDto>()
                    {
                        new TransferDetailsRequestDto
                        {
                            Item = 1,
                            IdProduct = 1,
                            Quantity = 1,
                            UnitPrice = 50,
                            TotalPrice = 50
                        }
                    }
                });

                var list = await context.ListTransferByStore(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "Id",
                    Download = false,
                    StateFilter = 1
                });

                transferId = list.Data!.OrderByDescending(x => x.IdTransfer).First().IdTransfer;
            }

            // Act
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ITransferService>();

                var result = await context.CancelTransfer(1, transferId);

                var cancelled = await context.TransferById(1, transferId);

                Assert.Equal(ReplyMessage.MESSAGE_DELETE, result.Message);
                Assert.True(result.IsSuccess);
                Assert.Equal("Cancelado", cancelled.Data!.StatusTransfer);
            }
        }
    }
}
