using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPartnerRequest : AbstractApiRequest
{
    public Guid PartnerId { get; set; }
}