using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class EventsService : IEventsService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBase;

        public EventsService(HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
            _apiBase = _configuration["ApiBaseAddress"] ?? "https://localhost:7275";
        }

        public async Task<int?> CreateEventAsync(CreateEventRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/events";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken, request, userAgent);
            if (response != null && response.IsSuccessStatusCode)
            {
                var body = await _httpService.GetBodyFromHttpResponseAsync<dynamic>(response);
                if (body != null && body.GetProperty("id").ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    return body.GetProperty("id").GetInt32();
                }
            }
            return null;
        }

        public async Task<List<EventDto>> GetEventsAsync(int langId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/events?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, sessionToken, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<EventDto>>(result);
            // var result = await _apiService.GetAsync<List<EventDto>>(endpoint, sessionToken, userAgent);
            return list ?? new List<EventDto>();
        }

        public async Task<EventDto?> GetEventByIdAsync(int id, int langId,string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/events/{id}?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, sessionToken, userAgent);
            var eventDto = await _httpService.GetBodyFromHttpResponseAsync<EventDto>(result);
            return eventDto;
        }

        public async Task<EventDto?> GetEventByIdAbsAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/events/absolute/{id}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, sessionToken, userAgent);
            var eventDto = await _httpService.GetBodyFromHttpResponseAsync<EventDto>(result);
            return eventDto;
        }

        public async Task<bool> UpdateEventAsync(int id, UpdateEventRequest request,string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/events/{id}";
            var jsonRequest = System.Text.Json.JsonSerializer.Serialize(request);
            return await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        } 

        public async Task<bool> DeleteEventAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/events/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, sessionToken, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<EventCategoryDto>> GetEventCategoriesAsync(int langId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/eventcategories?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<EventCategoryDto>>(result);
            return list ?? new List<EventCategoryDto>();
        }

        // Variant Prices
        public async Task<List<VariantPriceDto>> GetVariantPricesAsync(int? eventId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/variantprices" + (eventId.HasValue ? $"?eventId={eventId}" : "");
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<VariantPriceDto>>(result);
            return list ?? new List<VariantPriceDto>();
        }

        public async Task<bool> CreateVariantPriceAsync(VariantPriceDto model, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/variantprices";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteVariantPriceAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/variantprices/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        // Event Needs
        public async Task<List<EventNeedDto>> GetEventNeedsAsync(int? eventId, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/eventneeds" + (eventId.HasValue ? $"?eventId={eventId}" : "");
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, token, userAgent, null);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<EventNeedDto>>(result);
            return list ?? new List<EventNeedDto>();
        }

        public async Task<bool> CreateEventNeedAsync(EventNeedDto model, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/eventneeds";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, token, model, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEventNeedAsync(int id, string token, string userAgent)
        {
            var endpoint = $"{_apiBase}/api/eventneeds/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, token, userAgent, null) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }
    }



    public class UpdateEventRequest {
        public int id;
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? EndDate { get; set; } = null;
        public bool HasVariantPrice { get; set; } = true;
        public bool HasNeeds { get; set; } = true;
        public string? ProgramPdf { get; set; }
        public TimeSpan? EndTime { get; set; } = TimeSpan.Zero;
        public string? CoverImage { get; set; } = null;
        public DateTime? BookingEndDate { get; set; } = null;
        public string? Location { get; set; }
        public string? Organizer { get; set; }
        public string? ContactInfo { get; set; }
        public decimal Price { get; set; }
        public bool IsOnline { get; set; } = false;
        public int LangID { get; set; } 
        public string? Subtitle { get; set; }
        public int? CategoryId { get; set; }
        public string? Coordinates { get; set; }
        public string? HeroImage { get; set; } = null;
        public int? GalleryId { get; set; }
    }
}
