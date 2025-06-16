using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.Exceptions;
using RWS_LBE_Transaction.Services.Interfaces;

namespace RWS_LBE_Transaction.Controllers
{
    [ApiController]
    [Route("api/v1/voucher/campaign")]
    public class CampaignController : ControllerBase
    {
        private readonly ILogger<CampaignController> _logger;
        private readonly IRlpService _rlp;
        public CampaignController(ILogger<CampaignController> logger, IRlpService rlpService)
        {
            _logger = logger;
            _rlp = rlpService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var pageString = Request.Query["page"];
            if (!int.TryParse(pageString, out var page) || page < 1)
            {
                return BadRequest(ApiResponse.InvalidQueryParametersErrorResponse());
            }
 
            try
            {
                var response = await _rlp.GetAllCampaigns(page);

                return Ok(ApiResponse.GenericSuccessResponse(response));
            }
            catch (ExternalApiException ex)
            {
                return RlpApiErrors.Handle(ex.RawResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all campaigns error");
                return StatusCode(500, ApiResponse.InternalErrorResponse());
            }

        }
    }
}