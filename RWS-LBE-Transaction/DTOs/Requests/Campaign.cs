
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.Requests
{
    //TODO: remove after testing
    public class CampaignTestRequest
    {
        [JsonPropertyName("hostname")]
        public string HostName { get; set; } = string.Empty;
    }
}