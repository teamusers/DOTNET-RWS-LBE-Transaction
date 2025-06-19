using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Services.Interfaces;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.DTOs.Requests;
using RWS_LBE_Transaction.DTOs.RLP.Requests;
using System;
using System.Threading.Tasks;
using RWS_LBE_Transaction.DTOs.Configurations;
using Microsoft.Extensions.Options;

namespace RWS_LBE_Transaction.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly IRlpService _rlpService;
        private readonly ITransactionSequenceService _transactionSequenceService;
        private readonly ExternalApiConfig _externalApiConfig;

        public TransactionController(
            IRlpService rlpService,
            ILogger<TransactionController> logger,
            ITransactionSequenceService transactionSequenceService,
            IOptions<ExternalApiConfig> externalApiConfig)
        {
            _rlpService = rlpService;
            _logger = logger;
            _transactionSequenceService = transactionSequenceService;
            _externalApiConfig = externalApiConfig.Value;
        }

        [HttpGet("user/{externalId}")]
        public async Task<IActionResult> GetTransaction(string externalId)
        {
            try
            {
                var transaction = await _rlpService.ViewTransaction(externalId);
                return Ok(ResponseTemplate.GenericSuccessResponse(transaction));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get all user transaction.");
                return RlpApiErrors.Handle(ex);
            }
        }

        [HttpPost("store")]
        public async Task<IActionResult> GetStoreTransaction([FromBody] object payload)
        {
            try
            {
                var result = await _rlpService.ViewStoreTransactionAsync(payload);
                return Ok(ResponseTemplate.GenericSuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get all store transactions.");
                return RlpApiErrors.Handle(ex);
            }
        }

        [HttpGet("point/{externalId}")]
        public async Task<IActionResult> GetPoints(string externalId)
        {
            try
            {
                var response = await _rlpService.ViewPoint(externalId);
                return Ok(ResponseTemplate.GenericSuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get user points.");
                return RlpApiErrors.Handle(ex);
            }
        }

        [HttpPost("earn")]
        public async Task<IActionResult> Earn([FromBody] SendTransaction req)
        {
            if (req == null || req.RequestPayload == null || req.StoreId == null)
                return BadRequest(ResponseTemplate.InvalidRequestBodyErrorResponse());

            string transactionId;
            long recordId;
            try
            {
                (transactionId, recordId) = await _transactionSequenceService.GetNextTransactionIDAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction ID record");
                return StatusCode(500, ResponseTemplate.InternalErrorResponse());
            }

            // Set the generated transaction ID in the payload
            req.RequestPayload.TransactionId = transactionId;

            // Generate a request ID (GUID)
            var generatedRequestId = Guid.NewGuid().ToString();

            // Get retailer ID from config
            var retailerId = _externalApiConfig.RlpApiConfig?.RetailerId ?? throw new InvalidOperationException("RetailerId not configured");

            var sendTransactionPayload = new SendTransactionRWS
            {
                ClientId = retailerId,
                RequestId = generatedRequestId,
                StoreId = req.StoreId,
                Culture = "en_US",
                RequestPayload = req.RequestPayload
            };

            SendTransactionResponse? transactionResponse = null;
            try
            {
                transactionResponse = await _rlpService.SendTransactionAsync(sendTransactionPayload);
            }
            catch (Exception ex)
            {
                await _transactionSequenceService.UpdateTransactionIDStatusAsync(recordId, "failed_send_transaction_error");
                _logger.LogError(ex, "Error sending transaction");
                return StatusCode(500, ResponseTemplate.InternalErrorResponse());
            }

            try
            {
                await _transactionSequenceService.UpdateTransactionIDStatusAsync(recordId, "success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction status to success");
            }

            if (transactionResponse != null)
                transactionResponse.TransactionId = transactionId;

            return StatusCode(201, ResponseTemplate.GenericSuccessResponse(transactionResponse));
        }
    }
} 