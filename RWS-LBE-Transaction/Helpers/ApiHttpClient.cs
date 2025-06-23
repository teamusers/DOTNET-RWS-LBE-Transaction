using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
                    request.Headers.TryAddWithoutValidation(key, value);
                }
            }

            var headersLog = opts.Headers != null
                ? string.Join(", ", opts.Headers.Select(h => $"{h.Key}: {h.Value}"))
                : "<none>";

            _logger.LogInformation("[API REQUEST] {Method} {Url}; Content-Type: {ContentType}; Body: {Body}; Headers: {Headers}",
                opts.Method,
                opts.Url,
                opts.ContentType,
                opts.Body == null ? "<empty>" : JsonSerializer.Serialize(opts.Body, _jsonOptions),
                headersLog);

            using var response = await _httpClient.SendAsync(request); // no cancellation token

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
