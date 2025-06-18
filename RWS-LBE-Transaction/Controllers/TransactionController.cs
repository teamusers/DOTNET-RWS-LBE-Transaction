using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Services.Interfaces;
using RWS_LBE_Transaction.Common;

namespace RWS_LBE_Transaction.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly IRlpService _rlpService;

        public TransactionController(
            IRlpService rlpService,
            ILogger<TransactionController> logger)
        {
            _rlpService = rlpService;
            _logger = logger;
        }

        [HttpGet("user/{externalId}")]
        public async Task<IActionResult> GetTransaction(string externalId)
        {
            try
            {
                var transaction = await _rlpService.ViewTransaction(externalId);
                return Ok(ResponseTemplate.GenericSuccessResponse(transaction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get all user transaction.");
                return RlpApiErrors.Handle(ex);
            }
        }
    }
} 