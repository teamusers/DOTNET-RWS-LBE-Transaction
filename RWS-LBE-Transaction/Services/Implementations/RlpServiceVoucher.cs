using System.Text.Json;
using Microsoft.Extensions.Options;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.RLP.Requests;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services.Interfaces;

namespace RWS_LBE_Transaction.Services.Implementations
{
    public class RlpServiceVoucher : IRlpServiceVoucher
    {
        private readonly RlpApiConfig _config;
        private readonly IApiHttpClient _apiHttpClient;

        public RlpServiceVoucher(IOptions<ExternalApiConfig> configOptions, IApiHttpClient apiHttpClient)
        {
            _config = configOptions.Value.RlpApiConfig
                ?? throw new ArgumentNullException(nameof(configOptions), "RlpApiConfig section is missing");
            _apiHttpClient = apiHttpClient;
        }

        public async Task RevokeOffer(string userOfferId, string reason)
        {
            var payload = new RevokeOfferRequest
            {
                RetailerId = _config.RetailerId,
                Reason = reason,
                UserOfferId = userOfferId
            };

            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(_config, RlpApiEndpoints.RevokeOffer, null, null);

            await _apiHttpClient.DoApiRequestAsync<object>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task UpdateOffer(string externalId, string userOfferId, string systemTransactionId)
        {
            var payload = new UpdateOfferRequest
            {
                RetailerId = _config.RetailerId,
                UserId = externalId,
                UserOfferId = userOfferId,
                CustomData = JsonSerializer.Serialize(new
                {
                    system_transaction_id = systemTransactionId
                })
            };

            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(_config, RlpApiEndpoints.UpdateOffer, null, null);

            await _apiHttpClient.DoApiRequestAsync<object>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task<IssueOfferResponse?> IssueOffer(string externalId, string offerId)
        {
            var payload = new IssueOfferRequest
            {
                RetailerId = _config.RetailerId,
                UserId = externalId,
                OfferId = offerId
            };

            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(_config, RlpApiEndpoints.IssueOffer, null, null);

            return await _apiHttpClient.DoApiRequestAsync<IssueOfferResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task ManualRedeemOffer(string userOfferId)
        {
            var payload = new ManualRedeemOfferRequest
            {
                RetailerId = _config.RetailerId,
                UserOfferId = userOfferId
            };

            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(_config, RlpApiEndpoints.ManualRedeemOffer, null, null);

            await _apiHttpClient.DoApiRequestAsync<object>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }
    }
} 