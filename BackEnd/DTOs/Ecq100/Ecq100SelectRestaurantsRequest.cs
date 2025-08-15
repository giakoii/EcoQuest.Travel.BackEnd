using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectRestaurantsRequest : AbstractApiRequest
{
    public List<Guid>? DestinationIds { get; set; }
}