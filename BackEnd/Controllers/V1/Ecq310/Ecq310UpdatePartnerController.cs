using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers.V1.Ecq310;

[Route("api/v1/admin/[controller]")]
[ApiController]
[Authorize(Policy = "Admin")]
public class Ecq310UpdatePartnerController : ControllerBase
{
    
}