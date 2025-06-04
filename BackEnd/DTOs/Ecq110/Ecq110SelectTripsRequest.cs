using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectTripsRequest : AbstractApiRequest
{
    // This can be empty as we're selecting all trips
    // or we could filter by user ID if provided by the endpoint
}
