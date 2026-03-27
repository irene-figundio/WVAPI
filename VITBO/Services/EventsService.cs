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

        public async Task<bool> CreateEventAsync(CreateEventRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/events";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken,request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }

        public async Task<List<EventDto>> GetEventsAsync(int langId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/events?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, sessionToken, userAgent);
            var list = await _httpService.GetBodyFromHttpResponseAsync<List<EventDto>>(result);
            // var result = await _apiService.GetAsync<List<EventDto>>(endpoint, sessionToken, userAgent);
            return list ?? new List<EventDto>();
        }

        public async Task<EventDto?> GetEventByIdAsync(int id, int langId,string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/events/{id}?langId={langId}";
            var result = await _httpService.SendHttpRequestAsync(HttpMethod.Get, endpoint, sessionToken, userAgent);
            var eventDto = await _httpService.GetBodyFromHttpResponseAsync<EventDto>(result);
            return eventDto;
        }

        public async Task<bool> UpdateEventAsync(int id, UpdateEventRequest request,string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/events/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint,sessionToken,request, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        } 

        public async Task<bool> DeleteEventAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}/events/{id}";
            return await _httpService.SendHttpRequestAsync(HttpMethod.Delete, endpoint, sessionToken, userAgent) is HttpResponseMessage response && response.IsSuccessStatusCode;
        }


    }



    public class UpdateEventRequest {
        public int id;
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public DateTime? EndDate { get; set; } = null;
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
        public int GalleryId { get; set; }
    }
}
