using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.DTOs.VMS.Responses
{
    public class GetVoucherTypesResponse
    {
        [JsonPropertyName("InterfaceResponseHeaderDT")]
        public InterfaceResponseHeaderDT InterfaceResponseHeaderDT { get; set; } = new InterfaceResponseHeaderDT();

        [JsonPropertyName("VoucherTypeDT")]
        public List<VoucherType> VoucherTypeDT { get; set; } = [];
    }
}