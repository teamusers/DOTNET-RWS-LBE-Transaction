using Microsoft.Extensions.Options;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.RLP.Requests;
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
            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(_config, RlpApiEndpoints.GetAllCampaigns, null, query);

            return await _apiHttpClient.DoApiRequestAsync<GetAllCampaignsResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth   
            });
        }

        public async Task<GetCampaignsByIdResponse?> GetCampaignsById(string externalId)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(_config, RlpApiEndpoints.GetCampaignsById, externalId, null);

            return await _apiHttpClient.DoApiRequestAsync<GetCampaignsByIdResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth
            });
        }

        public async Task<FetchOffersDetailsResponse?> FetchOffersDetails(List<string> offerIdList)
        {
            var payload = new FetchOffersDetailsRequest
            {
                RetailerId = _config.RetailerId,
                OfferIds = offerIdList,
                Skip = 0,
                Take = 1000
            };

            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(_config, RlpApiEndpoints.FetchOffersDetails, null, null);

            return await _apiHttpClient.DoApiRequestAsync<FetchOffersDetailsResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }
    }
}