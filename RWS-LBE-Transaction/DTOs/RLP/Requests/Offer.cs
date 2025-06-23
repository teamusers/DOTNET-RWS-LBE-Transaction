using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Requests
{
    public class FetchOffersDetailsRequest
    {
        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("offer_ids")]
        public List<string>? OfferIds { get; set; }

        [JsonPropertyName("skip")]
        public int Skip { get; set; }

        [JsonPropertyName("take")]
        public int Take { get; set; }
    }

    public class RevokeOfferRequest
    {
        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("user_offer_id")]
        public string? UserOfferId { get; set; }
    }

    public class UpdateOfferRequest
    {
        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("user_offer_id")]
        public string? UserOfferId { get; set; }

        [JsonPropertyName("custom_data")]
        public string? CustomData { get; set; }

        [JsonPropertyName("remain_pending_extended_data")]
        public bool RemainPendingExtendedData { get; set; } = false;

        [JsonPropertyName("update_redemption_dates")]
        public bool UpdateRedemptionDates { get; set; } = false;
    }

    public class IssueOfferRequest
    {
        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("offer_id")]
        public string? OfferId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; } = 1;

        [JsonPropertyName("issue_exact_version")]
        public bool IssueExactVerson { get; set; } = false;
    }
}