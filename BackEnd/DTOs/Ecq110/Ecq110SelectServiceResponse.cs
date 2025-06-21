using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectServiceResponse : AbstractApiResponse<List<Ecq110SelectServiceEntity>>
{
    public override List<Ecq110SelectServiceEntity> Response { get; set; }
}

public class Ecq110SelectServiceEntity
{
    public Guid ServiceId { get; set; }
    
    public string ServiceName { get; set; }
    
    public string Cost { get; set; }
    
    public string Address { get; set; }
}