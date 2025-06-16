using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{

    public class RlpErrorResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("errors")]
        public RlpErrors Errors { get; set; } = new RlpErrors();
    }

    public class RlpErrors
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("user")]
        public object? User { get; set; }
    }
}