using System.Text.Json;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class GetAllCampaignsResponse
    {
        [JsonPropertyName("page")]
        public int? Page { get; set; }

        [JsonPropertyName("page_size")]
        public int? PageSize { get; set; }

        [JsonPropertyName("number_of_pages")]
        public int? NumberOfPages { get; set; }

        [JsonPropertyName("campaigns")]
        public JsonElement Campaigns { get; set; }
    }

    public class GetCampaignsByIdResponse
    {
        [JsonPropertyName("campaigns")]
        public JsonElement Campaigns { get; set; }
    }
}