using RWS_LBE_Transaction.DTOs.VMS.Responses;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IVmsService
    {
        Task<GetVoucherTypesResponse?> GetVoucherTypes();
    }
}