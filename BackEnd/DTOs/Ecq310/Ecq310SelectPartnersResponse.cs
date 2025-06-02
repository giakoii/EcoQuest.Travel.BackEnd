using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPartnersResponse : AbstractApiResponse<List<Ecq310SelectPartnersEntity>>
{
    public override List<Ecq310SelectPartnersEntity> Response { get; set; }
}

public class Ecq310SelectPartnersEntity
{
    public Guid PartnerId { get; set; }

    public Guid AccountId { get; set; }

    public string? CompanyName { get; set; }

    public string ContactName { get; set; }

    public string? Phone { get; set; }

    public string? Description { get; set; }

    public bool? Verified { get; set; }
    
    public List<byte> PartnerType { get; set; }

    public string CreatedAt { get; set; }
}