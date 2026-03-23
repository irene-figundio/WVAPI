namespace VITBO.Services
{
    using System.Text.Json;
    using System.Net.Http.Headers;

    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;

        public HttpService(IHttpClientFactory httpClientFactory, ILogger<HttpService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("HttpClient");
            _logger = logger;
        }

        public async Task<HttpResponseMessage?> SendHttpRequestAsync<T>(HttpMethod method, string url, string? sessionToken = null, T? content = default, string? userAgent = null, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(method, url);

            // Specify Authorization header
            if (!string.IsNullOrEmpty(sessionToken))
            {
                request.Headers.Add("Authorization", $"Bearer {sessionToken}");
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.Headers.TryAddWithoutValidation("User-Agent", userAgent);
            }

            // POST/PUT case: add body
            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                if (content != null)
                {
                    var json = JsonSerializer.Serialize(content);
                    request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    _logger.LogWarning("No content provided for {Method} request to {Url}.", method, url);
                }
            }

            // GET/DELETE case: no body needed

            try
            {
                return await _httpClient.SendAsync(request, ct); //Do the actual request
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to {Url} failed.", url);

                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "HTTP request to {Url} timed out or was cancelled.", url);

                return null;
            }
        }

        public async Task<HttpResponseMessage?> PostMultipartAsync(string url, MultipartFormDataContent content, string? sessionToken = null, string? userAgent = null, CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            if (!string.IsNullOrEmpty(sessionToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessionToken);
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.Headers.TryAddWithoutValidation("User-Agent", userAgent);
            }

            try
            {
                return await _httpClient.SendAsync(request, ct);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP multipart POST to {Url} failed.", url);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "HTTP multipart POST to {Url} timed out or was cancelled.", url);
                return null;
            }
        }

        public async Task<T?> GetBodyFromHttpResponseAsync<T>(HttpResponseMessage? response)
        {
            if (response?.Content == null)
                return default;

            var responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
                return default;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                AllowTrailingCommas = true
            };

            try
            {
                // If T is not a collection but the JSON starts with '[', try to unwrap the first element.
                if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string) && responseBody.TrimStart().StartsWith("["))
                {
                    var list = JsonSerializer.Deserialize<List<T>>(responseBody, options);
                    return list != null && list.Count > 0 ? list[0] : default;
                }

                return JsonSerializer.Deserialize<T>(responseBody, options);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization failed for type {Type}. Raw JSON: {Json}", typeof(T).Name, responseBody);

                return default;
            }
        }
    }
}
