using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Services.Interfaces;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.DTOs.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace RWS_LBE_Transaction.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IRlpServiceBooking _rlpService;
        private readonly ExternalApiConfig _externalApiConfig;

        public BookingController(
            IRlpServiceBooking rlpService,
            ILogger<BookingController> logger,
            IOptions<ExternalApiConfig> externalApiConfig)
        {
            _rlpService = rlpService;
            _logger = logger;
            _externalApiConfig = externalApiConfig.Value;
        }

        [HttpGet("bme/rlp/bookings/list")]
        public async Task<IActionResult> GetBookingList([FromQuery] string? currentPage = null, [FromQuery] string? ngid = null, [FromQuery] string? status = null, [FromQuery] string? type = null, [FromQuery] string? pageSize = null)
        {
            try
            {
                var response = await _rlpService.ViewBookingList(currentPage, ngid, status, type, pageSize);
                return Ok(ResponseTemplate.GenericSuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get booking list.");
                return RlpApiErrors.Handle(ex);
            }
        }

        [HttpGet("bme/rlp/bookings/detail")]
        public async Task<IActionResult> GetBookingDetail([FromQuery] string orderNo, [FromQuery] string? ngid = null, [FromQuery] string? status = null)
        {
            try
            {
                var response = await _rlpService.ViewBookingDetail(orderNo, ngid, status);
                return Ok(ResponseTemplate.GenericSuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[API EXCEPTION] RLP: Failed to get booking details.");
                return RlpApiErrors.Handle(ex);
            }
        }
    }
} 