using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq220;

public class Ecq220UpdateRestaurantResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}
