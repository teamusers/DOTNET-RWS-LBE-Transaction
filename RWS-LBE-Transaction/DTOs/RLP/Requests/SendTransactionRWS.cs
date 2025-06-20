using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.Requests;

namespace RWS_LBE_Transaction.DTOs.RLP.Requests
{
    public class SendTransactionRWS
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [JsonPropertyName("culture")]
        public string Culture { get; set; } = "en_US";

        [JsonPropertyName("request_payload")]
        public SendTransactionRequestPayload RequestPayload { get; set; } = new();
    }
} 