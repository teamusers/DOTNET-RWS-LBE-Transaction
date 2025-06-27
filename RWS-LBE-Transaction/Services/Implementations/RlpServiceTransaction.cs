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
    public class RlpServiceTransaction : IRlpServiceTransaction
    {
        private readonly RlpApiConfig _config;
        private readonly IApiHttpClient _apiHttpClient;

        public RlpServiceTransaction(IOptions<ExternalApiConfig> configOptions, IApiHttpClient apiHttpClient)
        {
            _config = configOptions.Value.RlpApiConfig
                ?? throw new ArgumentNullException(nameof(configOptions), "RlpApiConfig section is missing");
            _apiHttpClient = apiHttpClient;
        }

        public async Task<UserTransactionResponse?> ViewTransaction(string externalId, string? event_types = null, int? count = null, int? since = null)
        {
            var queryParams = new List<string>();
            
            if (!string.IsNullOrEmpty(event_types))
            {
                queryParams.Add($"event_types={event_types}");
            }
            
            if (count.HasValue)
            {
                queryParams.Add($"count={count.Value}");
            }
            
            if (since.HasValue)
            {
                queryParams.Add($"since={since.Value}");
            }
            
            string? query = queryParams.Count > 0 ? string.Join("&", queryParams) : null;

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
    }
}