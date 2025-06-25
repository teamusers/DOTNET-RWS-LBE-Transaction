using System;
using System.Collections.Generic;
using System.Text.Json;
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
        public List<PurchaseTransactionPayload>? Results { get; set; }

        [JsonPropertyName("pagination_token")]
        public string? PaginationToken { get; set; }
    }

    public sealed class PurchaseTransactionPayload
    {
        // ────────────────── basic meta ──────────────────
        [JsonPropertyName("id")]                     public Guid   Id                 { get; init; }
        [JsonPropertyName("retailer_id")]            public Guid   RetailerId         { get; init; }
        [JsonPropertyName("store_id")]               public Guid   StoreId            { get; init; }
        [JsonPropertyName("pos_transaction_id")]     public string PosTransactionId   { get; init; } = default!;
        [JsonPropertyName("pos_employee_id")]        public string? PosEmployeeId     { get; init; }
        [JsonPropertyName("table_id")]               public string? TableId           { get; init; }
        [JsonPropertyName("guest_count")]            public int    GuestCount         { get; init; }
        [JsonPropertyName("is_voided")]              public bool   IsVoided           { get; init; }
        [JsonPropertyName("is_closed")]              public bool   IsClosed           { get; init; }

        // ────────────────── money ──────────────────
        [JsonPropertyName("subtotal")]               public decimal Subtotal          { get; init; }
        [JsonPropertyName("taxtotal")]               public decimal TaxTotal          { get; init; }
        [JsonPropertyName("check_amount")]           public decimal CheckAmount       { get; init; }

        // ────────────────── misc header fields ──────────────────
        [JsonPropertyName("guest_receipt_code")]     public string GuestReceiptCode   { get; init; } = default!;
        [JsonPropertyName("channel")]                public string Channel            { get; init; } = default!;
        [JsonPropertyName("store_time_zone")]        public string StoreTimeZone      { get; init; } = default!;
        [JsonPropertyName("include_tax")]            public bool   IncludeTax         { get; init; }

        // ────────────────── important timestamps ──────────────────
        [JsonPropertyName("sm_transaction_process_date")]  public DateTime SmProcessDate { get; init; }
        [JsonPropertyName("open_time")]                     public DateTime OpenTime      { get; init; }
        [JsonPropertyName("modified_time")]                 public DateTime ModifiedTime  { get; init; }
        [JsonPropertyName("create_date")]                   public DateTime CreateDate    { get; init; }
        [JsonPropertyName("last_updated")]                  public DateTime LastUpdated   { get; init; }

        // ────────────────── core collections ──────────────────
        [JsonPropertyName("items")]     public List<TransactionItem>    Items     { get; init; } = new();
        [JsonPropertyName("payments")]  public List<TransactionPayment> Payments  { get; init; } = new();

        [JsonPropertyName("discounts")]              public JsonElement Discounts            { get; init; }
        [JsonPropertyName("business_def_collection")]public JsonElement BusinessDefCollection{ get; init; }

        [JsonPropertyName("catalog_lineages")]       public JsonElement CatalogLineages      { get; init; }
        [JsonPropertyName("location_lineages")]      public JsonElement LocationLineages     { get; init; }
        [JsonPropertyName("location_lineages_core")] public JsonElement LocationLineagesCore { get; init; }
    }

    // ──────────────────────────────── children ────────────────────────────────
    public sealed class TransactionItem
    {
        [JsonPropertyName("id")]              public Guid     Id              { get; init; }
        [JsonPropertyName("transaction_id")]  public Guid     TransactionId   { get; init; }
        [JsonPropertyName("master_item_id")]  public Guid     MasterItemId    { get; init; }
        [JsonPropertyName("pos_item_key")]    public string   PosItemKey      { get; init; } = default!;
        [JsonPropertyName("quantity")]        public decimal  Quantity        { get; init; }
        [JsonPropertyName("unit_price")]      public decimal  UnitPrice       { get; init; }
        [JsonPropertyName("subtotal")]        public decimal  Subtotal        { get; init; }
        [JsonPropertyName("tax_included")]    public decimal  TaxIncluded     { get; init; }
        [JsonPropertyName("retailer_id")]     public Guid     RetailerId      { get; init; }
        [JsonPropertyName("store_id")]        public Guid     StoreId         { get; init; }
        [JsonPropertyName("line_id")]         public string   LineId          { get; init; } = default!;
        [JsonPropertyName("create_date")]     public DateTime CreateDate      { get; init; }
        [JsonPropertyName("last_updated")]    public DateTime LastUpdated     { get; init; }
    }

    public sealed class TransactionPayment
    {
        [JsonPropertyName("id")]              public Guid     Id            { get; init; }
        [JsonPropertyName("transaction_id")]  public Guid     TransactionId { get; init; }
        [JsonPropertyName("payment_id")]      public string   PaymentId     { get; init; } = default!;
        [JsonPropertyName("type")]            public string   Type          { get; init; } = default!;
        [JsonPropertyName("amount")]          public decimal  Amount        { get; init; }
        [JsonPropertyName("payment_time")]    public DateTime PaymentTime   { get; init; }
        [JsonPropertyName("retailer_id")]     public Guid     RetailerId    { get; init; }
        [JsonPropertyName("store_id")]        public Guid     StoreId       { get; init; }
        [JsonPropertyName("receipt_code")]    public string   ReceiptCode   { get; init; } = default!;
        [JsonPropertyName("user_id")]         public string   UserId        { get; init; } = default!;
        [JsonPropertyName("user_id_type")]    public string   UserIdType    { get; init; } = default!;
        [JsonPropertyName("create_date")]     public DateTime CreateDate    { get; init; }
        [JsonPropertyName("last_updated")]    public DateTime LastUpdated   { get; init; }
    }
} 