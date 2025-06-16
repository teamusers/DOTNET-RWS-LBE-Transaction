using System.Net;

namespace RWS_LBE_Transaction.Exceptions
{
    public class ExternalApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string RawResponse { get; }

        public ExternalApiException(string message, HttpStatusCode statusCode, string rawResponse)
            : base(message)
        {
            StatusCode = statusCode;
            RawResponse = rawResponse;
        }
    }
}