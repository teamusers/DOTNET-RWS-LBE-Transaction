using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Responses;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.DTOs.RLP.Shared;
using RWS_LBE_Transaction.Helpers;
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
                return BadRequest(ResponseTemplate.InvalidQueryParametersErrorResponse());
            }

            try
            {
                var response = await _rlp.GetAllCampaigns(page);

                return Ok(ResponseTemplate.GenericSuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get all campaigns.");
                return RlpApiErrors.Handle(ex);
            }

        }

        [HttpGet("{rlpId}")]
        public async Task<IActionResult> GetCampaignsById([FromRoute] string rlpId)
        {
            GetCampaignsByIdResponse? campaignsResponse;
            try
            {
                campaignsResponse = await _rlp.GetCampaignsById(rlpId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get campaigns by ID.");
                return RlpApiErrors.Handle(ex);
            }

            // ensure that response data is present
            if (campaignsResponse?.Campaigns.ValueKind != JsonValueKind.Object)
            {
                _logger.LogError("[API EXCEPTION] RLP: Get campaigns by ID returned no data.");
                return StatusCode(500, ResponseTemplate.InternalErrorResponse());
            }

            var offerIdList = RlpHelper.ExtractOfferIDsFromJsonElement(campaignsResponse.Campaigns);
            List<Offer>? offerDetailsList = [];

            if (offerIdList.Count != 0)
            {
                // fetch offer details
                try
                {
                    var offersResponse = await _rlp.FetchOffersDetails(offerIdList);
                    offerDetailsList = offersResponse?.Payload?.Results;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to fetch offer details.");
                    return RlpApiErrors.Handle(ex);
                }
            }

            return Ok(ResponseTemplate.GenericSuccessResponse(new GetCampaignsByIdResponseData
            {
                Campaigns = campaignsResponse.Campaigns,
                Offers = offerDetailsList ?? []
            }));
        }
    }
}