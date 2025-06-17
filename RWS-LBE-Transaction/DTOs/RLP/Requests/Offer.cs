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
}