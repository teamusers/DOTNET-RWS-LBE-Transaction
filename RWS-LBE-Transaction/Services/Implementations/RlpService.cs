using Microsoft.Extensions.Options;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services.Interfaces;

namespace RWS_LBE_Transaction.Services.Implementations
{
    public class RlpService : IRlpService
    {
        private readonly RlpApiConfig _config;
        private readonly IApiHttpClient _apiHttpClient;

        public RlpService(IOptions<ExternalApiConfig> configOptions, IApiHttpClient apiHttpClient)
        {
            _config = configOptions.Value.RlpApiConfig
                ?? throw new ArgumentNullException(nameof(configOptions), "RlpApiConfig section is missing");
            _apiHttpClient = apiHttpClient;
        }

        public async Task<GetAllCampaignsResponse?> GetAllCampaigns(int page)
        {
            string query = $"{RlpApiQueries.CampaignsQuery}&page={page}";
            var (basicAuth, url) = BuildRlpCoreRequestInfo(RlpApiEndpoints.GetAllCampaigns, null, query);

            return await _apiHttpClient.DoApiRequestAsync<GetAllCampaignsResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth   
            });
        }

        public ((string Username, string Password) BasicAuth, string Url) BuildRlpCoreRequestInfo(string basePath, string? externalId, string? queryParams)
        {
            string username = _config.Core!.ApiKey;
            string password = _config.Core!.ApiSecret;

            string url = BuildRlpUrl(_config.Core!.Host, _config.Core!.ApiKey, basePath, externalId, queryParams);

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
    }
}