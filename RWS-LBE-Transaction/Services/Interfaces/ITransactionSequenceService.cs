namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface ITransactionSequenceService
    {
        Task<(string TransactionId, long RecordId)> CreateTransactionIDRecordAsync(CancellationToken cancellationToken = default);
        Task UpdateTransactionIDStatusAsync(long recordId, string status, CancellationToken cancellationToken = default);
        Task<(string TransactionId, long RecordId)> GetNextTransactionIDAsync(CancellationToken cancellationToken = default);
    }
} 