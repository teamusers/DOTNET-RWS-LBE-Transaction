using RWS_LBE_Transaction.DTOs.Auth.Requests;

namespace RWS_LBE_Transaction.Services.Interfaces
{
    public interface IAuthService
    {
        AuthRequest GenerateSignature(string appId, string secretKey);
        AuthRequest GenerateSignatureWithParams(
            string appId,
            string nonce,
            string timestamp,
            string secretKey);
    }
}