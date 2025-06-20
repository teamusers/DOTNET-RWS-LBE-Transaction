using RWS_LBE_Transaction.DTOs.RLP.Responses;

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
        Task<UserTransactionResponse?> ViewTransaction(string externalId);
    }
}