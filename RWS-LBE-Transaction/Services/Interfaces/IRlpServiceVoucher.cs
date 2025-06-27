using RWS_LBE_Transaction.DTOs.RLP.Responses;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IRlpServiceVoucher
    {
        Task RevokeOffer(string userOfferId, string reason);
        Task UpdateOffer(string externalId, string userOfferId, string systemTransactionId);
        Task<IssueOfferResponse?> IssueOffer(string externalId, string offerId);
        Task ManualRedeemOffer(string userOfferId);
    }
} 