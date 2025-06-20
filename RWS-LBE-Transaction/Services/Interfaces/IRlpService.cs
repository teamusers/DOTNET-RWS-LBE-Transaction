using RWS_LBE_Transaction.DTOs.RLP.Responses;
using RWS_LBE_Transaction.DTOs.RLP.Requests;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IRlpService
    {
        // Campaigns
        Task<GetAllCampaignsResponse?> GetAllCampaigns(int page);
        Task<GetCampaignsByIdResponse?> GetCampaignsById(string externalId);

        // Offers
        Task<FetchOffersDetailsResponse?> FetchOffersDetails(List<string> offerIdList);
        Task RevokeOffer(string userOfferId, string reason);
        Task UpdateOffer(string externalId, string userOfferId, string systemTransactionId);

        // Transactions
        Task<UserTransactionResponse?> ViewTransaction(string externalId, string? event_types = null);
        Task<string?> ViewTransactionRaw(string externalId, string? event_types = null);

        Task<StoreTransactionsResponse?> ViewStoreTransaction(object payload);
        Task<string?> ViewStoreTransactionRaw(object payload);

        Task<UserPointResponse?> ViewPoint(string externalId);
        Task<string?> ViewPointRaw(string externalId);

        Task<SendTransactionResponse?> SendTransactionAsync(SendTransactionRWS payload);

        Task<UserBalanceResponse?> ViewAllBalancesAsync(string externalId, ViewBalanceRWS payload);

        Task<SpendResponse?> SpendMultipleTransactionsAsync(SpendRequest payload);
    }
}