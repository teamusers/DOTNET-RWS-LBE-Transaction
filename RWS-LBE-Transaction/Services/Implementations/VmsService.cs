using Microsoft.Extensions.Options;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.VMS.Requests;
using RWS_LBE_Transaction.DTOs.VMS.Responses;
using RWS_LBE_Transaction.DTOs.VMS.Shared;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services.Interfaces;

namespace RWS_LBE_Transaction.Services.Implementations
{
    public class VmsService : IVmsService
    {
        private readonly VmsApiConfig _config;
        private readonly IApiHttpClient _apiHttpClient;

        public VmsService(IOptions<ExternalApiConfig> configOptions, IApiHttpClient apiHttpClient)
        {
            _config = configOptions.Value.VmsApiConfig
                ?? throw new ArgumentNullException(nameof(configOptions), "VmsApiConfig section is missing");
            _apiHttpClient = apiHttpClient;
        }

        public async Task<GetVoucherTypesResponse?> GetVoucherTypes()
        {
            var payload = new GetVoucherTypesRequest
            {
                InterfaceRequestHeaderDT = { SystemID = _config.SystemID }
            };

            var (sigHeaders, url) = VmsHelper.BuildVmsRequestInfo(_config, HttpMethod.Post, VmsApiEndpoints.GetVoucherType, payload);

            return await _apiHttpClient.DoApiRequestAsync<GetVoucherTypesResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = sigHeaders,
                Body = payload
            });
        }

        public async Task<IssueVoucherResponse?> IssueVoucher(VoucherIssuanceParamDT voucher)
        {
            var payload = new IssueVoucherRequest
            {
                InterfaceRequestHeaderDT = { SystemID = _config.SystemID },
                VoucherRequestParamDT = { IsBatchProcess = false },
                VoucherIssuanceParamDT = [voucher]
            };

            var (sigHeaders, url) = VmsHelper.BuildVmsRequestInfo(_config, HttpMethod.Post, VmsApiEndpoints.IssueVoucher, payload);

            return await _apiHttpClient.DoApiRequestAsync<IssueVoucherResponse>(new DTOs.Shared.ApiRequestOptions
            {
                Method = HttpMethod.Post,
                Url = url,
                Headers = sigHeaders,
                Body = payload
            });
        }
    }
}