using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class SendTransactionResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("payload")]
        public object? Payload { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }
    }
} 