using System.Net;

namespace RWS_LBE_Transaction.DTOs.Shared
{
    public class ApiRequestOptions
    {
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public string Url { get; set; } = null!;
        public object? Body { get; set; }
        public string ContentType { get; set; } = "application/json";
        public string? BearerToken { get; set; }
        public (string Username, string Password)? BasicAuth { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public HttpStatusCode ExpectedStatus { get; set; } = HttpStatusCode.OK;
        public bool BypassSslValidation { get; set; } = false;
    }
}