using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.InventoryPeriod;
using Application.Dtos.Response.InventoryPeriod;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Static;

namespace Test.Api.InventoryPeriod
{
    public class InventoryPeriodServiceTest : IClassFixture<ApiFactory>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public InventoryPeriodServiceTest(ApiFactory factory)
        {
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }
        // ===================== LIST PERIODS =====================

        [Fact]
        public async Task ListPeriods_WhenCalled_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.ListPeriods(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "IdPeriod",
                Download = false
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task ListPeriods_WhenFilteringByName_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.ListPeriods(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "IdPeriod",
                Download = false,
                TextFilter = "Agosto"
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.All(result.Data!, x => Assert.Contains("Agosto", x.PeriodName));
        }

        [Fact]
        public async Task ListPeriods_WhenFilteringByDateRange_ReturnsFilteredData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.ListPeriods(1, new BaseFiltersRequest()
            {
                NumberPage = 1,
                NumberRecordsPage = 10,
                Sort = "IdPeriod",
                Download = false,
                StartDate = "2026-08-01",
                EndDate = "2026-08-31"
            });

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        // ===================== GET PERIOD DETAIL =====================

        [Fact]
        public async Task GetPeriodDetail_WhenPeriodExists_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.GetPeriodDetail(1);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data!.IdPeriod);
        }

        [Fact]
        public async Task GetPeriodDetail_WhenPeriodNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.GetPeriodDetail(999999);

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
        }

        // ===================== GET CLOSING BY PERIOD =====================

        [Fact]
        public async Task GetClosingByPeriod_WhenPeriodHasClosing_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            // IdPeriod=1 está CLOSED y tiene datos de cierre
            var result = await context.GetClosingByPeriod(1);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task GetClosingByPeriod_WhenPeriodNotExists_ReturnsEmptyList()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.GetClosingByPeriod(999999);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data!);
        }

        // ===================== GET OPENING BY PERIOD =====================

        [Fact]
        public async Task GetOpeningByPeriod_WhenPeriodHasOpening_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            // IdPeriod=2 está OPEN y tiene datos de apertura
            var result = await context.GetOpeningByPeriod(2);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.True(result.TotalRecords > 0);
        }

        [Fact]
        public async Task GetOpeningByPeriod_WhenPeriodNotExists_ReturnsEmptyList()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.GetOpeningByPeriod(999999);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data!);
        }

        // ===================== GET SYSTEM STOCK CALCULATED =====================

        [Fact]
        public async Task GetSystemStockCalculated_WhenPeriodExists_ReturnsData()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            // IdPeriod=2 está OPEN — tiene movimientos calculados
            var result = await context.GetSystemStockCalculated(2);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetSystemStockCalculated_WhenPeriodNotExists_ReturnsEmptyList()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.GetSystemStockCalculated(999999);

            Assert.Equal(ReplyMessage.MESSAGE_QUERY, result.Message);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data!);
        }

        // ===================== OPEN PERIOD =====================

        [Fact]
        public async Task OpenPeriod_WhenSendingEmptyValues_ReturnsValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.OpenPeriod(1, 1, new InventoryPeriodOpenRequestDto()
            {
                PeriodName = "",
                StartDate = default,
                EndDate = default
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task OpenPeriod_WhenEndDateNotGreaterThanStartDate_ReturnsValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.OpenPeriod(1, 1, new InventoryPeriodOpenRequestDto()
            {
                PeriodName = "Periodo Test",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(-1)
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task OpenPeriod_WhenAlreadyOpenPeriodExists_ReturnsFail()
        {
            // Arrange — verificar si existe un período OPEN
            BaseResponse<IEnumerable<InventoryPeriodResponseDto>> list;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                list = await context.ListPeriods(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "IdPeriod",
                    Download = false
                });
            }

            var hasOpenPeriod = list.Data!.Any(x => x.Status!.Trim().Equals("OPEN", StringComparison.OrdinalIgnoreCase));

            if (!hasOpenPeriod)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                await context.OpenPeriod(1, 1, new InventoryPeriodOpenRequestDto()
                {
                    PeriodName = $"Periodo-{Guid.NewGuid().ToString()[..6]}",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddMonths(1)
                });
            }

            // Act — scope propio para intentar abrir otro (debe fallar)
            BaseResponse<bool> result;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                result = await context.OpenPeriod(1, 1, new InventoryPeriodOpenRequestDto()
                {
                    PeriodName = $"Periodo-{Guid.NewGuid().ToString()[..6]}",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddMonths(1)
                });
            }

            Assert.False(result.IsSuccess);
            Assert.Contains("período abierto", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        // ===================== CLOSE PERIOD =====================

        [Fact]
        public async Task ClosePeriod_WhenSendingEmptyIdPeriod_ReturnsValidationErrors()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.ClosePeriod(1, 1, new InventoryPeriodCloseRequestDto()
            {
                IdPeriod = 0,
                PhysicalCounts = []
            });

            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, result.Message);
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task ClosePeriod_WhenPeriodNotExists_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            var result = await context.ClosePeriod(1, 1, new InventoryPeriodCloseRequestDto()
            {
                IdPeriod = 999999,
                PhysicalCounts = []
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task ClosePeriod_WhenPeriodAlreadyClosed_ReturnsNotFound()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();

            // IdPeriod=1 ya está CLOSED — no debe poder cerrarse de nuevo
            var result = await context.ClosePeriod(1, 1, new InventoryPeriodCloseRequestDto()
            {
                IdPeriod = 1,
                PhysicalCounts = []
            });

            Assert.Equal(ReplyMessage.MESSAGE_NOT_FOUND, result.Message);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task ClosePeriod_WhenOpenPeriodExistsWithoutPhysicalCount_ClosesSuccessfully()
        {
            // Arrange — leer el período abierto actual
            BaseResponse<IEnumerable<InventoryPeriodResponseDto>> list;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                list = await context.ListPeriods(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "IdPeriod",
                    Download = false
                });
            }

            var current = list.Data!.FirstOrDefault(x => x.Status!.Trim().Equals("OPEN", StringComparison.OrdinalIgnoreCase));

            if (current is null)
            {
                Assert.Fail("No existe un período OPEN en la BD. Crea uno antes de ejecutar este test.");
                return;
            }

            // Act — scope propio para cerrar
            BaseResponse<bool> result;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                result = await context.ClosePeriod(1, 1, new InventoryPeriodCloseRequestDto()
                {
                    IdPeriod = current.IdPeriod,
                    PhysicalCounts = []
                });
            }

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            // Teardown — scope propio para reabrir
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                await context.OpenPeriod(1, 1, new InventoryPeriodOpenRequestDto()
                {
                    PeriodName = current.PeriodName,
                    StartDate = DateTime.ParseExact(current.StartDate!, "dd/MM/yyyy HH:mm", null),
                    EndDate = DateTime.ParseExact(current.EndDate!, "dd/MM/yyyy HH:mm", null)
                });
            }
        }

        [Fact]
        public async Task ClosePeriod_WhenPhysicalCountProvided_PersistsPhysicalStockAndDifference()
        {
            // Arrange — leer el período abierto actual
            BaseResponse<IEnumerable<InventoryPeriodResponseDto>> list;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                list = await context.ListPeriods(1, new BaseFiltersRequest()
                {
                    NumberPage = 1,
                    NumberRecordsPage = 100,
                    Sort = "IdPeriod",
                    Download = false
                });
            }

            var current = list.Data!.FirstOrDefault(x => x.Status!.Trim().Equals("OPEN", StringComparison.OrdinalIgnoreCase));

            if (current is null)
            {
                Assert.Fail("No existe un período OPEN en la BD. Crea uno antes de ejecutar este test.");
                return;
            }

            // Arrange — leer stock calculado
            BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>> systemStock;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                systemStock = await context.GetSystemStockCalculated(current.IdPeriod);
            }

            if (!systemStock.Data!.Any())
            {
                return;
            }

            var firstProduct = systemStock.Data!.First();

            // Act — scope propio para cerrar con conteo físico
            BaseResponse<bool> result;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                result = await context.ClosePeriod(1, 1, new InventoryPeriodCloseRequestDto()
                {
                    IdPeriod = current.IdPeriod,
                    PhysicalCounts =
                    [
                        new InventoryPeriodPhysicalCountDto
                {
                    IdProduct = firstProduct.IdProduct,
                    PhysicalStock = 50
                }
                    ]
                });
            }

            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, result.Message);
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            // Assert — scope propio para verificar closing guardado
            BaseResponse<IEnumerable<InventoryPeriodClosingResponseDto>> closing;
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                closing = await context.GetClosingByPeriod(current.IdPeriod);
            }

            var entry = closing.Data!.First(x => x.IdProduct == firstProduct.IdProduct);
            Assert.Equal(50, entry.PhysicalStock);
            Assert.Equal(50, entry.ClosingStock);
            Assert.Equal(50 - firstProduct.SystemStock, entry.Difference);

            // Teardown — scope propio para reabrir
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IInventoryPeriodService>();
                await context.OpenPeriod(1, 1, new InventoryPeriodOpenRequestDto()
                {
                    PeriodName = current.PeriodName,
                    StartDate = DateTime.ParseExact(current.StartDate!, "dd/MM/yyyy HH:mm", null),
                    EndDate = DateTime.ParseExact(current.EndDate!, "dd/MM/yyyy HH:mm", null)
                });
            }
        }
    }
}
