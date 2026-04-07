using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface IEventsService
    {
        Task<List<EventDto>> GetEventsAsync(int langId, string sessionToken, string userAgent);
        Task<bool> CreateEventAsync(CreateEventRequest request, string sessionToken, string userAgent);
        Task<EventDto?> GetEventByIdAsync(int id, int langId, string sessionToken, string userAgent);
        Task<EventDto?> GetEventByIdAbsAsync(int id, string sessionToken, string userAgent);
        Task<bool> UpdateEventAsync(int id, UpdateEventRequest request, string sessionToken, string userAgent);
        Task<bool> DeleteEventAsync(int id, string sessionToken, string userAgent);

        Task<List<EventCategoryDto>> GetEventCategoriesAsync(int langId, string token, string userAgent);

        // Variant Prices
        Task<List<VariantPriceDto>> GetVariantPricesAsync(int? eventId, string token, string userAgent);
        Task<bool> CreateVariantPriceAsync(VariantPriceDto model, string token, string userAgent);
        Task<bool> DeleteVariantPriceAsync(int id, string token, string userAgent);

        // Event Needs
        Task<List<EventNeedDto>> GetEventNeedsAsync(int? eventId, string token, string userAgent);
        Task<bool> CreateEventNeedAsync(EventNeedDto model, string token, string userAgent);
        Task<bool> DeleteEventNeedAsync(int id, string token, string userAgent);
    }
}
