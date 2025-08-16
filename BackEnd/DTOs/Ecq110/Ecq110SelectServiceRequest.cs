using System.ComponentModel.DataAnnotations;
using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq110;

public class Ecq110SelectServiceRequest : AbstractApiRequest
{
    [Required(ErrorMessage = "ServiceType is required.")]
    public int ServiceType { get; set; }
    
    public List<Guid>? DestinationIds { get; set; }
}