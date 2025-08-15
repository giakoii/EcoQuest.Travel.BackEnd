using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectAttractionsRequest : AbstractApiRequest
{
    public List<Guid>? DestinationIds { get; set; }
}