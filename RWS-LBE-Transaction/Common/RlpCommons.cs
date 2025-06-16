using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.DTOs.RLP.Responses;

namespace RWS_LBE_Transaction.Common
{
    public class RlpApiEndpoints
    {
        public const string GetAllCampaigns = "/priv/v1/apps/:api_key/campaigns";
    }

    public class RlpApiQueries
    {
        public const string CampaignsQuery = "sections=behaviors,progress,offers,redemptions?creatives=tiles,promotion";
    }

    public class RlpApiErrors
    {
        public const string UserNotFound = "user_not_found";

        public static IActionResult Handle(string raw)
        {
            try
            {
                var errResp = JsonSerializer.Deserialize<RlpErrorResponse>(raw);

                if (errResp?.Errors?.Code == UserNotFound)
                {
                    return new ConflictObjectResult(ApiResponse.ExistingUserNotFoundErrorResponse());
                }

                return new BadRequestObjectResult(ApiResponse.UnmappedRlpErrorResponse(errResp));
            }
            catch (JsonException)
            {
                return new ObjectResult(ApiResponse.InternalErrorResponse()) { StatusCode = 500 };
            }
        }
    }
}