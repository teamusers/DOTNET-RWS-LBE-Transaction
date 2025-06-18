using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.VMS.Shared
{
    public class VoucherType
    {
        [JsonPropertyName("VoucherTypeCode")]
        public string? VoucherTypeCode { get; set; }

        [JsonPropertyName("VoucherTypeDescription")]
        public string? VoucherTypeDescription { get; set; }
    }

    public class VoucherIssuanceParam
    {
        public string? SystemTransactionID { get; set; }
        public string? TerminalCode { get; set; }
        public string? TerminalDescription { get; set; }
        public string? VoucherNo { get; set; }
        public double VoucherValue { get; set; }
        public double FaceValue { get; set; }
        public double DiscountPercentage { get; set; }
        public string? IsInclusiveGSTFlag { get; set; }
        public DateTime IssueDateTime { get; set; }
        public DateTime ValidityStartTime { get; set; }
        public DateTime ValidityEndTime { get; set; }
        public string? VoucherTypeCode { get; set; }
        public string? TransactionTypeCode { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemDecription { get; set; } //TODO Note: original key has a typo "Decription"
        public string? CostCenter { get; set; }
        public double GSTRate { get; set; }
        public double GSTAmount { get; set; }
        public string? MediaCode { get; set; }
        public string? ExtSysPromoCode { get; set; }
    }
}