using BackEnd.Controllers;

namespace BackEnd.DTOs.User;

public class SelectPartnerResponse : AbstractApiResponse<SelectPartnerEntity>
{
    public override SelectPartnerEntity Response { get; set; }
}

public class SelectPartnerEntity
{
}