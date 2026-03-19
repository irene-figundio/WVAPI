using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IEventsService
    {
        Task<List<EventDto>> GetEventsAsync(int langId);
        Task<bool> CreateEventAsync(CreateEventRequest request);
    }
}
