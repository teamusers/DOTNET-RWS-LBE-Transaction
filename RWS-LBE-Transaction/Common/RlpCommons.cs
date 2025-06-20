using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.Exceptions;
using RWS_LBE_Transaction.Helpers;

namespace RWS_LBE_Transaction.Common
{
    public static class RlpApiEndpoints
    {
        public const string GetAllCampaigns = "/priv/v1/apps/:api_key/campaigns";
        public const string GetCampaignsById = "/priv/v1/apps/:api_key/external/users/:external_id/campaigns";
        public const string FetchOffersDetails = "/offers/api/2.0/offers/fetch_offers_details";
        public const string ViewTransaction = "/priv/v1/apps/:api_key/external/users/:external_id/timelines/END_USER_MEMBER_STATEMENT";
        public const string ViewStoreTransaction = "/transactions/api/1.0/transactions/info/get_store_transactions";
        public const string ViewPoint = "/priv/v1/apps/:api_key/external/users/:external_id?user[user_profile]=true&expand_incentives=true&show_identifiers=true";
        public const string SendTransaction = "/api/2.0/send_transaction";
        public const string ViewAllBalances = "/incentives/api/1.0/user_points/all_balances";
        public const string SpendMultipleTransactions = "/incentives/api/1.0/user_points/spend_multiple";
    }

    public class RlpApiQueries
    {
        public const string CampaignsQuery = "sections=behaviors,progress,offers,redemptions?creatives=tiles,promotion";
    }

    public class RlpApiErrors
    {
        public const string UserNotFound = "user_not_found";
        public const string NoActiveCampaignFound = "no_active_campaign_found";

        public static IActionResult Handle(Exception ex)
        {
            if (ex is ExternalApiException { RawResponse: not null } apiEx)
            {
                if (ApiException.TryParseJson<RlpErrorResponse>(apiEx.RawResponse, out var errResp))
                {
                    return errResp?.Errors?.Code switch
                    {
                        UserNotFound => new ConflictObjectResult(ResponseTemplate.ExistingUserNotFoundErrorResponse()),
                        NoActiveCampaignFound => new ConflictObjectResult(ResponseTemplate.ActiveCampaignNotFoundErrorResponse()),
                        _ => new BadRequestObjectResult(ResponseTemplate.UnmappedRlpErrorResponse(errResp))
                    };
                }
            }

            return new ObjectResult(ResponseTemplate.InternalErrorResponse()) { StatusCode = 500 };
        }
    }
}