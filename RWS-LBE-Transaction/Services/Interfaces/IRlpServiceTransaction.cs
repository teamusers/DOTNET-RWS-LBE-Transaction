using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.DTOs.RLP.Requests;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IRlpServiceTransaction
    {
        Task<UserTransactionResponse?> ViewTransaction(string externalId, string? event_types = null, int? count = null, int? since = null);
        Task<StoreTransactionsResponse?> ViewStoreTransaction(object payload);
        Task<UserPointResponse?> ViewPoint(string externalId);
        Task<SendTransactionResponse?> SendTransactionAsync(SendTransactionRWS payload);
        Task<UserBalanceResponse?> ViewAllBalancesAsync(string externalId, ViewBalanceRWS payload);
        Task<SpendResponse?> SpendMultipleTransactionsAsync(SpendRequest payload);
    }
}