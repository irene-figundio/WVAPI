namespace VITBO.Models
{
    public class TripDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string DepartureCity { get; set; } = null!;
        public string DepartureCountry { get; set; } = null!;
        public string ArrivalCity { get; set; } = null!;
        public string? ArrivalCountry { get; set; }
        public int DurationDays { get; set; }
        public int DurationNights { get; set; }
        public int MaxGuests { get; set; }
        public string Status { get; set; } = null!;
    }

    public class CreateTripRequest
    {
        public int EventId { get; set; }
        public string DepartureCity { get; set; } = null!;
        public string DepartureCountry { get; set; } = null!;
        public string ArrivalCity { get; set; } = null!;
        public string? ArrivalCountry { get; set; }
        public int DurationDays { get; set; }
        public int DurationNights { get; set; }
        public int MaxGuests { get; set; }
        public string Status { get; set; } = null!;
    }

    public class UpdateTripRequest : CreateTripRequest
    {
        public int Id { get; set; }
    }

    public class StayDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Location { get; set; }
        public int OrderIndex { get; set; }
        public int? ItineraryDayId { get; set; }
    }

    public class CreateStayRequest
    {
        public int TripId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Location { get; set; }
        public int OrderIndex { get; set; }
        public int? ItineraryDayId { get; set; }
    }

    public class ItineraryDayDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int DayNumber { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CreateItineraryDayRequest
    {
        public int TripId { get; set; }
        public int DayNumber { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class ItineraryStopDto
    {
        public int Id { get; set; }
        public int DayId { get; set; }
        public TimeSpan? Time { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public int OrderIndex { get; set; }
    }

    public class CreateItineraryStopRequest
    {
        public int DayId { get; set; }
        public TimeSpan? Time { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public int OrderIndex { get; set; }
    }

    public class TripMustDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string Text { get; set; } = null!;
        public int TypeId { get; set; }
    }

    public class CreateTripMustRequest
    {
        public int TripId { get; set; }
        public string Text { get; set; } = null!;
        public int TypeId { get; set; }
    }
}
