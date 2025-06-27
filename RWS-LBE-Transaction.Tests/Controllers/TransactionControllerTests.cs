using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.Controllers;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.Requests;
using RWS_LBE_Transaction.DTOs.RLP.Requests;
using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.Exceptions;
using RWS_LBE_Transaction.Services.Interfaces;
using System.Net;
using Xunit;
using RequestTransactionPayment = RWS_LBE_Transaction.DTOs.Requests.TransactionPayment;

namespace RWS_LBE_Transaction.Tests.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<IRlpServiceTransaction> _mockRlpService;
        private readonly Mock<ILogger<TransactionController>> _mockLogger;
        private readonly Mock<ITransactionSequenceService> _mockTransactionSequenceService;
        private readonly Mock<IErrorHandler> _mockErrorHandler;
        private readonly Mock<IOptions<ExternalApiConfig>> _mockExternalApiConfig;
        private readonly TransactionController _controller;
        private readonly ExternalApiConfig _externalApiConfig;

        public TransactionControllerTests()
        {
            _mockRlpService = new Mock<IRlpServiceTransaction>();
            _mockLogger = new Mock<ILogger<TransactionController>>();
            _mockTransactionSequenceService = new Mock<ITransactionSequenceService>();
            _mockErrorHandler = new Mock<IErrorHandler>();
            _mockExternalApiConfig = new Mock<IOptions<ExternalApiConfig>>();

            _externalApiConfig = new ExternalApiConfig
            {
                RlpApiConfig = new RlpApiConfig
                {
                    RetailerId = "test-retailer-id"
                }
            };

            _mockExternalApiConfig.Setup(x => x.Value).Returns(_externalApiConfig);

            _controller = new TransactionController(
                _mockRlpService.Object,
                _mockLogger.Object,
                _mockTransactionSequenceService.Object,
                _mockErrorHandler.Object,
                _mockExternalApiConfig.Object);
        }

        #region GetTransaction Tests

        [Fact]
        public async Task GetTransaction_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var externalId = "RWS_2";
            var eventTypes = "earn,spend";
            var count = 10;
            var since = 1234567890;
            var expectedResponse = new UserTransactionResponse
            {
                Status = "success",
                Result = new List<Result>(),
                Count = 1,
                EventTypes = new List<EventType>()
            };

            _mockRlpService.Setup(x => x.ViewTransaction(externalId, eventTypes, count, since))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTransaction(externalId, eventTypes, count, since);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal(Codes.SUCCESSFUL, apiResponse.Code);
            Assert.Equal("successful", apiResponse.Message);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task GetTransaction_ServiceThrowsException_ReturnsErrorHandlerResult()
        {
            // Arrange
            var externalId = "RWS_2";
            var exception = new Exception("API Error");
            var expectedErrorResponse = new ConflictObjectResult(ResponseTemplate.ExistingUserNotFoundErrorResponse());

            _mockRlpService.Setup(x => x.ViewTransaction(externalId, null, null, null))
                .ThrowsAsync(exception);

            _mockErrorHandler.Setup(x => x.Handle(exception))
                .Returns(expectedErrorResponse);

            // Act
            var result = await _controller.GetTransaction(externalId);

            // Assert
            Assert.Same(expectedErrorResponse, result);
            _mockErrorHandler.Verify(x => x.Handle(exception), Times.Once);
        }

        #endregion

        #region GetStoreTransaction Tests

        [Fact]
        public async Task GetStoreTransaction_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var payload = new { storeId = "store-123", date = "2024-01-01" };
            var expectedResponse = new StoreTransactionsResponse
            {
                Status = "success",
                Payload = new GetStoreTransactionsResponse
                {
                    TotalRecords = 1,
                    Results = new List<PurchaseTransactionPayload>()
                }
            };

            _mockRlpService.Setup(x => x.ViewStoreTransaction(payload))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetStoreTransaction(payload);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal(Codes.SUCCESSFUL, apiResponse.Code);
            Assert.Equal("successful", apiResponse.Message);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task GetStoreTransaction_ServiceThrowsException_ReturnsErrorHandlerResult()
        {
            // Arrange
            var payload = new { storeId = "store-123" };
            var exception = new Exception("Service error");
            var expectedErrorResponse = new BadRequestObjectResult(ResponseTemplate.InternalErrorResponse());

            _mockRlpService.Setup(x => x.ViewStoreTransaction(payload))
                .ThrowsAsync(exception);

            _mockErrorHandler.Setup(x => x.Handle(exception))
                .Returns(expectedErrorResponse);

            // Act
            var result = await _controller.GetStoreTransaction(payload);

            // Assert
            Assert.Same(expectedErrorResponse, result);
            _mockErrorHandler.Verify(x => x.Handle(exception), Times.Once);
        }

        #endregion

        #region GetPoints Tests

        [Fact]
        public async Task GetPoints_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var externalId = "RWS_2";
            var expectedResponse = new UserPointResponse
            {
                Status = "success",
                User = new RlpUser()
            };

            _mockRlpService.Setup(x => x.ViewPoint(externalId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetPoints(externalId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal(Codes.SUCCESSFUL, apiResponse.Code);
            Assert.Equal("successful", apiResponse.Message);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task GetPoints_ServiceThrowsException_ReturnsErrorHandlerResult()
        {
            // Arrange
            var externalId = "RWS_2";
            var exception = new Exception("Service error");
            var expectedErrorResponse = new ObjectResult(ResponseTemplate.InternalErrorResponse()) { StatusCode = 500 };

            _mockRlpService.Setup(x => x.ViewPoint(externalId))
                .ThrowsAsync(exception);

            _mockErrorHandler.Setup(x => x.Handle(exception))
                .Returns(expectedErrorResponse);

            // Act
            var result = await _controller.GetPoints(externalId);

            // Assert
            Assert.Same(expectedErrorResponse, result);
            _mockErrorHandler.Verify(x => x.Handle(exception), Times.Once);
        }

        #endregion

        #region Earn Tests

        [Fact]
        public async Task Earn_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new SendTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    PosEmployeeId = "emp-123",
                    Subtotal = 100.0,
                    TaxTotal = 10.0,
                    OpenTime = DateTime.UtcNow,
                    ModifiedTime = DateTime.UtcNow,
                    Channel = "POS"
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var expectedResponse = new SendTransactionResponse
            {
                Status = "success",
                Payload = new { transactionId = transactionId }
            };

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.SendTransactionAsync(It.IsAny<SendTransactionRWS>()))
                .ReturnsAsync(expectedResponse);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "success", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Earn(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal(Codes.SUCCESSFUL, apiResponse.Code);
            Assert.Equal("successful", apiResponse.Message);
            
            var responseData = Assert.IsType<SendTransactionResponse>(apiResponse.Data);
            Assert.Equal(transactionId, responseData.TransactionId);
            
            _mockTransactionSequenceService.Verify(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRlpService.Verify(x => x.SendTransactionAsync(It.IsAny<SendTransactionRWS>()), Times.Once);
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "success", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Earn_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Earn(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(Codes.INVALID_REQUEST_BODY, apiResponse.Code);
        }

        [Fact]
        public async Task Earn_NullRequestPayload_ReturnsBadRequest()
        {
            // Arrange
            var request = new SendTransaction 
            { 
                StoreId = "store-123",
                RequestPayload = null! // Explicitly set to null
            };

            // Act
            var result = await _controller.Earn(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(Codes.INVALID_REQUEST_BODY, apiResponse.Code);
        }

        [Fact]
        public async Task Earn_NullStoreId_ReturnsBadRequest()
        {
            // Arrange
            var request = new SendTransaction
            {
                StoreId = null!, // Explicitly set to null
                RequestPayload = new SendTransactionRequestPayload()
            };

            // Act
            var result = await _controller.Earn(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(Codes.INVALID_REQUEST_BODY, apiResponse.Code);
        }

        [Fact]
        public async Task Earn_TransactionSequenceServiceThrowsException_ReturnsErrorHandlerResult()
        {
            // Arrange
            var request = new SendTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload()
            };

            var exception = new Exception("Database error");
            var expectedErrorResponse = new ObjectResult(ResponseTemplate.InternalErrorResponse()) { StatusCode = 500 };

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            _mockErrorHandler.Setup(x => x.Handle(exception))
                .Returns(expectedErrorResponse);

            // Act
            var result = await _controller.Earn(request);

            // Assert
            Assert.Same(expectedErrorResponse, result);
            _mockErrorHandler.Verify(x => x.Handle(exception), Times.Once);
        }

        [Fact]
        public async Task Earn_SendTransactionThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new SendTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload()
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var exception = new Exception("API error");

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.SendTransactionAsync(It.IsAny<SendTransactionRWS>()))
                .ThrowsAsync(exception);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_send_transaction_error", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Earn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.Equal(Codes.INTERNAL_ERROR, apiResponse.Code);
            
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_send_transaction_error", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Earn_UpdateStatusThrowsException_LogsErrorButReturnsSuccess()
        {
            // Arrange
            var request = new SendTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload()
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var expectedResponse = new SendTransactionResponse { Status = "success" };

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.SendTransactionAsync(It.IsAny<SendTransactionRWS>()))
                .ReturnsAsync(expectedResponse);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "success", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Update error"));

            // Act
            var result = await _controller.Earn(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Error updating transaction status to success")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        #endregion

        #region Burn Tests

        [Fact]
        public async Task Burn_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    OpenTime = DateTime.Parse("2025-05-23T03:05:38.2388782Z"),  
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 50.0,
                            Type = "points"
                            
                        }
                    }
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var spendId = "spend-account-123";
            var spendBalance = 100.0;

            var walletBalance = new UserBalanceResponse
            {
                Payload = new UserBalancePayload
                {
                    Details = new List<BalanceAccountDetail>
                    {
                        new BalanceAccountDetail
                        {
                            GroupingLabel = "Spend",
                            AccountName = "NG Dollar Point Account",
                            PointAccountId = spendId,
                            AvailableBalance = spendBalance
                        }
                    }
                }
            };

            var spendResponse = new SpendResponse { Status = "success" };
            var transactionResponse = new SendTransactionResponse { Status = "success" };

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.ViewAllBalancesAsync("user-123", It.IsAny<ViewBalanceRWS>()))
                .ReturnsAsync(walletBalance);

            _mockRlpService.Setup(x => x.SpendMultipleTransactionsAsync(It.IsAny<SpendRequest>()))
                .ReturnsAsync(spendResponse);

            _mockRlpService.Setup(x => x.SendTransactionAsync(It.IsAny<SendTransactionRWS>()))
                .ReturnsAsync(transactionResponse);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "success", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal(Codes.SUCCESSFUL, apiResponse.Code);
            Assert.Equal("successful", apiResponse.Message);
            
            var responseData = Assert.IsType<SendTransactionResponse>(apiResponse.Data);
            Assert.Equal(transactionId, responseData.TransactionId);
            Assert.Equal(request.RequestPayload.OpenTime, responseData.OpenTime);
        }

        [Fact]
        public async Task Burn_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Burn(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(Codes.INVALID_REQUEST_BODY, apiResponse.Code);
        }

        [Fact]
        public async Task Burn_NullRequestPayload_ReturnsBadRequest()
        {
            // Arrange
            var request = new BurnTransaction { StoreId = "store-123" };

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(Codes.INVALID_REQUEST_BODY, apiResponse.Code);
        }

        [Fact]
        public async Task Burn_EmptyPayments_ReturnsBadRequest()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>()
                }
            };

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(Codes.INVALID_REQUEST_BODY, apiResponse.Code);
        }

        [Fact]
        public async Task Burn_NoUserIdInPayments_ReturnsConflict()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            Amount = 50.0,
                            Type = "points"
                        }
                    }
                }
            };

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(conflictResult.Value);
            Assert.Equal(Codes.INVALID_REQUEST_BODY, apiResponse.Code);
        }

        [Fact]
        public async Task Burn_SubtotalLessThanOrEqualToZero_ReturnsConflict()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 0.0,
                            Type = "points"
                        }
                    }
                }
            };

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(conflictResult.Value);
            Assert.Equal(Codes.TRANSACTION_BURN_SUBTOTAL_LESS_THAN_OR_EQUAL_TO_ZERO, apiResponse.Code);
        }

        [Fact]
        public async Task Burn_TransactionSequenceServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 50.0,
                            Type = "points"
                        }
                    }
                }
            };

            var exception = new Exception("Database error");

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.Equal(Codes.INTERNAL_ERROR, apiResponse.Code);
        }

        [Fact]
        public async Task Burn_ViewAllBalancesThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 50.0,
                            Type = "points"
                        }
                    }
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var exception = new Exception("API error");

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.ViewAllBalancesAsync("user-123", It.IsAny<ViewBalanceRWS>()))
                .ThrowsAsync(exception);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Burn_NullWalletBalance_ReturnsInternalServerError()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 50.0,
                            Type = "points"
                        }
                    }
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.ViewAllBalancesAsync("user-123", It.IsAny<ViewBalanceRWS>()))
                .ReturnsAsync((UserBalanceResponse?)null);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Burn_NoSpendWalletFound_ReturnsInternalServerError()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 50.0,
                            Type = "points"
                        }
                    }
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;

            var walletBalance = new UserBalanceResponse
            {
                Payload = new UserBalancePayload
                {
                    Details = new List<BalanceAccountDetail>
                    {
                        new BalanceAccountDetail
                        {
                            GroupingLabel = "Earn",
                            AccountName = "Different Account",
                            PointAccountId = "different-account",
                            AvailableBalance = 100.0
                        }
                    }
                }
            };

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.ViewAllBalancesAsync("user-123", It.IsAny<ViewBalanceRWS>()))
                .ReturnsAsync(walletBalance);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_wallet_error", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Burn_InsufficientBalance_ReturnsInternalServerError()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 150.0, // More than available balance
                            Type = "points"
                        }
                    }
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var spendId = "spend-account-123";
            var spendBalance = 100.0; // Less than requested amount

            var walletBalance = new UserBalanceResponse
            {
                Payload = new UserBalancePayload
                {
                    Details = new List<BalanceAccountDetail>
                    {
                        new BalanceAccountDetail
                        {
                            GroupingLabel = "Spend",
                            AccountName = "NG Dollar Point Account",
                            PointAccountId = spendId,
                            AvailableBalance = spendBalance
                        }
                    }
                }
            };

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.ViewAllBalancesAsync("user-123", It.IsAny<ViewBalanceRWS>()))
                .ReturnsAsync(walletBalance);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_insufficient_balance", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_insufficient_balance", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Burn_SpendTransactionThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 50.0,
                            Type = "points"
                        }
                    }
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var spendId = "spend-account-123";
            var spendBalance = 100.0;

            var walletBalance = new UserBalanceResponse
            {
                Payload = new UserBalancePayload
                {
                    Details = new List<BalanceAccountDetail>
                    {
                        new BalanceAccountDetail
                        {
                            GroupingLabel = "Spend",
                            AccountName = "NG Dollar Point Account",
                            PointAccountId = spendId,
                            AvailableBalance = spendBalance
                        }
                    }
                }
            };

            var exception = new Exception("Spend API error");

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.ViewAllBalancesAsync("user-123", It.IsAny<ViewBalanceRWS>()))
                .ReturnsAsync(walletBalance);

            _mockRlpService.Setup(x => x.SpendMultipleTransactionsAsync(It.IsAny<SpendRequest>()))
                .ThrowsAsync(exception);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_spend_error", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_spend_error", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Burn_SendTransactionThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new BurnTransaction
            {
                StoreId = "store-123",
                RequestPayload = new SendTransactionRequestPayload
                {
                    Payments = new List<RequestTransactionPayment>
                    {
                        new RequestTransactionPayment
                        {
                            UserId = "user-123",
                            Amount = 50.0,
                            Type = "points"
                        }
                    }
                }
            };

            var transactionId = "1000000001";
            var recordId = 1L;
            var spendId = "spend-account-123";
            var spendBalance = 100.0;

            var walletBalance = new UserBalanceResponse
            {
                Payload = new UserBalancePayload
                {
                    Details = new List<BalanceAccountDetail>
                    {
                        new BalanceAccountDetail
                        {
                            GroupingLabel = "Spend",
                            AccountName = "NG Dollar Point Account",
                            PointAccountId = spendId,
                            AvailableBalance = spendBalance
                        }
                    }
                }
            };

            var spendResponse = new SpendResponse { Status = "success" };
            var exception = new Exception("Send transaction API error");

            _mockTransactionSequenceService.Setup(x => x.GetNextTransactionIDAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactionId, recordId));

            _mockRlpService.Setup(x => x.ViewAllBalancesAsync("user-123", It.IsAny<ViewBalanceRWS>()))
                .ReturnsAsync(walletBalance);

            _mockRlpService.Setup(x => x.SpendMultipleTransactionsAsync(It.IsAny<SpendRequest>()))
                .ReturnsAsync(spendResponse);

            _mockRlpService.Setup(x => x.SendTransactionAsync(It.IsAny<SendTransactionRWS>()))
                .ThrowsAsync(exception);

            _mockTransactionSequenceService.Setup(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_send_transaction_error", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Burn(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            _mockTransactionSequenceService.Verify(x => x.UpdateTransactionIDStatusAsync(recordId, "failed_send_transaction_error", It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
} 