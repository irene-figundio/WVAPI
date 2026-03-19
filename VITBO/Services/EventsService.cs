using VITBO.Models;
using VITBO.Services.Interfaces;

namespace VITBO.Services
{
    public class EventsService : IEventsService
    {
        private readonly ApiService _apiService;

        public EventsService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> CreateEventAsync(CreateEventRequest request)
        {
            return await _apiService.PostVoidAsync("api/Events", request);
        }

        public async Task<List<EventDto>> GetEventsAsync(int langId)
        {
            var endpoint = $"api/Events?langId={langId}";
            var result = await _apiService.GetAsync<List<EventDto>>(endpoint);
            return result ?? new List<EventDto>();
        }
    }
}
