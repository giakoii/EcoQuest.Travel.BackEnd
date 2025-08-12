namespace BackEnd.Utils.Const;

public static class ConstantEnum
{
    public enum UserRole
    {
        Customer = 1,
        Partner = 2,
        Admin = 3,	
    }
    
    public enum PartnerType
    {
        Hotel = 1,
        Restaurant = 2,
        Attraction = 3,
    }
    
    public enum ServiceType
    {
        Hotel = 1,
        Restaurant = 2,
        Attraction = 3,
    }
    
    public enum EntityType
    {
        Hotel = 1,
        Restaurant = 2,
        Attraction = 3,
        Destination = 4,
        Blog = 5,
        HotelRoom = 6
    }
    
    public enum BedType
    {
        Single = 1,
        Twin = 2,
        Double = 3,
        Queen = 4,
        King = 5,
        SuperKing = 6,
        Suite = 7,
        Family = 8,
        Bunk = 9,
        Sofa = 10,
        Other = 99
    }

    public enum TripStatus
    {
        Planned = 1,
        Ongoing = 2,
        Completed = 3,
        Cancelled = 4,
    }
    
    public enum IntentType
    {
        top_destinations,
        top_restaurants,
        top_attraction,
        hotel_booking,
        trip_cost,
        ask_ai,
        unknown
    }

    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Completed = 4,
    }
    
    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
    }

    public enum PaymentMethod
    {
        PayOs = 1,
    }

    public enum UserType
    {
        Premier = 2
    }
}