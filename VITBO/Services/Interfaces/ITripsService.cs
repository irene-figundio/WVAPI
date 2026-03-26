using VITBO.Models;

namespace VITBO.Services.Interfaces
{
    public interface ITripsService
    {
        // Trips
        Task<List<TripDto>> GetTripsAsync(int? eventId, string sessionToken, string userAgent);
        Task<TripDto?> GetTripByIdAsync(int id, string sessionToken, string userAgent);
        Task<bool> CreateTripAsync(CreateTripRequest request, string sessionToken, string userAgent);
        Task<bool> UpdateTripAsync(int id, UpdateTripRequest request, string sessionToken, string userAgent);
        Task<bool> DeleteTripAsync(int id, string sessionToken, string userAgent);

        // Stays
        Task<List<StayDto>> GetStaysByTripIdAsync(int tripId, string sessionToken, string userAgent);
        Task<StayDto?> GetStayByIdAsync(int id, string sessionToken, string userAgent);
        Task<bool> CreateStayAsync(CreateStayRequest request, string sessionToken, string userAgent);
        Task<bool> UpdateStayAsync(int id, CreateStayRequest request, string sessionToken, string userAgent);
        Task<bool> DeleteStayAsync(int id, string sessionToken, string userAgent);

        // ItineraryDays
        Task<List<ItineraryDayDto>> GetItineraryDaysByTripIdAsync(int tripId, string sessionToken, string userAgent);
        Task<ItineraryDayDto?> GetItineraryDayByIdAsync(int id, string sessionToken, string userAgent);
        Task<bool> CreateItineraryDayAsync(CreateItineraryDayRequest request, string sessionToken, string userAgent);
        Task<bool> UpdateItineraryDayAsync(int id, CreateItineraryDayRequest request, string sessionToken, string userAgent);
        Task<bool> DeleteItineraryDayAsync(int id, string sessionToken, string userAgent);

        // ItineraryStops
        Task<List<ItineraryStopDto>> GetItineraryStopsByDayIdAsync(int dayId, string sessionToken, string userAgent);
        Task<ItineraryStopDto?> GetItineraryStopByIdAsync(int id, string sessionToken, string userAgent);
        Task<bool> CreateItineraryStopAsync(CreateItineraryStopRequest request, string sessionToken, string userAgent);
        Task<bool> UpdateItineraryStopAsync(int id, CreateItineraryStopRequest request, string sessionToken, string userAgent);
        Task<bool> DeleteItineraryStopAsync(int id, string sessionToken, string userAgent);

        // TripMusts
        Task<List<TripMustDto>> GetTripMustsByTripIdAsync(int tripId, string sessionToken, string userAgent);
        Task<TripMustDto?> GetTripMustByIdAsync(int id, string sessionToken, string userAgent);
        Task<bool> CreateTripMustAsync(CreateTripMustRequest request, string sessionToken, string userAgent);
        Task<bool> UpdateTripMustAsync(int id, CreateTripMustRequest request, string sessionToken, string userAgent);
        Task<bool> DeleteTripMustAsync(int id, string sessionToken, string userAgent);
    }
}
