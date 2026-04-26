using Application.Interfaces;
using Application.Security;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    [Route("api/[controller]")]
    public class SequenceController : BaseApiController
    {
        private readonly ISequenceService _sequenceService;

        public SequenceController(ISequenceService sequenceService)
        {
            _sequenceService = sequenceService;
        }

        [HttpGet("Product-Code")]
        [RequirePermission("Productos", "Crear")]
        public async Task<IActionResult> GenerateProductCode()
        {
            var response = await _sequenceService.ViewProductCode();
            return Ok(response);
        }
    }
}
