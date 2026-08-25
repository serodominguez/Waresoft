using Application.Commons.Bases.Request;
using Application.Dtos.Request.InventoryPeriod;
using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Controllers;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class InventoryPeriodController : BaseApiController
    {
        private readonly IInventoryPeriodService _inventoryPeriodService;

        public InventoryPeriodController(IInventoryPeriodService inventoryPeriodService)
        {
            _inventoryPeriodService = inventoryPeriodService;
        }

        [HttpGet]
        [RequirePermission("Periodo", "Leer")]
        public async Task<IActionResult> ListPeriods([FromQuery] BaseFiltersRequest filters)
        {
            var response = await _inventoryPeriodService.ListPeriods(AuthenticatedUserStoreId, filters);
            return Ok(response);
        }

        [HttpGet("{periodId}")]
        [RequirePermission("Periodo", "Leer")]
        public async Task<IActionResult> GetPeriodDetail(int periodId)
        {
            var response = await _inventoryPeriodService.GetPeriodDetail(periodId);
            return Ok(response);
        }

        [HttpGet("{periodId}/Closing")]
        [RequirePermission("Periodo", "Leer")]
        public async Task<IActionResult> GetClosingByPeriod(int periodId)
        {
            var response = await _inventoryPeriodService.GetClosingByPeriod(periodId);
            return Ok(response);
        }

        [HttpGet("{periodId}/Opening")]
        [RequirePermission("Periodo", "Leer")]
        public async Task<IActionResult> GetOpeningByPeriod(int periodId)
        {
            var response = await _inventoryPeriodService.GetOpeningByPeriod(periodId);
            return Ok(response);
        }

        [HttpGet("{periodId}/SystemStock")]
        [RequirePermission("Periodo", "Leer")]
        public async Task<IActionResult> GetSystemStockCalculated(int periodId)
        {
            var response = await _inventoryPeriodService.GetSystemStockCalculated(periodId);
            return Ok(response);
        }

        [HttpPost("Open")]
        [RequirePermission("Periodo", "Crear")]
        public async Task<IActionResult> OpenPeriod([FromBody] InventoryPeriodOpenRequestDto requestDto)
        {
            var response = await _inventoryPeriodService.OpenPeriod(AuthenticatedUserId, AuthenticatedUserStoreId, requestDto);
            return Ok(response);
        }

        [HttpPut("Close")]
        [RequirePermission("Periodo", "Editar")]
        public async Task<IActionResult> ClosePeriod([FromBody] InventoryPeriodCloseRequestDto requestDto)
        {
            var response = await _inventoryPeriodService.ClosePeriod(AuthenticatedUserId, AuthenticatedUserStoreId, requestDto);
            return Ok(response);
        }
    }
}
