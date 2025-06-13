using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RWS_LBE_Transaction.Services;

namespace RWS_LBE_Transaction.Helpers
{
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

        public async Task<(T? Result, string RawResponse)> DoApiRequestAsync<T>(ApiRequestOptions opts)
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

            if (!string.IsNullOrEmpty(opts.BearerToken) && opts.BasicAuth.HasValue)
            {
                throw new InvalidOperationException("Cannot use both Bearer token and Basic Auth");
            }

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

            _logger.LogInformation("API REQUEST: {Method} {Url}; Content-Type: {ContentType}; Body: {Body}",
                opts.Method, opts.Url, opts.ContentType, opts.Body == null ? "<empty>" : JsonSerializer.Serialize(opts.Body, _jsonOptions));

            using var response = await _httpClient.SendAsync(request); // no cancellation token

            var rawResponse = await response.Content.ReadAsStringAsync();

            var sanitized = rawResponse.Replace("\n", " ").Replace("\t", " ").Trim();

            _logger.LogInformation("API RESPONSE: Status: {StatusCode}; Body: {Body}",
                (int)response.StatusCode, sanitized);

            if ((int)response.StatusCode != opts.ExpectedStatus)
            {
                throw new HttpRequestException($"Unexpected status {(int)response.StatusCode}: {rawResponse}");
            }

            if (string.IsNullOrWhiteSpace(rawResponse))
            {
                return (default, rawResponse);
            }

            T? result;
            try
            {
                result = JsonSerializer.Deserialize<T>(rawResponse, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize API response: {RawResponse}", rawResponse);
                throw;
            }

            return (result, rawResponse);
        }
    }
}
