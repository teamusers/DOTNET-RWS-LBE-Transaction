using Microsoft.AspNetCore.Mvc;
using RWS_LBE_Transaction.Common;
using RWS_LBE_Transaction.Services.Interfaces;

namespace RWS_LBE_Transaction.Services.Implementations
{
    public class ErrorHandler : IErrorHandler
    {
        public IActionResult Handle(Exception ex)
        {
            return RlpApiErrors.Handle(ex);
        }
    }
} 