using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public sealed class BookingDetailResponse
    {
        [JsonPropertyName("resultCode")] public string ResultCode { get; init; } = default!;
        [JsonPropertyName("resultMsg")]  public string ResultMsg { get; init; } = default!;
        [JsonPropertyName("result")]     public BookingDetailResult? Result { get; init; }
    }

    public sealed class BookingDetailResult
    {
        [JsonPropertyName("bookingDetail")] public BookingDetail? BookingDetail { get; init; }
    }

    public sealed class BookingDetail
    {
        [JsonPropertyName("bookingNo")]           public string BookingNo { get; init; } = default!;
        [JsonPropertyName("offers")]              public List<BookingOffer> Offers { get; init; } = new();
        [JsonPropertyName("patronInfo")]          public PatronInfo? PatronInfo { get; init; }
        [JsonPropertyName("comment")]             public string? Comment { get; init; }
        [JsonPropertyName("createTime")]          public string? CreateTime { get; init; }
        [JsonPropertyName("payTime")]             public string? PayTime { get; init; }
        [JsonPropertyName("cancelTime")]          public string? CancelTime { get; init; }
        [JsonPropertyName("status")]              public string? Status { get; init; }
        [JsonPropertyName("failedReason")]        public string? FailedReason { get; init; }
        [JsonPropertyName("totalPrice")]          public double? TotalPrice { get; init; }
        [JsonPropertyName("salesChannel")]        public string? SalesChannel { get; init; }
        [JsonPropertyName("voucherAmt")]          public double? VoucherAmt { get; init; }
        [JsonPropertyName("netAmt")]              public double? NetAmt { get; init; }
        [JsonPropertyName("voucherCode")]         public string? VoucherCode { get; init; }
        [JsonPropertyName("paymentMode")]         public string? PaymentMode { get; init; }
        [JsonPropertyName("totalTaxAmount")]      public double? TotalTaxAmount { get; init; }
        [JsonPropertyName("paymentNo")]           public string? PaymentNo { get; init; }
        [JsonPropertyName("additionalMetadata")]  public object? AdditionalMetadata { get; init; }
        [JsonPropertyName("paymentCardNumber")]   public string? PaymentCardNumber { get; init; }
        [JsonPropertyName("errorCode")]           public string? ErrorCode { get; init; }
        [JsonPropertyName("errorMessage")]        public string? ErrorMessage { get; init; }
    }

    public sealed class BookingOffer
    {
        [JsonPropertyName("priceRuleId")] public int PriceRuleId { get; init; }
        [JsonPropertyName("offerCount")]  public int OfferCount { get; init; }
        [JsonPropertyName("offerNo")]     public string OfferNo { get; init; } = default!;
        [JsonPropertyName("offerId")]     public string OfferId { get; init; } = default!;
        [JsonPropertyName("hotel")]       public HotelDetail? Hotel { get; init; }
        [JsonPropertyName("hotels")]      public List<HotelDetail> Hotels { get; init; } = new();
        [JsonPropertyName("attraction")]  public List<object> Attraction { get; init; } = new();
        [JsonPropertyName("paymentType")] public string? PaymentType { get; init; }
    }

    public sealed class HotelDetail
    {
        [JsonPropertyName("offerId")]            public int? OfferId { get; init; }
        [JsonPropertyName("offerNo")]            public string? OfferNo { get; init; }
        [JsonPropertyName("prodId")]             public int? ProdId { get; init; }
        [JsonPropertyName("prodNo")]             public string? ProdNo { get; init; }
        [JsonPropertyName("hotelCode")]          public string? HotelCode { get; init; }
        [JsonPropertyName("hotelName")]          public string? HotelName { get; init; }
        [JsonPropertyName("roomCategoryName")]   public string? RoomCategoryName { get; init; }
        [JsonPropertyName("numOfRoom")]          public int? NumOfRoom { get; init; }
        [JsonPropertyName("numOfAdult")]         public int? NumOfAdult { get; init; }
        [JsonPropertyName("numOfChild")]         public int? NumOfChild { get; init; }
        [JsonPropertyName("numOfSenior")]        public int? NumOfSenior { get; init; }
        [JsonPropertyName("checkInDate")]        public string? CheckInDate { get; init; }
        [JsonPropertyName("checkOutDate")]       public string? CheckOutDate { get; init; }
        [JsonPropertyName("checkInTime")]        public string? CheckInTime { get; init; }
        [JsonPropertyName("checkOutTime")]       public string? CheckOutTime { get; init; }
        [JsonPropertyName("arrivalTime")]        public string? ArrivalTime { get; init; }
        [JsonPropertyName("departureTime")]      public string? DepartureTime { get; init; }
        [JsonPropertyName("items")]              public List<object> Items { get; init; } = new();
        [JsonPropertyName("bookingNumber")]      public string? BookingNumber { get; init; }
        [JsonPropertyName("withBreakfast")]      public bool? WithBreakfast { get; init; }
        [JsonPropertyName("comment")]            public string? Comment { get; init; }
        [JsonPropertyName("patronInfo")]         public PatronInfo? PatronInfo { get; init; }
        [JsonPropertyName("nightsNo")]           public int? NightsNo { get; init; }
        [JsonPropertyName("status")]             public string? Status { get; init; }
        [JsonPropertyName("grossAmt")]           public double? GrossAmt { get; init; }
        [JsonPropertyName("voucherAmt")]         public double? VoucherAmt { get; init; }
        [JsonPropertyName("netAmt")]             public double? NetAmt { get; init; }
        [JsonPropertyName("voucherCode")]        public string? VoucherCode { get; init; }
        [JsonPropertyName("roomType")]           public string? RoomType { get; init; }
        [JsonPropertyName("rateCode")]           public string? RateCode { get; init; }
        [JsonPropertyName("noshow")]             public bool? NoShow { get; init; }
    }

    public sealed class PatronInfo
    {
        [JsonPropertyName("firstName")]         public string? FirstName { get; init; }
        [JsonPropertyName("lastName")]          public string? LastName { get; init; }
        [JsonPropertyName("phoneNo")]           public string? PhoneNo { get; init; }
    }
}