using Microsoft.Extensions.Options;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.DTOs.Shared;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services.Interfaces;
using System.Text.Json;

namespace RWS_LBE_Transaction.Services.Implementations
{
    public class RlpServiceBooking : IRlpServiceBooking
    {
        private readonly RlpApiConfig _config;
        private readonly IApiHttpClient _apiHttpClient;
        private readonly ILogger<RlpServiceBooking> _logger;

        public RlpServiceBooking(IOptions<ExternalApiConfig> configOptions, IApiHttpClient apiHttpClient, ILogger<RlpServiceBooking> logger)
        {
            _config = configOptions.Value.RlpApiConfig
                ?? throw new ArgumentNullException(nameof(configOptions), "RlpApiConfig section is missing");
            _apiHttpClient = apiHttpClient;
            _logger = logger;
        }

        public async Task<BookingListResponse?> ViewBookingList(string? currentPage = null, string? ngid = null, string? status = null, string? type = null, string? pageSize = null)
        {
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(currentPage))
                queryParams.Add("currentPage", currentPage);
            if (!string.IsNullOrEmpty(ngid))
                queryParams.Add("ngid", ngid);
            if (!string.IsNullOrEmpty(status))
                queryParams.Add("status", status);
            if (!string.IsNullOrEmpty(type))
                queryParams.Add("type", type);
            if (!string.IsNullOrEmpty(pageSize))
                queryParams.Add("pageSize", pageSize);

            var url = BuildRlpBookingRequestInfo("/rlp/v1/booking/list", queryParams);

            // Generate CA signing headers
            var signingResult = await CaRequestSigner.SignAsync(
                "GET", 
                url, 
                _config.Booking!.ApiKey, 
                _config.Booking!.ApiSecret,
                _logger
            );

            var options = new ApiRequestOptions
            {
                Method = HttpMethod.Get,
                Url = url,
                Headers = signingResult.Headers,
                BypassSslValidation = true
            };

            return await _apiHttpClient.DoApiRequestAsync<BookingListResponse>(options);
        }

        public async Task<BookingDetailResponse?> ViewBookingDetail(string orderNo, string? ngid = null, string? status = null)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "orderNo", orderNo }
            };

            if (!string.IsNullOrEmpty(ngid))
                queryParams.Add("ngid", ngid);
            if (!string.IsNullOrEmpty(status))
                queryParams.Add("status", status);

            var url = BuildRlpBookingRequestInfo("/api/booking/detail", queryParams);

            // Generate CA signing headers
            var signingResult = await CaRequestSigner.SignAsync(
                "GET", 
                url, 
                _config.Booking!.ApiKey, 
                _config.Booking!.ApiSecret,
                _logger
            );

            var options = new ApiRequestOptions
            {
                Method = HttpMethod.Get,
                Url = url,
                Headers = signingResult.Headers,
                BypassSslValidation = true
            };

            return await _apiHttpClient.DoApiRequestAsync<BookingDetailResponse>(options);
        }

        private string BuildRlpBookingRequestInfo(string endpoint, Dictionary<string, string> queryParams)
        {
            var baseUrl = _config.Booking!.Host.TrimEnd('/');
            var endpointPath = endpoint.TrimStart('/');
            var fullUrl = $"{baseUrl}/{endpointPath}";

            if (queryParams.Any())
            {
                var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
                fullUrl += $"?{queryString}";
            }

            return fullUrl;
        }
    }
} 