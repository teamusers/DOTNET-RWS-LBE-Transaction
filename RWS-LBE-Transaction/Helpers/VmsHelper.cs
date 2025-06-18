using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RWS_LBE_Transaction.DTOs.Configurations;
using RWS_LBE_Transaction.DTOs.VMS.Shared;

namespace RWS_LBE_Transaction.Helpers
{
    public class VmsHelper
    {
        public static (Dictionary<string, string> SigHeaders, string Url) BuildVmsRequestInfo(VmsApiConfig config, HttpMethod method, string basePath, object? payload)
        {
            return (BuildSignatureHeaders(method, basePath, payload, config.Username, config.PresharedKey), $"{config.Host}{basePath}");
        }

        public static Dictionary<string, string> BuildSignatureHeaders(
        HttpMethod method,
        string path,
        object? requestBodyObject,
        string userName,
        string presharedKey)
        {
            var now = DateTime.UtcNow;
            var timestampNow = now.ToString("yyyyMMddHHmmssfff");
            var dateHeader = now.ToString("r"); // RFC1123 format
            var requestLine = $"{method.Method} {path} HTTP/1.1";

            var jsonBody = requestBodyObject != null
                ? JsonSerializer.Serialize(requestBodyObject)
                : "";

            var bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
            var base64Md5 = Convert.ToBase64String(MD5.HashData(bodyBytes));
            var contentLength = bodyBytes.Length;

            var contentType = "application/json";

            var sigHeaders = new Dictionary<string, string>
            {
                { "date", dateHeader },
                { "request-line", requestLine },
                { "content-type", contentType },
                { "content-md5", base64Md5 },
                { "payload-length", contentLength.ToString() }
            };

            var sigString = string.Join("\n", sigHeaders.Select(kvp =>
                kvp.Key == "request-line" ? kvp.Value : $"{kvp.Key.ToLowerInvariant()}: {kvp.Value}"
            ));

            var keyBytes = Encoding.UTF8.GetBytes(presharedKey);
            var sigBytes = new HMACSHA256(keyBytes).ComputeHash(Encoding.UTF8.GetBytes(sigString));
            var signatureBase64 = Convert.ToBase64String(sigBytes);

            var headersList = string.Join(" ", sigHeaders.Keys);
            var authHeader = $"hmac username=\"{userName}\",algorithm=\"hmac-sha256\",headers=\"{headersList}\",signature=\"{signatureBase64}\"";

            return new Dictionary<string, string>
            {
                { "Authorization", authHeader },
                { "Date", dateHeader },
                { "Content-MD5", base64Md5 },
                { "Payload-Length", contentLength.ToString() },
                { "Request-Id", $"rlc-int-{timestampNow}_{Guid.NewGuid()}" }
            };
        }

        public static bool ValidateVoucherType(List<VoucherType> validVoucherTypes, string voucherType)
        {
            if (validVoucherTypes.Any(v =>
                v.VoucherTypeCode != null && voucherType != null &&
                v.VoucherTypeCode == voucherType))
            {
                return true;
            }

            return false;
        }

        //TODO: update logic
        public static bool ValidateTransactionType(string transactionType)
        {
            if (transactionType == "validTransactionTypeCode")
            {
                return true;
            }

            return false;
        }
    }
}