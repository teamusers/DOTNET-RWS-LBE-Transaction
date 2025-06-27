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

        private static readonly Random _random = new Random();

        /// <summary>
        /// Creates a UUID matching the JavaScript implementation exactly
        /// </summary>
    public static string CreateUuid()
    {
        // 1️⃣  Get 16 random bytes
        byte[] bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);

        // 2️⃣  Set version = 4  (xxxx0100)
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x40);

        // 3️⃣  Set variant = RFC 4122 (10xx xxxx)
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        // 4️⃣  Convert to Guid & string
        return new Guid(bytes).ToString();
    }

        /// <summary>
        /// Generates CA signing headers for the given request parameters.
        /// </summary>
        public static Task<CaSigningResult> SignAsync(string method, 
                                                           string url, 
                                                           string appKey, 
                                                           string appSecret,
                                                           ILogger? logger = null,
                                                           Dictionary<string, string>? additionalHeaders = null)
        {
            // ──────────────────────────────────────────────────────────────────────────
            // 1. Common values
            // ──────────────────────────────────────────────────────────────────────────
            var nonce = CreateUuid();                                       // x-ca-nonce
            var now   = DateTime.Now.ToString("M/d/yyyy, h:mm:ss tt").ToUpper();   // matches JS Date().toLocaleString(). maybe wrong?

            string accept = "*/*";
            string contentType = "application/json"; // For GET requests
            string md5 = ""; // For GET requests, body is empty

            // ──────────────────────────────────────────────────────────────────────────
            // 2. Canonical headers (x-ca-*, sorted, **lower-case keys**)
            // ──────────────────────────────────────────────────────────────────────────
            var canonicalHeaders = new List<KeyValuePair<string, string>>
            {

                new KeyValuePair<string, string>("x-ca-key", appKey),
                new KeyValuePair<string, string>("x-ca-nonce", nonce),
                new KeyValuePair<string, string>("x-ca-signaturemethod", "HmacSHA256")
            };

            // Add any additional x-ca- headers
            // if (additionalHeaders != null)
            // {
            //     foreach (var header in additionalHeaders)
            //     {
            //         var name = header.Key.ToLowerInvariant();
            //         if (name.StartsWith("x-ca-") && !ExcludedHeaders.Contains(name))
            //         {
            //             canonicalHeaders.Add(new KeyValuePair<string, string>(name, header.Value));
            //         }
            //     }
            // }

            var sortedHeaders = canonicalHeaders.OrderBy(p => p.Key, StringComparer.Ordinal);
            string signatureHeaders = String.Join(",", sortedHeaders.Select(h => h.Key));


            // ──────────────────────────────────────────────────────────────────────────
            // 3. Canonical URL (path + sorted query params)
            // ──────────────────────────────────────────────────────────────────────────
            string canonicalUrl = UrlToSign(url);

            // ──────────────────────────────────────────────────────────────────────────
            // 4. Build the string to sign
            // ──────────────────────────────────────────────────────────────────────────
            var sb = new StringBuilder();
            sb.Append(method)  .Append('\n')
              .Append(accept)  .Append('\n')
              .Append(md5)     .Append('\n')
              .Append(contentType) .Append('\n')
              .Append(now)     .Append('\n');

            foreach (var kv in sortedHeaders)
                sb.Append(kv.Key).Append(':').Append(kv.Value).Append('\n');

            sb.Append(canonicalUrl);
            string textToSign = sb.ToString();

            // ──────────────────────────────────────────────────────────────────────────
            // 5. HMAC-SHA256 and Base64
            // ──────────────────────────────────────────────────────────────────────────
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(textToSign));
                signature = Convert.ToBase64String(hash);
            }

            // ──────────────────────────────────────────────────────────────────────────
            // 6. Return the headers
            // ──────────────────────────────────────────────────────────────────────────
            var result = new CaSigningResult
            {
                Headers = new Dictionary<string, string>
                {
                    { "X-Ca-Key", appKey },
                    { "X-Ca-Nonce", nonce },
                    { "X-Ca-Signature", signature },
                    { "X-Ca-Signature-Headers", "x-ca-key,x-ca-nonce,x-ca-signaturemethod" },
                    { "X-Ca-SignatureMethod", "HmacSHA256" },
                    { "Date", now },
                    { "Content-MD5", "" }
                }
            };

            return Task.FromResult(result);
        }

        // =============  helpers  ====================================================

        private static string UrlToSign(string url)
        {
            var uri = new Uri(url);
            var paramBag = new SortedDictionary<string, string>(StringComparer.Ordinal);

            // ── ①  query string ------------------------------------------------------
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            foreach (string k in query.AllKeys.Where(k => k != null))
            {
                var value = query[k];
                if (value != null)
                    paramBag[k] = value;
            }

            // ── ②  canonical path (exact slash build that the JS does) --------------
            var segments = uri.AbsolutePath
                          .Split('/', StringSplitOptions.RemoveEmptyEntries);
            var path = String.Join("/", segments.Select(s => Uri.UnescapeDataString(s)));
            path = "/" + path;                               // JS prepends a slash

            // ── ③  attach sorted params --------------------------------------------
            if (paramBag.Count == 0) return path;

            var qs = String.Join("&", paramBag.Select(p => $"{p.Key}={p.Value}"));
            return $"{path}?{qs}";
        }
    }

    public class CaSigningResult
    {
        public Dictionary<string, string> Headers { get; set; } = new();
    }
} 