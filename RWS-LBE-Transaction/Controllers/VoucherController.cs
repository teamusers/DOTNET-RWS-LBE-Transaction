using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<IActionResult> SendVoucherIssuance([FromBody] SendVoucherIssuanceRequest req)
        {
            //TODO: add request body validator logic

            VoucherIssuanceParamDT voucher = req.VoucherIssuanceParamDT;
            List<VoucherTypeDT> voucherTypeList;

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
                return BadRequest(ResponseTemplate.VmsGetVoucherTypeCodesErrorResponse());
            }

            // Verify Voucher Type Code and Transaction Type Code
            RevokeOfferReason? revokeReason = null;

            if (!VmsHelper.ValidateVoucherType(voucherTypeList, voucher.VoucherTypeCode!))
            {
                revokeReason = RlpRevokeOfferReasons.INVALID_VOUCHER_TYPE_CODE;
            }
            else if (!VmsHelper.ValidateTransactionType(voucher.TransactionTypeCode!))
            {
                revokeReason = RlpRevokeOfferReasons.INVALID_TRANSACTION_TYPE_CODE;
            }

            if (revokeReason != null)
            {
                // revoke voucher
                try
                {
                    await _rlp.RevokeOffer(voucher.VoucherNo!, revokeReason.Description!);
                    return BadRequest(ResponseTemplate.SendVoucherIssuanceErrorResponse(revokeReason, null));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to revoke voucher.");
                    return BadRequest(ResponseTemplate.InvalidVoucherIssuanceRevokeErrorResponse());
                }
            }

            // Generate new systemTransactionId

            voucher.SystemTransactionID = Guid.NewGuid().ToString();

            // Issue Voucher on VMS (Retry up to 3 Times if timeout)
            voucher.TerminalCode = VmsEnums.DEFAULT_TERMINAL_CODE;

            int retryCount = 0;
            const int maxRetries = 3;
            InterfaceResponseHeaderDT? interfaceResponseHeaderDT = null;

            while (true)
            {
                try
                {
                    var issueVoucherResponse = await _vms.IssueVoucher(voucher);
                    interfaceResponseHeaderDT = issueVoucherResponse?.InterfaceResponseHeaderDT;

                    // 3001 = Duplicate Voucher Number Error, assume voucher was successfully issued into VMS in previous timed out request
                    if (interfaceResponseHeaderDT?.FaultCodeID == 3001)
                    {
                        break;
                    }
                    else
                    {
                        revokeReason = RlpRevokeOfferReasons.VMS_ERROR;
                        break;
                    }
                }
                catch (TaskCanceledException)
                {
                    if (retryCount >= maxRetries)
                    {
                        _logger.LogError("[API EXCEPTION] VMS: Voucher issuance for {VoucherNo} timed out. Retry count exceeded.", voucher.VoucherNo);
                        revokeReason = RlpRevokeOfferReasons.VMS_TIMEOUT;
                        break;
                    }

                    retryCount++;
                    _logger.LogInformation("[API RETRY] VMS: Request timed out when issuing voucher {VoucherNo}. Retry count: {RetryCount}", voucher.VoucherNo, retryCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[API EXCEPTION] VMS: Failed to issue voucher {VoucherNo}.", voucher.VoucherNo);
                    revokeReason = RlpRevokeOfferReasons.VMS_ERROR;
                    break;
                }
            }

            if (revokeReason != null)
            {
                // revoke voucher
                try
                {
                    await _rlp.RevokeOffer(voucher.VoucherNo!, revokeReason.Description!);
                    return BadRequest(ResponseTemplate.SendVoucherIssuanceErrorResponse(revokeReason, interfaceResponseHeaderDT));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to revoke voucher.");
                    return BadRequest(ResponseTemplate.InvalidVoucherIssuanceRevokeErrorResponse());
                }
            }

            // if no error, update offer in RLP
            try
            {
                await _rlp.UpdateOffer(req.RlpId, voucher.VoucherNo!, voucher.SystemTransactionID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to update voucher in RLP after issuance.");
                return BadRequest(ResponseTemplate.ValidVoucherIssuanceUpdateErrorResponse());
            }

            return Ok(ResponseTemplate.GenericSuccessResponse(null));
        }

        [HttpPost("issue/trigger")]
        public async Task<IActionResult> ManualVoucherIssuance([FromBody] ManualVoucherIssuanceRequest req)
        {
            //TODO: add request body validator logic

            VoucherIssuanceParamDT voucher = req.VoucherIssuanceParamDT;

            // Issue offer to User via RLP

            try
            {
                var issueOfferResponse = await _rlp.IssueOffer(req.RlpId, req.OfferId);
                voucher.VoucherNo = issueOfferResponse?.Payload?.StatusList?[0].UserOfferId;

                if (voucher.VoucherNo.IsNullOrEmpty())
                {
                    _logger.LogError("[API EXCEPTION] RLP: Offer issued but user_offer_id not found.");
                    return BadRequest(ResponseTemplate.UnmappedRlpErrorResponse(null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to issue offer.");
                return RlpApiErrors.Handle(ex);
            }

            // Generate new systemTransactionId

            voucher.SystemTransactionID = Guid.NewGuid().ToString();

            // issue voucher to vms
            voucher.TerminalCode = VmsEnums.DEFAULT_TERMINAL_CODE;
            
            try
            {
                var issueVoucherResponse = await _vms.IssueVoucher(voucher);
                //  Issue Voucher to VMS
                InterfaceResponseHeaderDT? interfaceResponseHeaderDT = issueVoucherResponse?.InterfaceResponseHeaderDT;
                if (interfaceResponseHeaderDT?.FaultCodeID != 0)
                {
                    return BadRequest(ResponseTemplate.UnmappedVmsErrorResponse(interfaceResponseHeaderDT));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] VMS: Failed to issue voucher {VoucherNo}.", voucher.VoucherNo);
                return VmsApiErrors.Handle(ex);
            }

            // if no error, update offer in RLP
            try
            {
                await _rlp.UpdateOffer(req.RlpId, voucher.VoucherNo!, voucher.SystemTransactionID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to update voucher in RLP after issuance.");
                return BadRequest(ResponseTemplate.ValidVoucherIssuanceUpdateErrorResponse());
            }

            return Ok(ResponseTemplate.GenericSuccessResponse(null));
        }

        [HttpPost("utilise")]
        public async Task<IActionResult> UtiliseVoucher([FromBody] UtiliseVoucherRequest req)
        {
            // enquire voucher validity

            try
            {
                var enquireVoucherResponse = await _vms.EnquireVoucher(req.VoucherNo);

                if (enquireVoucherResponse?.InterfaceResponseHeaderDT.FaultCodeID != 0)
                {
                    _logger.LogError("[API EXCEPTION] VMS: Failed to enquire voucher {VoucherNo}.", req.VoucherNo);
                    return BadRequest(ResponseTemplate.UnmappedVmsErrorResponse(enquireVoucherResponse?.InterfaceResponseHeaderDT));
                }

                if (enquireVoucherResponse?.VoucherEnquiryInfoDT?[0].VoucherStatus != "V")
                {
                    _logger.LogError("[API EXCEPTION] VMS: Voucher {VoucherNo} status is invalid.", req.VoucherNo);
                    return Conflict(ResponseTemplate.VoucherStatusInvalidErrorResponse(enquireVoucherResponse?.VoucherEnquiryInfoDT?[0]));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] VMS: Failed to enquire voucher {VoucherNo}.", req.VoucherNo);
                return VmsApiErrors.Handle(ex);
            }

            // utilise voucher
            try
            {
                var voucher = new VoucherUtilizationParamDT
                {
                    SystemTransactionID = Guid.NewGuid().ToString(),
                    TerminalCode = VmsEnums.DEFAULT_TERMINAL_CODE,
                    VoucherNo = req.VoucherNo,
                    UtilizeDateTime = req.UtilizeDateTime
                };

                var utilizeVoucherResponse = await _vms.UtilizeVoucher(voucher);

                if (utilizeVoucherResponse?.InterfaceResponseHeaderDT.FaultCodeID != 0)
                {
                    _logger.LogError("[API EXCEPTION] VMS: Failed to utilize voucher {VoucherNo}.", req.VoucherNo);
                    return BadRequest(ResponseTemplate.UnmappedVmsErrorResponse(utilizeVoucherResponse?.InterfaceResponseHeaderDT));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] VMS: Failed to utilize voucher {VoucherNo}.", req.VoucherNo);
                return VmsApiErrors.Handle(ex);
            }

            return Ok(ResponseTemplate.GenericSuccessResponse(null));
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncVoucher([FromBody] SyncVoucherRequest req)
        {
            try
            {
                await _rlp.ManualRedeemOffer(req.VoucherNo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to sync voucher redemption status.");
                return RlpApiErrors.Handle(ex);
            }

            return Ok(ResponseTemplate.GenericSuccessResponse(null));
        }
    }
}