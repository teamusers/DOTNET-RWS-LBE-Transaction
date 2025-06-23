using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.DTOs.VMS.Shared;
using RWS_LBE_Transaction.Exceptions;

namespace RWS_LBE_Transaction.Common
{
    public class VmsApiEndpoints
    {
        public const string GetVoucherType = "/vms/GetVoucherType";
        public const string IssueVoucher = "/vms/IssueVoucher";
        public const string EnquireVoucher = "/vms/EnquireVoucher";
        public const string UtilizeVoucher = "/vms/UtilizeVoucher";
    }

    public class TransactionTypeCodes
    {
        public const string ITD = "I";
        public const string NON_ITD = "N";
    }

    public class VmsApiErrors
    {
        public static IActionResult Handle(Exception ex)
        {
            if (ex is ExternalApiException { RawResponse: not null } apiEx)
            {
                if (ApiException.TryParseJson<VmsErrorResponse>(apiEx.RawResponse, out var errResp))
                {
                    return new BadRequestObjectResult(ResponseTemplate.UnmappedVmsErrorResponse(errResp?.InterfaceResponseHeaderDT));
                }

                return new BadRequestObjectResult(ResponseTemplate.UnmappedVmsErrorResponse(null));
            }

            return new ObjectResult(ResponseTemplate.InternalErrorResponse()) { StatusCode = 500 };
        }
    }
}