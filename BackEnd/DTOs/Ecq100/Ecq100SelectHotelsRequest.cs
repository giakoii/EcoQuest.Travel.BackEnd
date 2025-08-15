using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq100;

public class Ecq100SelectHotelsRequest : AbstractApiRequest
{
    public List<Guid>? DestinationIds { get; set; }
}