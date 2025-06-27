using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public sealed class BookingListResponse
    {
        [JsonPropertyName("resultCode")] public string ResultCode { get; init; } = default!;
        [JsonPropertyName("resultMsg")]  public string ResultMsg { get; init; } = default!;
        [JsonPropertyName("result")]     public BookingResult? Result { get; init; }
    }

    public sealed class BookingResult
    {
        [JsonPropertyName("bookings")] public List<BookingEntry> Bookings { get; init; } = new();
    }

    public sealed class BookingEntry
    {
        [JsonPropertyName("orderNo")]   public string OrderNo { get; init; } = default!;
        [JsonPropertyName("productNo")] public string ProductNo { get; init; } = default!;
        [JsonPropertyName("specMap")]   public SpecMap SpecMap { get; init; } = new();
    }

    public sealed class SpecMap
    {
        [JsonPropertyName("gstAmount")]              public string GstAmount { get; init; } = default!;
        [JsonPropertyName("HotelCode")]              public string HotelCode { get; init; } = default!;
        [JsonPropertyName("CheckOutTime")]           public string CheckOutTime { get; init; } = default!;
        [JsonPropertyName("ItemCode")]               public string ItemCodeRaw { get; init; } = default!; // optional: parse into class
        [JsonPropertyName("NumOfRoom")]              public string NumOfRoom { get; init; } = default!;
        [JsonPropertyName("HotelArriveTime")]        public string HotelArriveTime { get; init; } = default!;
        [JsonPropertyName("HotelDepartTime")]        public string HotelDepartTime { get; init; } = default!;
        [JsonPropertyName("OfferName")]              public string OfferName { get; init; } = default!;
        [JsonPropertyName("serviceCharge")]          public string ServiceCharge { get; init; } = default!;
        [JsonPropertyName("RateCode")]               public string RateCode { get; init; } = default!;
        [JsonPropertyName("RoomType")]               public string RoomType { get; init; } = default!;
        [JsonPropertyName("MAKE_PAYMENT_RESERVATION_ID")] public string MakePaymentReservationId { get; init; } = default!;
        [JsonPropertyName("RoomTypeDescription")]    public string RoomTypeDescription { get; init; } = default!;
        [JsonPropertyName("PromotionFlag")]          public string PromotionFlag { get; init; } = default!;
        [JsonPropertyName("NumOfAdult")]             public string NumOfAdult { get; init; } = default!;
        [JsonPropertyName("HotelName")]              public string HotelName { get; init; } = default!;
        [JsonPropertyName("PackageCodes")]           public string PackageCodesRaw { get; init; } = default!; // usually []
        [JsonPropertyName("CheckInTime")]            public string CheckInTime { get; init; } = default!;
        [JsonPropertyName("NumOfChild")]             public string NumOfChild { get; init; } = default!;
        [JsonPropertyName("BookingNumber")]          public string BookingNumber { get; init; } = default!;
    }
}
