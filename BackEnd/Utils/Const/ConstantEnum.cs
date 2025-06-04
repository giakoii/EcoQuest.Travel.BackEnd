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
    
    public enum EntityImage
    {
        Hotel = 1,
        Restaurant = 2,
        Attraction = 3,
        Destination = 4,
    }

    public enum TripStatus
    {
        Planned = 0,
        Ongoing = 1,
        Completed = 2,
        Cancelled = 3,
    }

}