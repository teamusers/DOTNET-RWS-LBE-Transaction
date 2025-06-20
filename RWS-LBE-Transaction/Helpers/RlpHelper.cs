using System.Text.Json;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.RLP.Shared;

namespace RWS_LBE_Transaction.Helpers
{
    public class RlpHelper
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static ((string Username, string Password) BasicAuth, string Url) BuildRlpCoreRequestInfo(RlpApiConfig config, string basePath, string? externalId, string? queryParams)
        {
            string username = config.Core!.ApiKey;
            string password = config.Core!.ApiSecret;

            string url = BuildRlpUrl(config.Core!.Host, config.Core!.ApiKey, basePath, externalId, queryParams);

            return ((username, password), url);
        }

        public static ((string Username, string Password) BasicAuth, string Url) BuildRlpOffersRequestInfo(RlpApiConfig config, string basePath, string? externalId, string? queryParams)
        {
            string username = config.Offers!.ApiKey;
            string password = config.Offers!.ApiSecret;

            string url = BuildRlpUrl(config.Offers!.Host, config.Offers!.ApiKey, basePath, externalId, queryParams);

            return ((username, password), url);
        }

        public static ((string Username, string Password) BasicAuth, string Url) BuildRlpTransactionRequestInfo(RlpApiConfig config, string basePath, string? externalId, string? queryParams)
        {
            string username = config.Transaction!.ApiKey;
            string password = config.Transaction!.ApiSecret;

            string url = BuildRlpUrl(config.Transaction!.Host, config.Transaction!.ApiKey, basePath, externalId, queryParams);

            return ((username, password), url);
        } 

        public static string BuildRlpUrl(string host, string apiKey, string basePath, string? externalId, string? queryParams)
        {
            var endpoint = basePath
                .Replace(":api_key", apiKey)
                .Replace(":external_id", externalId);

            if (!string.IsNullOrWhiteSpace(queryParams))
            {
                endpoint = $"{endpoint}?{queryParams}";
            }

            return $"{host}{endpoint}";
        }

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