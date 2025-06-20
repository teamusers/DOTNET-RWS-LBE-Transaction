using System.Text.Json.Serialization;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.DTOs.Requests
{
    public class SendVoucherIssuanceRequest
    {
        [JsonPropertyName("rlp_id")]
        public required string RlpId { get; set; }

        [JsonPropertyName("voucher_issuance_param")]
        public required VoucherIssuanceParamDT VoucherIssuanceParamDT { get; set; }
    }
}