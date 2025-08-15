using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPaymentsRequest : AbstractApiRequest
{
    public DateTime? DateFrom { get; set; }
    
    public DateTime? DateTo { get; set; }
    
    public string? Status { get; set; }
    
    public int PageNumber { get; set; } = 1;
    
    public int PageSize { get; set; } = 10;
}