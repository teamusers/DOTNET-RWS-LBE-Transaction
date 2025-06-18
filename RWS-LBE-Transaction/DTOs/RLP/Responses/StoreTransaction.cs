using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class StoreTransactionsResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("errors")]
        public IErrorResponse? Errors { get; set; }

        [JsonPropertyName("payload")]
        public GetStoreTransactionsResponse? Payload { get; set; }
    }

    public class IErrorResponse
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("raw_message")]
        public string? RawMessage { get; set; }

        [JsonPropertyName("stack_trace")]
        public string? StackTrace { get; set; }
    }

    public class GetStoreTransactionsResponse
    {
        [JsonPropertyName("total_records")]
        public int TotalRecords { get; set; }

        [JsonPropertyName("results")]
        public List<TransactionDTO>? Results { get; set; }

        [JsonPropertyName("pagination_token")]
        public string? PaginationToken { get; set; }
    }

    public class TransactionDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("store_id")]
        public string? StoreId { get; set; }

        [JsonPropertyName("pos_transaction_id")]
        public string? PosTransactionId { get; set; }

        [JsonPropertyName("pos_employee_id")]
        public string? PosEmployeeId { get; set; }

        [JsonPropertyName("table_id")]
        public string? TableId { get; set; }

        [JsonPropertyName("guest_count")]
        public int GuestCount { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("is_closed")]
        public bool IsClosed { get; set; }

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonPropertyName("taxtotal")]
        public decimal TaxTotal { get; set; }

        [JsonPropertyName("check_amount")]
        public decimal CheckAmount { get; set; }

        [JsonPropertyName("guest_receipt_code")]
        public string? GuestReceiptCode { get; set; }

        [JsonPropertyName("channel")]
        public string? Channel { get; set; }

        [JsonPropertyName("sm_transaction_process_date")]
        public string? SmTransactionProcessDate { get; set; }

        [JsonPropertyName("open_time")]
        public string? OpenTime { get; set; }

        [JsonPropertyName("modified_time")]
        public string? ModifiedTime { get; set; }

        [JsonPropertyName("items")]
        public List<TransactionItemDTO>? Items { get; set; }

        [JsonPropertyName("payments")]
        public List<TransactionPaymentDTO>? Payments { get; set; }

        [JsonPropertyName("discounts")]
        public List<TransactionDiscountDTO>? Discounts { get; set; }
    }

    public class TransactionItemDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("master_item_id")]
        public string? MasterItemId { get; set; }

        [JsonPropertyName("pos_item_key")]
        public string? PosItemKey { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonPropertyName("tax_included")]
        public decimal TaxIncluded { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("store_id")]
        public string? StoreId { get; set; }

        [JsonPropertyName("line_id")]
        public string? LineId { get; set; }

        [JsonPropertyName("modifies_line_id")]
        public string? ModifiesLineId { get; set; }
    }

    public class TransactionPaymentDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("payment_id")]
        public string? PaymentId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("payment_time")]
        public string? PaymentTime { get; set; }

        [JsonPropertyName("retailer_id")]
        public string? RetailerId { get; set; }

        [JsonPropertyName("store_id")]
        public string? StoreId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }
    }

    public class TransactionDiscountDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("reference_id")]
        public string? ReferenceId { get; set; }

        [JsonPropertyName("reference_id_type")]
        public string? ReferenceIdType { get; set; }

        [JsonPropertyName("pos_discount_id")]
        public string? PosDiscountId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("discounted_line_ids")]
        public List<string>? DiscountedLineIds { get; set; }

        [JsonPropertyName("discount_source")]
        public string? DiscountSource { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("stack_order")]
        public int StackOrder { get; set; }

        [JsonPropertyName("applied_time")]
        public string? AppliedTime { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }
    }
} 