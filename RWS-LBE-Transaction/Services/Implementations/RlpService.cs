using System.Text.Json;
using Microsoft.Extensions.Options;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.RLP.Requests;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.DTOs.Shared;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services.Interfaces;
using System.Net.Http;

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

        public async Task<UserTransactionResponse?> ViewTransaction(string externalId, string? event_types = null)
        {
            string? query = null;
            if (!string.IsNullOrEmpty(event_types))
            {
                query = $"event_types={event_types}";
            }

            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(
                _config,
                RlpApiEndpoints.ViewTransaction,
                externalId,
                query);

            return await _apiHttpClient.DoApiRequestAsync<UserTransactionResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth
            });
        }


        public async Task<UserPointResponse?> ViewPoint(string externalId)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(
                _config,
                RlpApiEndpoints.ViewPoint,
                externalId,
                "user[user_profile]=true&expand_incentives=true&show_identifiers=true");

            return await _apiHttpClient.DoApiRequestAsync<UserPointResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Method = HttpMethod.Get,
                Url = url,
                BasicAuth = basicAuth
            });
        }

        public async Task<SendTransactionResponse?> SendTransactionAsync(SendTransactionRWS payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpTransactionRequestInfo(
                _config,
                RlpApiEndpoints.SendTransaction,
                null,
                null);

            return await _apiHttpClient.DoApiRequestAsync<SendTransactionResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task<UserBalanceResponse?> ViewAllBalancesAsync(string externalId, ViewBalanceRWS payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(
                _config,
                RlpApiEndpoints.ViewAllBalances,
                externalId,
                null);

            return await _apiHttpClient.DoApiRequestAsync<UserBalanceResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task<SpendResponse?> SpendMultipleTransactionsAsync(SpendRequest payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(
                _config,
                RlpApiEndpoints.SpendMultipleTransactions,
                null,
                null);

            return await _apiHttpClient.DoApiRequestAsync<SpendResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task<StoreTransactionsResponse?> ViewStoreTransaction(object payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(
                _config,
                RlpApiEndpoints.ViewStoreTransaction,
                null,
                null);

            return await _apiHttpClient.DoApiRequestAsync<StoreTransactionsResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task<object?> ViewTransactionRaw(string externalId, string? event_types = null)
        {
            string? query = null;
            if (!string.IsNullOrEmpty(event_types))
            {
                query = $"event_types={event_types}";
            }

            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(
                _config,
                RlpApiEndpoints.ViewTransaction,
                externalId,
                query);

            Console.WriteLine($"URL: {url}");

            return await _apiHttpClient.DoApiRequestAsync<object>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth
            });
        }

        public async Task<object?> ViewStoreTransactionRaw(object payload)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpOffersRequestInfo(
                _config,
                RlpApiEndpoints.ViewStoreTransaction,
                null,
                null);

            return await _apiHttpClient.DoApiRequestAsync<object>(new DTOs.Shared.ApiRequestOptions
            {
                Url = url,
                BasicAuth = basicAuth,
                Method = HttpMethod.Post,
                Body = payload
            });
        }

        public async Task<object?> ViewPointRaw(string externalId)
        {
            var (basicAuth, url) = RlpHelper.BuildRlpCoreRequestInfo(
                _config,
                RlpApiEndpoints.ViewPoint,
                externalId,
                "user[user_profile]=true&expand_incentives=true&show_identifiers=true");

            return await _apiHttpClient.DoApiRequestAsync<object>(new DTOs.Shared.ApiRequestOptions
            {
                Method = HttpMethod.Get,
                Url = url,
                BasicAuth = basicAuth
            });
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
    }
}