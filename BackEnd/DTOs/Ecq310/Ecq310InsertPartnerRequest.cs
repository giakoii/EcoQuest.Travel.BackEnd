using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310InsertPartnerRequest : AbstractApiRequest
{
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? CompanyName { get; set; }

    public string ContactName { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public List<byte> PartnerType { get; set; }
}