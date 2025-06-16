using RWS_LBE_Transaction.DTOs.RLP.Responses;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IRlpService
    {
        Task<GetAllCampaignsResponse?> GetAllCampaigns(int page);

    }
}