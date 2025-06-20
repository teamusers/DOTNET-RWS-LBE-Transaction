using RWS_LBE_Transaction.DTOs.VMS.Responses;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IVmsService
    {
        Task<GetVoucherTypesResponse?> GetVoucherTypes();

        Task<IssueVoucherResponse?> IssueVoucher(VoucherIssuanceParamDT voucher);
    }
}