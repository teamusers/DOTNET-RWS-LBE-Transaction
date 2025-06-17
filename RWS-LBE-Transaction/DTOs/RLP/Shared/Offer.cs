using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Shared
{
    public class Offer
    {
        [JsonPropertyName("offer_id")]
        public string? OfferId { get; set; }

        [JsonPropertyName("acquisition_count")]
        public int? AcquisitionCount { get; set; }

        [JsonPropertyName("redemption_count")]
        public int? RedemptionCount { get; set; }

        [JsonPropertyName("total_inventory")]
        public int? TotalInventory { get; set; }

        [JsonPropertyName("available_inventory_remaining")]
        public int? AvailableInventoryRemaining { get; set; }
    }
}