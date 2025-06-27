using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure;
using RWS_LBE_Transaction.DTOs.Shared;
using RWS_LBE_Transaction.Exceptions;

namespace RWS_LBE_Transaction.Helpers
{
    public interface IApiHttpClient
    {
        Task<T?> DoApiRequestAsync<T>(ApiRequestOptions opts);
    }

    public class ApiHttpClient : IApiHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiHttpClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiHttpClient(HttpClient httpClient, ILogger<ApiHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                // You can add more options here if needed
            };
        }

        public async Task<T?> DoApiRequestAsync<T>(ApiRequestOptions opts)
        {
            using var request = new HttpRequestMessage(opts.Method, opts.Url);

            // Determine if a Content-Type is expected (even for GET)
            string? headerContentType = null;
            if (opts.Headers != null &&
                opts.Headers.TryGetValue("Content-Type", out var contentTypeHeader) &&
                !string.IsNullOrWhiteSpace(contentTypeHeader))
            {
                headerContentType = contentTypeHeader;
            }

            if (opts.Body != null)
            {
                if (opts.ContentType == "application/x-www-form-urlencoded" && opts.Body is Dictionary<string, string> formDict)
                {
                    request.Content = new FormUrlEncodedContent(formDict);
                }
                else
                {
                    var json = JsonSerializer.Serialize(opts.Body, _jsonOptions);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    opts.ContentType = "application/json"; // normalize
                }
            }
            else if (opts.Method == HttpMethod.Get && !string.IsNullOrEmpty(headerContentType))
            {
                // For GET: force empty content to attach Content-Type if required
                request.Content = new StringContent(string.Empty);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(headerContentType);
            }

            // bearer token takes priority
            if (!string.IsNullOrEmpty(opts.BearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", opts.BearerToken);
            }
            else if (opts.BasicAuth.HasValue)
            {
                var (user, pass) = opts.BasicAuth.Value;
                var basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuthValue);
            }

            if (opts.Headers != null)
            {
                foreach (var (key, value) in opts.Headers)
                {
                    if (key.Equals("Date", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTimeOffset.TryParse(value, out var parsedDate))
                        {
                            request.Headers.Date = parsedDate;
                        }
                        else
                        {
                            request.Headers.TryAddWithoutValidation(key, value);
                        }
                    }
                    else if (key.Equals("Content-MD5", StringComparison.OrdinalIgnoreCase) && request.Content != null)
                    {
                        request.Content.Headers.ContentMD5 = Convert.FromBase64String(value);
                    }
                    else if (!key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)) // already handled above
                    {
                        request.Headers.TryAddWithoutValidation(key, value);
                    }
                }
            }

            var generalHeaders = string.Join(", ",
                request.Headers.Select(h => $"{h.Key}: {string.Join(";", h.Value)}"));

            var contentHeaders = request.Content?.Headers != null
                ? string.Join(", ", request.Content.Headers.Select(h => $"{h.Key}: {string.Join(";", h.Value)}"))
                : "<none>";

            var requestBody = request.Content == null
                ? "<empty>"
                : await request.Content.ReadAsStringAsync(); // logs actual serialized body

            _logger.LogInformation(
                "[API REQUEST] {Method} {Url}; Headers: {GeneralHeaders}; Content-Headers: {ContentHeaders}; Body: {Body}",
                request.Method,
                request.RequestUri,
                generalHeaders,
                contentHeaders,
                requestBody);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            var rawResponse = await response.Content.ReadAsStringAsync();
            var sanitized = rawResponse.Replace("\n", " ").Replace("\t", " ").Trim();

            _logger.LogInformation("[API RESPONSE] Status: {StatusCode}; Body: {Body}",
                (int)response.StatusCode, sanitized);

            if (response.StatusCode != opts.ExpectedStatus)
            {
                throw new ExternalApiException("Unexpected HTTP response status code received", response.StatusCode, rawResponse);
            }

            if (string.IsNullOrWhiteSpace(rawResponse))
            {
                return default;
            }

            T? result;
            try
            {
                result = JsonSerializer.Deserialize<T>(rawResponse, _jsonOptions);
            }
            catch (JsonException)
            {
                throw new ExternalApiException("API response deserialization failed.", response.StatusCode, rawResponse);
            }

            return result;
        }


    }
}
