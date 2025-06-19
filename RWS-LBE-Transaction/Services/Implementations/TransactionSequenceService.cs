using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RWS_LBE_Transaction.Data;
using RWS_LBE_Transaction.Models;
using RWS_LBE_Transaction.Services.Interfaces;

namespace RWS_LBE_Transaction.Services.Implementations
{
    public class TransactionSequenceService : ITransactionSequenceService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TransactionSequenceService> _logger;

        public TransactionSequenceService(AppDbContext dbContext, ILogger<TransactionSequenceService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<(string TransactionId, long RecordId)> CreateTransactionIDRecordAsync(CancellationToken cancellationToken = default)
        {
            var record = new TransactionIDRecord
            {
                Status = "pending"
            };

            _dbContext.TransactionIDRecords.Add(record);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var transactionId = (999999999 + record.Id).ToString();

            record.TransactionId = transactionId;
            record.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created transaction ID: {TransactionId} (record ID: {RecordId})", transactionId, record.Id);
            return (transactionId, record.Id);
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