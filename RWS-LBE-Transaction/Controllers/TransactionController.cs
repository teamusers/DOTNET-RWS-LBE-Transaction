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
using System.Collections.Generic;

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
        public async Task<IActionResult> GetTransaction(string externalId, [FromQuery] string? event_types = null, [FromQuery] int? count = null, [FromQuery] int? since = null)
        {
            try
            {
                var response = await _rlpService.ViewTransaction(externalId, event_types, count, since);
                return Ok(ResponseTemplate.GenericSuccessResponse(response));
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
                var response = await _rlpService.ViewStoreTransaction(payload);

                return Ok(ResponseTemplate.GenericSuccessResponse(response));
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
                return RlpApiErrors.Handle(ex);
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
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
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

            return Ok(ResponseTemplate.GenericSuccessResponse(transactionResponse));
        }

        [HttpPost("burn")]
        public async Task<IActionResult> Burn([FromBody] BurnTransaction req)
        {
            if (req == null || req.RequestPayload == null || req.RequestPayload.Payments == null || req.RequestPayload.Payments.Count == 0)
                return BadRequest(ResponseTemplate.InvalidRequestBodyErrorResponse());

            // Extract user ID and amount from payment details
            string? userId = null;
            double subtotal = 0;
            foreach (var payment in req.RequestPayload.Payments)
            {
                if (!string.IsNullOrEmpty(payment.UserId))
                {
                    userId = payment.UserId;
                    subtotal = payment.Amount;
                    break;
                }
            }

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No user ID found in payment details");
                return Conflict(ResponseTemplate.InvalidRequestBodyErrorResponse());
            }
            if (subtotal <= 0)
            {
                _logger.LogWarning("Invalid subtotal amount");
                return Conflict(ResponseTemplate.BurnSubtotalLessThanOrEqualToZeroErrorResponse());
            }

            // Generate transaction ID
            string transactionId;
            long recordId;
            try
            {
                (transactionId, recordId) = await _transactionSequenceService.GetNextTransactionIDAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction ID record");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
            }

            // Get wallet balance and find spend wallet
            var viewBalancePayload = new ViewBalanceRWS
            {
                RetailerId = _externalApiConfig.RlpApiConfig?.RetailerId ?? throw new InvalidOperationException("RetailerId not configured"),
                UserId = userId,
                Culture = "en_US"
            };

            UserBalanceResponse? walletBalance;
            try
            {
                walletBalance = await _rlpService.ViewAllBalancesAsync(userId, viewBalancePayload);
            }
            catch (Exception ex)
            {
                await _transactionSequenceService.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error");
                _logger.LogError(ex, "Error viewing wallet balance");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
            }

            if (walletBalance?.Payload?.Details == null)
            {
                await _transactionSequenceService.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
            }

            // Find the spend wallet
            string? spendId = null;
            double spendBalance = 0;
            foreach (var detail in walletBalance.Payload.Details)
            {
                if (string.Equals(detail.GroupingLabel, "Spend", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(detail.AccountName, "NG Dollar Point Account", StringComparison.OrdinalIgnoreCase))
                {
                    spendId = detail.PointAccountId;
                    spendBalance = detail.AvailableBalance;
                    break;
                }
            }

            if (string.IsNullOrEmpty(spendId))
            {
                await _transactionSequenceService.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
            }

            // Check if user has enough balance
            if (spendBalance - subtotal < 0)
            {
                await _transactionSequenceService.UpdateTransactionIDStatusAsync(recordId, "failed_insufficient_balance");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
            }

            // Generate IDs for spend operation
            var generatedIdempotencyId = Guid.NewGuid().ToString();
            var generatedReferenceId = Guid.NewGuid().ToString();

            // Spend the points
            var spendRequest = new SpendRequest
            {
                Culture = "en-US",
                RetailerId = _externalApiConfig.RlpApiConfig?.RetailerId ?? throw new InvalidOperationException("RetailerId not configured"),
                UserId = userId,
                SpendDetails = new List<SpendUserPointsRequestDetail>
                {
                    new()
                    {
                        Amount = subtotal,
                        PointAccountIds = new List<string> { spendId },
                        ReferenceId = generatedReferenceId,
                        ReferenceType = "AppSpend",
                        TransactionId = transactionId,
                        ForceSpend = true,
                        Rank = 0,
                        IsReturn = false,
                        IdempotencyId = generatedIdempotencyId
                    }
                },
                AllowPartialSuccess = true,
                DisableEventPublishing = false,
                ParallelMode = false
            };

            SpendResponse? spendResponse;
            try
            {
                spendResponse = await _rlpService.SpendMultipleTransactionsAsync(spendRequest);
            }
            catch (Exception ex)
            {
                await _transactionSequenceService.UpdateTransactionIDStatusAsync(recordId, "failed_spend_error");
                _logger.LogError(ex, "Error spending points");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
            }

            // TODO: Process spendResponse for later processing if needed

            // Set the generated transaction ID in the payload
            req.RequestPayload.TransactionId = transactionId;

            // Send the transaction (same as earn)
            var generatedRequestId = Guid.NewGuid().ToString();
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
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseTemplate.InternalErrorResponse());
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

            return Ok(ResponseTemplate.GenericSuccessResponse(transactionResponse));
        }
    }
} 