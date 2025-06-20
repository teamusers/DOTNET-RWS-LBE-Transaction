using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RWS_LBE_Transaction.Data;
using RWS_LBE_Transaction.Models;
using RWS_LBE_Transaction.Services.Interfaces;
using System.Data;

namespace RWS_LBE_Transaction.Services.Implementations
{
    public class TransactionSequenceService : ITransactionSequenceService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TransactionSequenceService> _logger;
        private readonly IConfiguration _configuration;
        private int MaxAttempts => GetMaxAttemptsFromConfig();

        public TransactionSequenceService(AppDbContext dbContext, ILogger<TransactionSequenceService> logger, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
        }

        private int GetMaxAttemptsFromConfig()
        {
            var configValue = _configuration["rlpTransactionIDGenerateMaxAttempts"];
            if (int.TryParse(configValue, out var value) && value > 0)
                return value;
            return 3;
        }

        public async Task<(string TransactionId, long RecordId)> CreateTransactionIDRecordAsync(CancellationToken cancellationToken = default)
        {
            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    return await TryCreateTransactionIDRecordAsync(attempt, cancellationToken);
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    if (attempt == MaxAttempts)
                    {
                        _logger.LogError(ex, "Failed to create transaction ID record after {MaxAttempts} attempts", MaxAttempts);
                        throw;
                    }
                    
                    _logger.LogWarning("Unique constraint violation on attempt {Attempt}, retrying...", attempt);
                    await Task.Delay(100 * attempt, cancellationToken); // Exponential backoff
                }
            }

            throw new InvalidOperationException("Failed to create transaction ID record after maximum attempts");
        }

        private async Task<(string TransactionId, long RecordId)> TryCreateTransactionIDRecordAsync(int attempt, CancellationToken cancellationToken = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            
            try
            {
                // Get the next transaction number
                var nextTransactionNumber = await GetNextTransactionNumberAsync(cancellationToken);
                
                // Create the transaction ID from the number
                var transactionId = (1000000000 + nextTransactionNumber).ToString();
                
                // Create the record with both transaction ID and number
                var record = new TransactionIDRecord
                {
                    TransactionId = transactionId,
                    TransactionNumber = nextTransactionNumber,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.TransactionIDRecords.Add(record);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Created transaction ID: {TransactionId} (number: {TransactionNumber}, record ID: {RecordId}, attempt: {Attempt})", 
                    transactionId, nextTransactionNumber, record.Id, attempt);
                return (transactionId, record.Id);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Gets the next transaction number by finding the maximum existing transaction number + 1
        /// </summary>
        private async Task<long> GetNextTransactionNumberAsync(CancellationToken cancellationToken = default)
        {
            var maxTransactionNumber = await _dbContext.TransactionIDRecords
                .MaxAsync(r => (long?)r.TransactionNumber, cancellationToken) ?? 0;
            
            return maxTransactionNumber + 1;
        }

        /// <summary>
        /// Checks if the exception is a unique constraint violation
        /// </summary>
        private bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            // Check for SQL Server unique constraint violation
            return ex.InnerException?.Message?.Contains("duplicate key") == true ||
                   ex.InnerException?.Message?.Contains("UNIQUE constraint") == true ||
                   ex.InnerException?.Message?.Contains("Cannot insert duplicate key") == true;
        }


        public async Task UpdateTransactionIDStatusAsync(long recordId, string status, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.TransactionIDRecords
                .Where(r => r.Id == recordId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.Status, status)
                    .SetProperty(r => r.UpdatedAt, DateTime.UtcNow), 
                    cancellationToken);

            if (result == 0)
            {
                _logger.LogWarning("No transaction record found with ID: {RecordId}", recordId);
                throw new InvalidOperationException("Transaction record not found");
            }

            _logger.LogInformation("Updated transaction record {RecordId} with status: {Status}", recordId, status);
        }

        public async Task<(string TransactionId, long RecordId)> GetNextTransactionIDAsync(CancellationToken cancellationToken = default)
        {
            return await CreateTransactionIDRecordAsync(cancellationToken);
        }
    }
} 