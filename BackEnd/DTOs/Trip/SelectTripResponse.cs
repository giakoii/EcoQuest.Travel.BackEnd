using BackEnd.Controllers;

namespace BackEnd.DTOs.Trip;

public class SelectTripResponse : AbstractApiResponse<SelectTripEntity>
{
    public override SelectTripEntity Response { get; set; }
}

public class SelectTripEntity
{
}