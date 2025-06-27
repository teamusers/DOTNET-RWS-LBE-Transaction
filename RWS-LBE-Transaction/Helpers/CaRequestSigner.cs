using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RWS_LBE_Transaction.Helpers
{
    public static class CaRequestSigner
    {
        private static readonly string[] ExcludedHeaders =
        {
            "x-ca-signature", "x-ca-signature-headers", "x-ca-key", "x-ca-nonce"
        };

        public static string CreateUuid()
        {
            byte[] bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            bytes[6] = (byte)((bytes[6] & 0x0F) | 0x40); // version 4
            bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80); // variant 10xx
            return new Guid(bytes).ToString();
        }

        public static Task<CaSigningResult> SignAsync(
            string method,
            string url,
            string appKey,
            string appSecret,
            string? body = null,
            string contentType = "application/json; charset=utf-8",
            string accept = "*/*",
            Dictionary<string, string>? additionalHeaders = null,
            ILogger? logger = null)
        {
            // ───────────────────────────────────────────────────────────────
            var nonce = CreateUuid();
            var now = DateTime.UtcNow;
            var dateHeader = now.ToString("r", CultureInfo.InvariantCulture); // RFC1123 format
            string md5 = string.Empty;

            if (!string.IsNullOrEmpty(body) && !contentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                var bodyBytes = Encoding.UTF8.GetBytes(body);
                using var md5Hasher = MD5.Create();
                var md5Hash = md5Hasher.ComputeHash(bodyBytes);
                md5 = Convert.ToBase64String(md5Hash);
            }

            // ───────────────────────────────────────────────────────────────
            // Canonical headers (x-ca-*)
            var canonicalHeaders = new List<KeyValuePair<string, string>>
            {
                new("x-ca-key", appKey),
                new("x-ca-nonce", nonce),
                new("x-ca-signaturemethod", "HmacSHA256")
            };

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    var name = header.Key.ToLowerInvariant();
                    if (name.StartsWith("x-ca-") && !ExcludedHeaders.Contains(name))
                    {
                        canonicalHeaders.Add(new(name, header.Value));
                    }
                }
            }

            var sortedHeaders = canonicalHeaders.OrderBy(kv => kv.Key, StringComparer.Ordinal).ToList();
            string signatureHeaders = string.Join(",", sortedHeaders.Select(kv => kv.Key));

            // ───────────────────────────────────────────────────────────────
            // Canonical URL
            string canonicalUrl = UrlToSign(url);

            // ───────────────────────────────────────────────────────────────
            // Build text to sign
            var sb = new StringBuilder();
            sb.Append(method).Append('\n')
              .Append(accept).Append('\n')
              .Append(md5).Append('\n')
              .Append(contentType).Append('\n')
              .Append(dateHeader).Append('\n');

            foreach (var kv in sortedHeaders)
                sb.Append(kv.Key).Append(':').Append(kv.Value).Append('\n');

            sb.Append(canonicalUrl);

            string textToSign = sb.ToString();

            // ───────────────────────────────────────────────────────────────
            // HMAC-SHA256 signature
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(textToSign));
                signature = Convert.ToBase64String(hash);
            }

            // ───────────────────────────────────────────────────────────────
            // Final headers
            var result = new CaSigningResult
            {
                Headers = new Dictionary<string, string>
                {
                    { "X-Ca-Key", appKey },
                    { "X-Ca-Nonce", nonce },
                    { "X-Ca-SignatureMethod", "HmacSHA256" },
                    { "X-Ca-Signature-Headers", signatureHeaders },
                    { "X-Ca-Signature", signature },
                    { "Date", dateHeader },
                    { "Accept", accept },
                    { "Content-MD5", md5 },
                    { "Content-Type", contentType }
                }
            };

            return Task.FromResult(result);
        }

        private static string UrlToSign(string url)
        {
            var uri = new Uri(url);
            var paramBag = new SortedDictionary<string, string>(StringComparer.Ordinal);

            // Parse query string
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            foreach (string? k in query.AllKeys.Where(k => k != null))
            {
                var value = query[k!];
                if (value != null)
                    paramBag[k!] = value;
            }

            var path = uri.AbsolutePath; // Keep slashes as-is
            if (paramBag.Count == 0) return path;

            var qs = string.Join("&", paramBag.Select(p => $"{p.Key}={p.Value}"));
            return $"{path}?{qs}";
        }
    }

    public class CaSigningResult
    {
        public Dictionary<string, string> Headers { get; set; } = new();
    }
}
