using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Requests;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services;

namespace RWS_LBE_Transaction.Controllers
{
    [ApiController]
    [Route("api/v1/voucher/campaign")]
    public class CampaignController : ControllerBase
    {
        private readonly ILogger<CampaignController> _logger;
        private readonly IApiHttpClient _apiHttpClient;
        public CampaignController(IApiHttpClient apiHttpClient, ILogger<CampaignController> logger)
        {
            _apiHttpClient = apiHttpClient;
            _logger = logger;
        }

        //TODO: remove after testing
        [HttpPost("test")]
        public async Task<IActionResult> TestEndpoint([FromBody] CampaignTestRequest req)
        {
            try
            {
                var (result, raw) = await _apiHttpClient.DoApiRequestAsync<object>(new ApiRequestOptions
                {
                    Method = HttpMethod.Post,
                    Url = req.HostName,
                    ExpectedStatus = 200 
                });

                return Ok(new ApiResponse<object>
                {
                    Code = Codes.SUCCESSFUL,
                    Message = "API call successful",
                    Data = result
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API call failed due to unexpected status or other HTTP error");

                return StatusCode(StatusCodes.Status502BadGateway, new ApiResponse<object>
                {
                    Code = Codes.INTERNAL_ERROR,
                    Message = "error",
                    Data = $"API call failed: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during API call");

                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    Code = Codes.INTERNAL_ERROR,
                    Message = "error",
                    Data = $"API call failed: {ex.Message}"
                });
            }

        }
    }
}