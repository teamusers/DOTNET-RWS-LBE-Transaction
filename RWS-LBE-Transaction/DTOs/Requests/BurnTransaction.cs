using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.Requests
{
    public class BurnTransaction
    {
        [JsonPropertyName("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [JsonPropertyName("request_payload")]
        public SendTransactionRequestPayload RequestPayload { get; set; } = new();
    }
} 