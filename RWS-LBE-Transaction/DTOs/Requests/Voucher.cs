using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.DTOs.Requests
{
    public class IssueVoucherRequest
    {
        [JsonPropertyName("VoucherIssuanceParamDT")]
        public required VoucherIssuanceParam VoucherIssuanceParamDT { get; set; }
    }
}