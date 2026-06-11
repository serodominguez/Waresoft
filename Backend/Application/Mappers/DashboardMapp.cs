using Application.Dtos.Response.Dashboard;
using Infrastructure.Persistences.ReadModels.Dashboard;

namespace Application.Mappers
{
    public static class DashboardMapp
    {
        private static readonly string[] MonthNames =
            { "Ene", "Feb", "Mar", "Abr", "May", "Jun",
                "Jul", "Ago", "Sep", "Oct", "Nov", "Dec" };

        public static DashboardGoodsIssueStatsResponseDto GoodsIssueStatsResponseDtoMapping(DashboardGoodsIssueStatsReadModel model)
        {
            var difference = model.IssuedLast7Days - model.IssuedPrevious7Days;

            return new DashboardGoodsIssueStatsResponseDto
            {
                TotalIssues = model.TotalIssues,
                DifferenceVsLast7Days = Math.Abs(difference),
                IsPositive = difference >= 0
            };
        }

        public static DashboardStoreInventoryStatsResponseDto InventoryStatsResponseDtoMapping(DashboardStoreInventoryStatsReadModel model)
        {
            var difference = model.BelowMinimumThisMonth - model.BelowMinimumLastMonth;

            return new DashboardStoreInventoryStatsResponseDto
            {
                BelowMinimum = model.BelowMinimum,
                DifferenceVsLastMonth = Math.Abs(difference),
                IsPositive = difference >= 0
            };
        }

        public static DashboardMovementResponseDto MovementStatsResponseDtoMapping(DashboardMovementStatsReadModel model)
        {
            return new DashboardMovementResponseDto
            {
                Month = MonthNames[model.Month - 1],
                Receipts = Math.Round(model.TotalReceipts / 100, 2),
                Issues = Math.Round(model.TotalIssues / 100, 2)
            };
        }

        public static DashboardProductReplenishmentResponseDto ProductReplenishmentResponseDtoMapping(DashboardProductReplenishmentReadModel model)
        {
            return new DashboardProductReplenishmentResponseDto
            {
                Available = model.Available,
                NotAvailable = model.NotAvailable,
                Discontinued = model.Discontinued
            };
        }

        public static DashboardProductStatsResponseDto ProductStatsResponseDtoMapping(DashboardProductStatsReadModel model)
        {
            return new DashboardProductStatsResponseDto
            {
                TotalActive = model.TotalActive,
                NewThisMonth = model.NewThisMonth
            };
        }

        public static DashboardTransferByStoreResponseDto TransferByStoreResponseDtoMapping(DashboardTransferByStoreReadModel model)
        {
            return new DashboardTransferByStoreResponseDto
            {
                StoreName = model.StoreName,
                TotalTransfers = model.TotalTransfers
            };
        }

        public static DashboardTransferStatsResponseDto TransferPendingResponseDtoMapping(DashboardTransferPendingReadModel model)
        {
            var difference = model.PendingToday - model.PendingYesterday;

            return new DashboardTransferStatsResponseDto
            {
                TotalPending = model.TotalPending,
                DifferenceVsYesterday = Math.Abs(difference),
                IsPendingPositive = difference >= 0
            };
        }

        public static DashboardTransferStatusResponseDto TransferStatusResponseDtoMapping(DashboardTransferStatusReadModel model)
        {
            return new DashboardTransferStatusResponseDto
            {
                Month = MonthNames[model.Month - 1],
                Sent = model.Sent,
                Pending = model.Pending,
                Received = model.Received
            };
        }
    }
}
