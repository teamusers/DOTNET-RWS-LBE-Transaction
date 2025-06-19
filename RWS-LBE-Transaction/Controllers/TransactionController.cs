using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Services.Interfaces;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs;
using RWS_LBE_Transaction.DTOs.RLP.Responses;

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

        [HttpPost("store")]
        public async Task<IActionResult> GetStoreTransaction([FromBody] object payload)
        {
            try
            {
                var result = await _rlpService.ViewStoreTransactionAsync(payload);
                return Ok(ResponseTemplate.GenericSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get all store transactions.");
                return RlpApiErrors.Handle(ex);
            }
        }

        [HttpGet("point/{externalId}")]
        public async Task<IActionResult> GetPoints(string externalId)
        {
            try
            {
                var response = await _rlpService.ViewPoint(externalId);
                return Ok(ResponseTemplate.GenericSuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get user points.");
                return RlpApiErrors.Handle(ex);
            }
        }
    }
} 