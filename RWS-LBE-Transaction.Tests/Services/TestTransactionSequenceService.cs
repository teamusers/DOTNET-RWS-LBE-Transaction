using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RWS_LBE_Transaction.Data;
using RWS_LBE_Transaction.Models;
using System.Data;
using RWS_LBE_Transaction.Services.Implementations;

namespace RWS_LBE_Transaction.Tests.Services
{
    /// <summary>
    /// Test-specific version of TransactionSequenceService that works with in-memory database
    /// </summary>
    public class TestTransactionSequenceService : RWS_LBE_Transaction.Services.Implementations.TransactionSequenceService
    {
        public TestTransactionSequenceService(AppDbContext dbContext, ILogger<TransactionSequenceService> logger, IConfiguration configuration)
            : base(dbContext, logger, configuration)
        {
        }

        /// <summary>
        /// Override to use traditional update instead of ExecuteUpdateAsync for in-memory database compatibility
        /// </summary>
        public override async Task UpdateTransactionIDStatusAsync(long recordId, string status, CancellationToken cancellationToken = default)
        {
            // _dbContext and _logger are protected in the base class for testability
            var record = await _dbContext.TransactionIDRecords.FindAsync(recordId, cancellationToken);
            
            if (record == null)
            {
                _logger.LogWarning("No transaction record found with ID: {RecordId}", recordId);
                throw new InvalidOperationException("Transaction record not found");
            }

            record.Status = status;
            record.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated transaction record {RecordId} with status: {Status}", recordId, status);
        }
    }
} 