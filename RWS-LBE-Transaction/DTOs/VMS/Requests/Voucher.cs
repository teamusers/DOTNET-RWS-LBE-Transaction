using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.DTOs.VMS.Requests
{
    public class GetVoucherTypesRequest
    {
        [JsonPropertyName("InterfaceRequestHeaderDT")]
        public InterfaceRequestHeaderDT InterfaceRequestHeaderDT { get; set; } = new InterfaceRequestHeaderDT();
    }
}