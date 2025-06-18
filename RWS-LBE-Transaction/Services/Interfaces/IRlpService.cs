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
        void RevokeOffer(object payload);

        // Transactions
        Task<UserTransactionResponse?> ViewTransaction(string externalId);

        Task<StoreTransactionsResponse> ViewStoreTransactionAsync(object payload);
    }
}