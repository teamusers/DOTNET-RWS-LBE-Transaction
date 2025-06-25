using Microsoft.AspNetCore.Mvc;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IErrorHandler
    {
        IActionResult Handle(Exception ex);
    }
} 