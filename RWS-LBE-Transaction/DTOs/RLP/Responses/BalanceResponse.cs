using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class UserBalanceResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("payload")]
        public UserBalancePayload Payload { get; set; } = new();
    }

    public class UserBalancePayload
    {
        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("summary")]
        public BalanceSummary Summary { get; set; } = new();

        [JsonPropertyName("details")]
        public List<BalanceAccountDetail> Details { get; set; } = new();
    }

    public class BalanceSummary
    {
        [JsonPropertyName("total_points")]
        public double TotalPoints { get; set; }

        [JsonPropertyName("life_time_points")]
        public double LifeTimePoints { get; set; }
    }

    public class BalanceAccountDetail
    {
        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        [JsonPropertyName("user_point_account_id")]
        public string? UserPointAccountId { get; set; }

        [JsonPropertyName("point_account_id")]
        public string? PointAccountId { get; set; }

        [JsonPropertyName("grouping_label")]
        public string? GroupingLabel { get; set; }

        [JsonPropertyName("available_balance")]
        public double AvailableBalance { get; set; }

        [JsonPropertyName("life_time_value")]
        public double LifeTimeValue { get; set; }
    }
} 