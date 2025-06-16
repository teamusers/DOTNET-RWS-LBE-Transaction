using System.Text.Json;
using RWS_LBE_Transaction.DTOs.RLP.Shared;

namespace RWS_LBE_Transaction.Helpers
{
    public class RlpHelper
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static List<string> ExtractOfferIDsFromJsonElement(JsonElement jsonElement)
        {
            try
            {

                var campaigns = jsonElement.Deserialize<RlpCampaignList>(_jsonOptions);

                return ExtractOfferIDs(campaigns);
            }
            catch (JsonException)
            {
                return [];
            }
        }

        public static List<string> ExtractOfferIDs(RlpCampaignList? campaigns)
        {
            if (campaigns == null)
                return [];

            var tiles = campaigns.Tiles ?? [];
            var promotions = campaigns.Promotions ?? [];

            var offerIDs = tiles.Concat(promotions)
            .Where(c => c.CustomPayload != null &&
                !string.IsNullOrEmpty(c.CustomPayload.OfferId) &&
                !c.CustomPayload.OfferId.Equals("none", StringComparison.CurrentCultureIgnoreCase))
            .Select(c => c.CustomPayload!.OfferId!)
            .Distinct()
            .ToList();

            return offerIDs;
        }
    }
}