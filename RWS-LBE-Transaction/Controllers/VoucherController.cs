using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Requests;
using RWS_LBE_Transaction.DTOs.VMS.Shared;
using RWS_LBE_Transaction.Helpers;
using RWS_LBE_Transaction.Services.Interfaces;

namespace RWS_LBE_Transaction.Controllers
{
    [ApiController]
    [Route("api/v1/voucher")]
    public class VoucherController : ControllerBase
    {
        private readonly ILogger<CampaignController> _logger;
        private readonly IRlpService _rlp;
        private readonly IVmsService _vms;
        public VoucherController(ILogger<CampaignController> logger, IRlpService rlpService, IVmsService vmsService)
        {
            _logger = logger;
            _rlp = rlpService;
            _vms = vmsService;
        }

        [HttpPost("issue")]
        public async Task<IActionResult> IssueVoucher([FromBody] IssueVoucherRequest req)
        {
            List<VoucherType> voucherTypeList;

            try
            {
                var voucherTypesResponse = await _vms.GetVoucherTypes();

                if (voucherTypesResponse!.VoucherTypeDT.Count != 0)
                {
                    voucherTypeList = voucherTypesResponse!.VoucherTypeDT;
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] VMS: Failed to get voucher types.");
                return StatusCode(500, ResponseTemplate.InternalErrorResponse());
            }

            return Ok(ResponseTemplate.GenericSuccessResponse(voucherTypeList));

            // // Verify Voucher Type

            // //TODO: remove mock ------------------------------------------------------
            // voucherTypeList = [
            //     new VoucherType
            //     {
            //         VoucherTypeCode = "AT_ACW_SEA",
            //         VoucherTypeDescription = "Attraction - ACW and SEAA Ticketing Counter"
            //     },
            //     new VoucherType
            //     {
            //         VoucherTypeCode = "AT_ALL",
            //         VoucherTypeDescription = "Attraction - All"
            //     }
            // ];
            // //TODO: remove mock ------------------------------------------------------

            // if (!VmsHelper.ValidateVoucherType(voucherTypeList, req.VoucherIssuanceParamDT.VoucherTypeCode!) ||
            //     !VmsHelper.ValidateTransactionType(req.VoucherIssuanceParamDT.TransactionTypeCode!))
            // {
            //     // revoke voucher

            // }


            // return Ok(ResponseTemplate.GenericSuccessResponse(null));
        }
    }
}