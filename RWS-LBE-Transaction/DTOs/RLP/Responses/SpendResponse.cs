using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class SpendResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("errors")]
        public DomainErrorResponse? Errors { get; set; }

        [JsonPropertyName("payload")]
        public SpendResponsePayload? Payload { get; set; }
    }

    public class SpendResponsePayload
    {
        [JsonPropertyName("details")]
        public List<SpendPointsDetail> Details { get; set; } = new();

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("summary")]
        public SpendSummary Summary { get; set; } = new();

        [JsonPropertyName("audits")]
        public List<PointAuditLog> Audits { get; set; } = new();

        [JsonPropertyName("time_of_occurrence")]
        public string? TimeOfOccurrence { get; set; }
    }

    public class SpendPointsDetail
    {
        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        [JsonPropertyName("user_point_account_id")]
        public string? UserPointAccountId { get; set; }

        [JsonPropertyName("point_account_id")]
        public string? PointAccountId { get; set; }

        [JsonPropertyName("amount_spent")]
        public double AmountSpent { get; set; }

        [JsonPropertyName("remaining_balance")]
        public double RemainingBalance { get; set; }

        [JsonPropertyName("reference_id")]
        public string? ReferenceId { get; set; }

        [JsonPropertyName("reference_type")]
        public string? ReferenceType { get; set; }

        [JsonPropertyName("tracking_id")]
        public string? TrackingId { get; set; }

        [JsonPropertyName("idempotency_id")]
        public string? IdempotencyId { get; set; }

        [JsonPropertyName("error_status")]
        public int ErrorStatus { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("point_source_id")]
        public string? PointSourceId { get; set; }

        [JsonPropertyName("idempotency_summary")]
        public string? IdempotencySummary { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("error_response")]
        public DomainErrorResponse? ErrorResponse { get; set; }
    }

    public class SpendSummary
    {
        [JsonPropertyName("amount_spent")]
        public double AmountSpent { get; set; }

        [JsonPropertyName("remaining_balance")]
        public double RemainingBalance { get; set; }
    }

    public class PointAuditLog
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        [JsonPropertyName("point_account_id")]
        public string? PointAccountId { get; set; }

        [JsonPropertyName("user_point_account_id")]
        public string? UserPointAccountId { get; set; }

        [JsonPropertyName("modification")]
        public double Modification { get; set; }

        [JsonPropertyName("amount_spent")]
        public double AmountSpent { get; set; }

        [JsonPropertyName("amount_expired")]
        public double AmountExpired { get; set; }

        [JsonPropertyName("audit_type")]
        public int AuditType { get; set; }

        [JsonPropertyName("modification_type")]
        public string? ModificationType { get; set; }

        [JsonPropertyName("modification_entity_id")]
        public string? ModificationEntityId { get; set; }

        [JsonPropertyName("spend_weight")]
        public int SpendWeight { get; set; }

        [JsonPropertyName("point_source_id")]
        public string? PointSourceId { get; set; }

        [JsonPropertyName("point_source_name")]
        public string? PointSourceName { get; set; }

        [JsonPropertyName("time_of_occurrence")]
        public string? TimeOfOccurrence { get; set; }

        [JsonPropertyName("request_id")]
        public string? RequestId { get; set; }

        [JsonPropertyName("create_date")]
        public string? CreateDate { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("idempotency_id")]
        public string? IdempotencyId { get; set; }
    }

    public class DomainErrorResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
} 