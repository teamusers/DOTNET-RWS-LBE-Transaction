using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.DTOs.VMS.Requests
{
    public class GetVoucherTypesRequest
    {
        [JsonPropertyName("InterfaceRequestHeaderDT")]
        public InterfaceRequestHeaderDT InterfaceRequestHeaderDT { get; set; } = new InterfaceRequestHeaderDT();
    }

    public class IssueVoucherRequest
    {
        [JsonPropertyName("InterfaceRequestHeaderDT")]
        public InterfaceRequestHeaderDT InterfaceRequestHeaderDT { get; set; } = new InterfaceRequestHeaderDT();

        [JsonPropertyName("VoucherRequestParamDT")]
        public VoucherRequestParamDT VoucherRequestParamDT { get; set; } = new VoucherRequestParamDT();

        [JsonPropertyName("VoucherIssuanceParamDT")]
        public List<VoucherIssuanceParamDT>? VoucherIssuanceParamDT { get; set; }
    }
}