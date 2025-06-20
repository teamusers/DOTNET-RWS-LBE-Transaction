using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.Requests
{
    public class SendTransaction
    {
        [JsonPropertyName("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [JsonPropertyName("request_payload")]
        public SendTransactionRequestPayload RequestPayload { get; set; } = new();
    }

    public class SendTransactionRequestPayload
    {
        [JsonPropertyName("pos_employee_id")]
        public string PosEmployeeId { get; set; } = string.Empty;

        [JsonPropertyName("sm_employee_id")]
        public string? SmEmployeeId { get; set; }

        [JsonPropertyName("table_id")]
        public string? TableId { get; set; }

        [JsonPropertyName("guest_count")]
        public int? GuestCount { get; set; }

        [JsonPropertyName("is_closed")]
        public bool IsClosed { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("from_transaction_id")]
        public string? FromTransactionId { get; set; }

        [JsonPropertyName("subtotal")]
        public double Subtotal { get; set; }

        [JsonPropertyName("tax_total")]
        public double TaxTotal { get; set; }

        [JsonPropertyName("open_time")]
        public DateTime OpenTime { get; set; }

        [JsonPropertyName("modified_time")]
        public DateTime ModifiedTime { get; set; }

        [JsonPropertyName("guest_receipt_code")]
        public string? GuestReceiptCode { get; set; }

        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<TransactionItem> Items { get; set; } = new();

        [JsonPropertyName("payments")]
        public List<TransactionPayment> Payments { get; set; } = new();

        [JsonPropertyName("discounts")]
        public List<TransactionDiscount> Discounts { get; set; } = new();

        [JsonPropertyName("custom_data")]
        public Dictionary<string, object>? CustomData { get; set; }

        [JsonPropertyName("business_def_collection")]
        public List<Dictionary<string, object>>? BusinessDefCollection { get; set; }
    }

    public class TransactionItem
    {
        [JsonPropertyName("line_id")]
        public string LineId { get; set; } = string.Empty;

        [JsonPropertyName("item_id")]
        public string ItemId { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public double Quantity { get; set; }

        [JsonPropertyName("unit_price")]
        public double UnitPrice { get; set; }

        [JsonPropertyName("subtotal")]
        public double Subtotal { get; set; }

        [JsonPropertyName("tax_included")]
        public double TaxIncluded { get; set; }

        [JsonPropertyName("modifies_line_id")]
        public string? ModifiesLineId { get; set; }

        [JsonPropertyName("custom_data")]
        public Dictionary<string, object>? CustomData { get; set; }
    }

    public class TransactionPayment
    {
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("payment_time")]
        public DateTime PaymentTime { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("user_id_type")]
        public string? UserIdType { get; set; }

        [JsonPropertyName("additional_user_id")]
        public string? AdditionalUserId { get; set; }

        [JsonPropertyName("additional_user_id_type")]
        public string? AdditionalUserIdType { get; set; }

        [JsonPropertyName("receipt_code")]
        public string? ReceiptCode { get; set; }

        [JsonPropertyName("custom_data")]
        public Dictionary<string, object>? CustomData { get; set; }
    }

    public class TransactionDiscount
    {
        [JsonPropertyName("reference_id")]
        public string ReferenceId { get; set; } = string.Empty;

        [JsonPropertyName("reference_id_type")]
        public string ReferenceIdType { get; set; } = string.Empty;

        [JsonPropertyName("pos_discount_id")]
        public string? PosDiscountId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("discounted_line_ids")]
        public List<string>? DiscountedLineIds { get; set; }

        [JsonPropertyName("discounted_line_id_quantities")]
        public List<double>? DiscountedLineIdQuantities { get; set; }

        [JsonPropertyName("discount_source")]
        public string DiscountSource { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("stack_order")]
        public int StackOrder { get; set; }

        [JsonPropertyName("applied_time")]
        public DateTime AppliedTime { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }
    }
} 