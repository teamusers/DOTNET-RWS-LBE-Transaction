using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.DTOs.VMS.Responses
{
    public class GetVoucherTypesResponse
    {
        [JsonPropertyName("InterfaceResponseHeaderDT")]
        public InterfaceResponseHeaderDT InterfaceResponseHeaderDT { get; set; } = new InterfaceResponseHeaderDT();

        [JsonPropertyName("VoucherTypeDT")]
        public List<VoucherTypeDT> VoucherTypeDT { get; set; } = [];
    }

    public class IssueVoucherResponse
    {
        [JsonPropertyName("InterfaceResponseHeaderDT")]
        public InterfaceResponseHeaderDT InterfaceResponseHeaderDT { get; set; } = new InterfaceResponseHeaderDT();

        [JsonPropertyName("VoucherIssuanceInfoDT")]
        public List<VoucherIssuanceInfoDT>? VoucherIssuanceInfoDT { get; set; }
    }

    public class EnquireVoucherResponse
    {
        [JsonPropertyName("InterfaceResponseHeaderDT")]
        public InterfaceResponseHeaderDT InterfaceResponseHeaderDT { get; set; } = new InterfaceResponseHeaderDT();

        [JsonPropertyName("VoucherEnquiryInfoDT")]
        public List<VoucherEnquiryInfoDT>? VoucherEnquiryInfoDT { get; set; }
    }

    public class UtilizeVoucherResponse
    {
        [JsonPropertyName("InterfaceResponseHeaderDT")]
        public InterfaceResponseHeaderDT InterfaceResponseHeaderDT { get; set; } = new InterfaceResponseHeaderDT();

        [JsonPropertyName("VoucherUtilizationInfoDT")]
        public List<VoucherUtilizationInfoDT>? VoucherUtilizationInfoDT { get; set; }
    }
}