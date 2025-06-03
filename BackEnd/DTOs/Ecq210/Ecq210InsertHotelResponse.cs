using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq210;

public class Ecq210InsertHotelResponse : AbstractApiResponse<string>
{
    public override string Response { get; set; }
}