using System.Text.Json;
using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.RLP.Shared;

namespace RWS_LBE_Transaction.DTOs.Responses
{
    public class GetCampaignsByIdResponseData
    {
        [JsonPropertyName("campaigns")]
        public JsonElement Campaigns { get; set; }

        [JsonPropertyName("offers")]
        public Offer[] Offers { get; set; } = [];
    }
}