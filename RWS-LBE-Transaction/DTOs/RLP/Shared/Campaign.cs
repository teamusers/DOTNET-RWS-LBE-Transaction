using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Shared
{
    public class RlpCampaignList
    {
        [JsonPropertyName("tiles")]
        public Campaign[]? Tiles { get; set; }

        [JsonPropertyName("promotions")]
        public Campaign[]? Promotions { get; set; }

    }

    public class Campaign
    {
        // only necessary fields are mapped due to large payload size from rlp
        [JsonPropertyName("custom_payload")]
        public CustomPayload? CustomPayload { get; set; }
    }

    public class CustomPayload
    {
        [JsonPropertyName("offer_id")]
        public string? OfferId { get; set; }
    }
}