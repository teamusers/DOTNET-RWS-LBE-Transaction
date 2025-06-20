using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Requests
{
    public class ViewBalanceRWS
    {
        [JsonPropertyName("retailer_id")]
        public string RetailerId { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("culture")]
        public string Culture { get; set; } = "en_US";
    }

    public class SpendRequest
    {
        [JsonPropertyName("culture")]
        public string Culture { get; set; } = "en-US";

        [JsonPropertyName("retailer_id")]
        public string RetailerId { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("spend_details")]
        public List<SpendUserPointsRequestDetail> SpendDetails { get; set; } = new();

        [JsonPropertyName("allow_partial_success")]
        public bool AllowPartialSuccess { get; set; }

        [JsonPropertyName("disable_event_publishing")]
        public bool DisableEventPublishing { get; set; }

        [JsonPropertyName("parallel_mode")]
        public bool ParallelMode { get; set; }
    }

    public class SpendUserPointsRequestDetail
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonPropertyName("overdraft_id")]
        public string OverdraftId { get; set; } = string.Empty;

        [JsonPropertyName("overdraft_timestamp")]
        public string OverdraftTimestamp { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("point_account_ids")]
        public List<string> PointAccountIds { get; set; } = new();

        [JsonPropertyName("reference_id")]
        public string ReferenceId { get; set; } = string.Empty;

        [JsonPropertyName("reference_type")]
        public string ReferenceType { get; set; } = string.Empty;

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; } = string.Empty;

        [JsonPropertyName("force_spend")]
        public bool ForceSpend { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("is_return")]
        public bool IsReturn { get; set; }

        [JsonPropertyName("idempotency_id")]
        public string IdempotencyId { get; set; } = string.Empty;

        [JsonPropertyName("point_source_id")]
        public string PointSourceId { get; set; } = string.Empty;
    }
} 