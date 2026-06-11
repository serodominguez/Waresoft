using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("GoodsIssueStats")]
        public async Task<IActionResult> GetGoodsIssueStats(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetGoodsIssueStats(AuthenticatedUserStoreId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("InventoryStats")]
        public async Task<IActionResult> GetInventoryStats(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetInventoryStats(AuthenticatedUserStoreId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("MovementsStats")]
        public async Task<IActionResult> GetMovementsStats(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetMovementsStats(AuthenticatedUserStoreId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("ProductReplenishment")]
        public async Task<IActionResult> GetProductReplenishment(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetProductReplenishment(AuthenticatedUserStoreId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("ProductStats")]
        public async Task<IActionResult> GetProductStats(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetProductStats(cancellationToken);
            return Ok(response);
        }

        [HttpGet("TransfersByStore")]
        public async Task<IActionResult> GetTransfersByStore(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetTransfersByStore(AuthenticatedUserStoreId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("TransferPending")]
        public async Task<IActionResult> GetTransferPending(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetTransferPending(AuthenticatedUserStoreId, cancellationToken);
            return Ok(response);
        }

        [HttpGet("TransferStatus")]
        public async Task<IActionResult> GetTransferStatus(CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetTransferStatus(AuthenticatedUserStoreId, cancellationToken);
            return Ok(response);
        }
    }
}
