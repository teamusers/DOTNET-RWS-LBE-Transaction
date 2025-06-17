using System.Text.Json;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class UserTransactionResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("result")]
        public List<UserTransaction> Result { get; set; } = new();

        [JsonPropertyName("grouping_field")]
        public string? GroupingField { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("event_types")]
        public List<EventType> EventTypes { get; set; } = new();
    }

    public class UserTransaction
    {
        [JsonPropertyName("event_stream_stream_id")]
        public string EventStreamStreamId { get; set; } = string.Empty;

        [JsonPropertyName("event_stream_event_category_id")]
        public int EventStreamEventCategoryId { get; set; }

        [JsonPropertyName("event_stream_event_type_id")]
        public int EventStreamEventTypeId { get; set; }

        [JsonPropertyName("target_id")]
        public int TargetId { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("event_stream_payload")]
        public TransactionPayload EventStreamPayload { get; set; } = new();

        [JsonPropertyName("contexts")]
        public List<JsonElement> Contexts { get; set; } = new();
    }

    public class TransactionPayload
    {
        [JsonPropertyName("business_def_collection")]
        public List<object> BusinessDefCollection { get; set; } = new();

        [JsonPropertyName("catalog_lineages")]
        public Dictionary<string, object> CatalogLineages { get; set; } = new();

        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;

        [JsonPropertyName("check_amount")]
        public decimal CheckAmount { get; set; }

        [JsonPropertyName("create_date")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("custom_data")]
        public object? CustomData { get; set; }

        [JsonPropertyName("discounts")]
        public List<object> Discounts { get; set; } = new();

        [JsonPropertyName("event_category_name")]
        public string EventCategoryName { get; set; } = string.Empty;

        [JsonPropertyName("event_category_slug")]
        public string EventCategorySlug { get; set; } = string.Empty;

        [JsonPropertyName("event_type_name")]
        public string EventTypeName { get; set; } = string.Empty;

        [JsonPropertyName("event_type_slug")]
        public string EventTypeSlug { get; set; } = string.Empty;

        [JsonPropertyName("from_pos_transaction_id")]
        public string? FromPosTransactionId { get; set; }

        [JsonPropertyName("from_transaction_id")]
        public string? FromTransactionId { get; set; }

        [JsonPropertyName("guest_count")]
        public int GuestCount { get; set; }

        [JsonPropertyName("guest_receipt_code")]
        public string GuestReceiptCode { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("include_tax")]
        public bool IncludeTax { get; set; }

        [JsonPropertyName("is_closed")]
        public bool IsClosed { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("items")]
        public List<TransactionItem> Items { get; set; } = new();

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("location_lineages")]
        public Dictionary<string, List<List<string>>> LocationLineages { get; set; } = new();

        [JsonPropertyName("location_lineages_core")]
        public List<List<LocationNode>> LocationLineagesCore { get; set; } = new();

        [JsonPropertyName("modified_time")]
        public DateTime ModifiedTime { get; set; }

        [JsonPropertyName("open_time")]
        public DateTime OpenTime { get; set; }

        [JsonPropertyName("payments")]
        public List<TransactionPayment> Payments { get; set; } = new();

        [JsonPropertyName("pos_employee_id")]
        public string PosEmployeeId { get; set; } = string.Empty;

        [JsonPropertyName("pos_transaction_id")]
        public string PosTransactionId { get; set; } = string.Empty;

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("retailer_id")]
        public string RetailerId { get; set; } = string.Empty;

        [JsonPropertyName("rewards_system_id")]
        public int RewardsSystemId { get; set; }

        [JsonPropertyName("sm_employee_id")]
        public string? SmEmployeeId { get; set; }

        [JsonPropertyName("sm_transaction_process_date")]
        public DateTime SmTransactionProcessDate { get; set; }

        [JsonPropertyName("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [JsonPropertyName("store_time_zone")]
        public string StoreTimeZone { get; set; } = string.Empty;

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonPropertyName("table_id")]
        public string TableId { get; set; } = string.Empty;

        [JsonPropertyName("taxtotal")]
        public decimal TaxTotal { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;
    }

    public class TransactionItem
    {
        [JsonPropertyName("create_date")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("custom_data")]
        public object? CustomData { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("line_id")]
        public string LineId { get; set; } = string.Empty;

        [JsonPropertyName("master_item_id")]
        public string MasterItemId { get; set; } = string.Empty;

        [JsonPropertyName("modifies_line_id")]
        public string? ModifiesLineId { get; set; }

        [JsonPropertyName("pos_item_key")]
        public string PosItemKey { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("retailer_id")]
        public string RetailerId { get; set; } = string.Empty;

        [JsonPropertyName("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonPropertyName("tax_included")]
        public decimal TaxIncluded { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; } = string.Empty;

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }
    }

    public class LocationNode
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }

    public class TransactionPayment
    {
        [JsonPropertyName("additional_user_id")]
        public string? AdditionalUserId { get; set; }

        [JsonPropertyName("additional_user_id_type")]
        public string? AdditionalUserIdType { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("create_date")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("custom_data")]
        public object? CustomData { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; } = string.Empty;

        [JsonPropertyName("payment_time")]
        public DateTime PaymentTime { get; set; }

        [JsonPropertyName("receipt_code")]
        public string ReceiptCode { get; set; } = string.Empty;

        [JsonPropertyName("retailer_id")]
        public string RetailerId { get; set; } = string.Empty;

        [JsonPropertyName("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("user_id_type")]
        public string UserIdType { get; set; } = string.Empty;
    }

    public class EventType
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("event_stream_event_category_id")]
        public int EventStreamEventCategoryId { get; set; }

        [JsonPropertyName("event_stream_stream_id")]
        public int EventStreamStreamId { get; set; }
    }
} 