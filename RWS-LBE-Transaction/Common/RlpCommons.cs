using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.DTOs.VMS.Shared;
using RWS_LBE_Transaction.Exceptions;

namespace RWS_LBE_Transaction.Common
{
    public class RlpApiEndpoints
    {
        public const string GetAllCampaigns = "/priv/v1/apps/:api_key/campaigns";
        public const string GetCampaignsById = "/priv/v1/apps/:api_key/external/users/:external_id/campaigns";
        public const string FetchOffersDetails = "/offers/api/2.0/offers/fetch_offers_details";
        public const string ViewTransaction = "/priv/v1/apps/:api_key/external/users/:external_id/timelines/END_USER_MEMBER_STATEMENT";
        public const string RevokeOffer = "/offers/api/2.0/offers/acquisition/revoke";
        public const string UpdateOffer = "/offers/api/2.0/offers/acquisition/update";
    }

    public class RlpApiQueries
    {
        public const string CampaignsQuery = "sections=behaviors,progress,offers,redemptions?creatives=tiles,promotion";
    }

    public class RlpRevokeOfferReasons
    {
        public static readonly RevokeOfferReason INVALID_VOUCHER_TYPE_CODE = new()
        {
            LbeErrorCode = Codes.VOUCHER_REVOKED_INVALID_VOUCHER_TYPE_CODE,
            Description = "Invalid VoucherTypeCode."
        };

        public static readonly RevokeOfferReason INVALID_TRANSACTION_TYPE_CODE = new()
        {
            LbeErrorCode = Codes.VOUCHER_REVOKED_INVALID_TRANSACTION_TYPE_CODE,
            Description = "Invalid TransactionTypeCode."
        };

        public static readonly RevokeOfferReason VMS_TIMEOUT = new()
        {
            LbeErrorCode = Codes.VOUCHER_REVOKED_VMS_TIMEOUT,
            Description = "VMS connection timed out."
        };

        public static readonly RevokeOfferReason VMS_ERROR = new()
        {
            LbeErrorCode = Codes.VOUCHER_REVOKED_VMS_ERROR,
            Description = "VMS encountered an error"
        };
    }

    public class RevokeOfferReason
    {
        public int LbeErrorCode { get; set; }
        public string? Description { get; set; }
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