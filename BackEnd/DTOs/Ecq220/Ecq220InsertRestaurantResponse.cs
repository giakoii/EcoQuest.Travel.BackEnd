using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq220;

public class Ecq220InsertRestaurantResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}
