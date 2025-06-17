using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.RLP.Shared;

namespace RWS_LBE_Transaction.DTOs.RLP.Responses
{
    public class FetchOffersDetailsResponse
    {
        [JsonPropertyName("payload")]
        public OfferDetailsPayload? Payload { get; set; }
    }

    public class OfferDetailsPayload
    {
        [JsonPropertyName("results")]
        public List<Offer>? Results { get; set; }
    }
}