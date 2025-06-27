using RWS_LBE_Transaction.DTOs.RLP.Responses;
using System.Threading.Tasks;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IRlpServiceBooking
    {
        Task<BookingListResponse?> ViewBookingList(string? currentPage = null, string? ngid = null, string? status = null, string? type = null, string? pageSize = null);
        Task<BookingDetailResponse?> ViewBookingDetail(string orderNo, string? ngid = null, string? status = null);
    }
} 