using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq210;

public class Ecq210UpdateHotelResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}
