using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.RLP.Shared;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class FetchOffersDetailsResponse
    {
        [JsonPropertyName("payload")]
        public OfferDetailsPayload? Payload { get; set; }
    }

    public class OfferDetailsPayload
    {
        [JsonPropertyName("results")]
        public List<Offer>? Results { get; set; }
    }

    public class IssueOfferResponse
    {
        [JsonPropertyName("payload")]
        public IssueOfferPayload? Payload { get; set; }
    }

    public class IssueOfferPayload
    {
        [JsonPropertyName("status_list")]
        public List<OfferStatusList>? StatusList { get; set; }
    }

    public class OfferStatusList
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = default!;

        [JsonPropertyName("user_offer_id")]
        public string UserOfferId { get; set; } = default!;

        [JsonPropertyName("offer_id")]
        public string OfferId { get; set; } = default!;

        [JsonPropertyName("root_offer_id")]
        public string RootOfferId { get; set; } = default!;

        [JsonPropertyName("pending_extended_data")]
        public bool PendingExtendedData { get; set; }

        [JsonPropertyName("fail_reason")]
        public int FailReason { get; set; }
    }
}