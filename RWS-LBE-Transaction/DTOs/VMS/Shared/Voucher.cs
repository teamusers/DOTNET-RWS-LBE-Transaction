using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.VMS.Shared
{
    public class VoucherTypeDT
    {
        [JsonPropertyName("VoucherTypeCode")]
        public string? VoucherTypeCode { get; set; }

        [JsonPropertyName("VoucherTypeDescription")]
        public string? VoucherTypeDescription { get; set; }
    }

    public class VoucherIssuanceParamDT
    {
        public string? SystemTransactionID { get; set; }
        public string? TerminalCode { get; set; }
        public string? TerminalDescription { get; set; }
        public string? VoucherNo { get; set; }
        public double VoucherValue { get; set; }
        public double FaceValue { get; set; }
        public double DiscountPercentage { get; set; }
        public bool IsInclusiveGSTFlag { get; set; }
        public DateTime IssueDateTime { get; set; }
        public DateTime ValidityStartTime { get; set; }
        public DateTime ValidityEndTime { get; set; }
        public string? VoucherTypeCode { get; set; }
        public string? TransactionTypeCode { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemDescription { get; set; }
        public string? CostCentre { get; set; }
        public double GSTRate { get; set; }
        public double GSTAmount { get; set; }
        public string? MediaCode { get; set; }
        public string? ExtSysPromoCode { get; set; }
    }

    public class VoucherEnquiryParamDT
    {
        public string? VoucherNo { get; set; }
    }

    public class VoucherUtilizationParamDT
    {
        public string? SystemTransactionID { get; set; }

        public string? TerminalCode { get; set; }

        public string? VoucherNo { get; set; }

        public DateTime UtilizeDateTime { get; set; }

        public bool IsUtilized { get; set; } = false;
    }

    public class VoucherRequestParamDT
    {
        public bool IsBatchProcess { get; set; } = false;
    }

    public class VoucherIssuanceInfoDT
    {
        [JsonPropertyName("systemTransactionID")]
        public string? SystemTransactionID { get; set; }

        [JsonPropertyName("vmsTransactionID")]
        public string? VmsTransactionID { get; set; }

        [JsonPropertyName("voucherNo")]
        public string? VoucherNo { get; set; }

        [JsonPropertyName("voucherErrorCodeID")]
        public int VoucherErrorCodeID { get; set; }

        [JsonPropertyName("voucherErrorCodeDescription")]
        public string? VoucherErrorCodeDescription { get; set; }

        [JsonPropertyName("mediaCode")]
        public string? MediaCode { get; set; }
    }

    public class VoucherEnquiryInfoDT
    {
        [JsonPropertyName("voucherNo")]
        public string? VoucherNo { get; set; }

        [JsonPropertyName("faceValue")]
        public decimal FaceValue { get; set; }

        [JsonPropertyName("discountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [JsonPropertyName("isInclusiveGSTFlag")]
        public bool IsInclusiveGSTFlag { get; set; }

        [JsonPropertyName("voucherTypeCode")]
        public string? VoucherTypeCode { get; set; }

        [JsonPropertyName("itemDescription")]
        public string? ItemDescription { get; set; }

        [JsonPropertyName("voucherStatus")]
        public string? VoucherStatus { get; set; }

        [JsonPropertyName("voucherStatusDescription")]
        public string? VoucherStatusDescription { get; set; }

        [JsonPropertyName("voucherIssuanceSystemID")]
        public int VoucherIssuanceSystemID { get; set; }

        [JsonPropertyName("voucherTrxnID")]
        public int VoucherTrxnID { get; set; }

        [JsonPropertyName("mediaCode")]
        public string? MediaCode { get; set; }

        [JsonPropertyName("extSysPromoCode")]
        public string? ExtSysPromoCode { get; set; }
    }

    public class VoucherUtilizationInfoDT
    {
        [JsonPropertyName("systemTransactionID")]
        public string? SystemTransactionID { get; set; }

        [JsonPropertyName("vmsTransactionID")]
        public string? VmsTransactionID { get; set; }

        [JsonPropertyName("voucherNo")]
        public string? VoucherNo { get; set; }

        [JsonPropertyName("redemptionValue")]
        public decimal RedemptionValue { get; set; }

        [JsonPropertyName("voucherErrorCodeID")]
        public int VoucherErrorCodeID { get; set; }

        [JsonPropertyName("voucherErrorCodeDescription")]
        public string? VoucherErrorCodeDescription { get; set; }
    }
}