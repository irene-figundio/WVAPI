using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class TripsService : ITripsService
    {
        private readonly HttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBase;

        public TripsService(HttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _configuration = configuration;
            _apiBase = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7275/";
            if (!_apiBase.EndsWith("/")) _apiBase += "/";
        }

        // Trips
        public async Task<List<TripDto>> GetTripsAsync(int? eventId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/trips{(eventId.HasValue ? $"?eventId={eventId}" : "")}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<List<TripDto>>(response) ?? new List<TripDto>();
        }

        public async Task<TripDto?> GetTripByIdAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/trips/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<TripDto>(response);
        }

        public async Task<bool> CreateTripAsync(CreateTripRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/trips";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTripAsync(int id, UpdateTripRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/trips/{id}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTripAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/trips/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, sessionToken, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        // Stays
        public async Task<List<StayDto>> GetStaysByTripIdAsync(int tripId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/stays?tripId={tripId}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<List<StayDto>>(response) ?? new List<StayDto>();
        }

        public async Task<StayDto?> GetStayByIdAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/stays/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<StayDto>(response);
        }

        public async Task<bool> CreateStayAsync(CreateStayRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/stays";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStayAsync(int id, CreateStayRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/stays/{id}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStayAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/stays/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, sessionToken, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        // ItineraryDays
        public async Task<List<ItineraryDayDto>> GetItineraryDaysByTripIdAsync(int tripId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarydays?tripId={tripId}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<List<ItineraryDayDto>>(response) ?? new List<ItineraryDayDto>();
        }

        public async Task<ItineraryDayDto?> GetItineraryDayByIdAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarydays/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<ItineraryDayDto>(response);
        }

        public async Task<bool> CreateItineraryDayAsync(CreateItineraryDayRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarydays";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItineraryDayAsync(int id, CreateItineraryDayRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarydays/{id}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItineraryDayAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarydays/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, sessionToken, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        // ItineraryStops
        public async Task<List<ItineraryStopDto>> GetItineraryStopsByDayIdAsync(int dayId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarystops?dayId={dayId}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<List<ItineraryStopDto>>(response) ?? new List<ItineraryStopDto>();
        }

        public async Task<ItineraryStopDto?> GetItineraryStopByIdAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarystops/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<ItineraryStopDto>(response);
        }

        public async Task<bool> CreateItineraryStopAsync(CreateItineraryStopRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarystops";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItineraryStopAsync(int id, CreateItineraryStopRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarystops/{id}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItineraryStopAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/itinerarystops/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, sessionToken, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        // TripMusts
        public async Task<List<TripMustDto>> GetTripMustsByTripIdAsync(int tripId, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/tripmusts?tripId={tripId}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<List<TripMustDto>>(response) ?? new List<TripMustDto>();
        }

        public async Task<TripMustDto?> GetTripMustByIdAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/tripmusts/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Get, endpoint, sessionToken, userAgent: userAgent);
            return await _httpService.GetBodyFromHttpResponseAsync<TripMustDto>(response);
        }

        public async Task<bool> CreateTripMustAsync(CreateTripMustRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/tripmusts";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Post, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTripMustAsync(int id, CreateTripMustRequest request, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/tripmusts/{id}";
            var response = await _httpService.SendHttpRequestAsync(HttpMethod.Put, endpoint, sessionToken, content: request, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTripMustAsync(int id, string sessionToken, string userAgent)
        {
            var endpoint = $"{_apiBase}api/tripmusts/{id}";
            var response = await _httpService.SendHttpRequestAsync<object>(HttpMethod.Delete, endpoint, sessionToken, userAgent: userAgent);
            return response != null && response.IsSuccessStatusCode;
        }
    }
}
