using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110InsertPaymentController - Insert a new payment for booking
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110InsertPaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IIdentityApiClient _identityApiClient;

    public Ecq110InsertPaymentController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
    {
        _paymentService = paymentService;
        _identityApiClient = identityApiClient;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
   public async Task<IActionResult> ProcessRequest(Ecq110InsertPaymentRequest request)
   {
       var identityEntity = new IdentityEntity();
       identityEntity = _identityApiClient.GetIdentity(User);
       
        try
        {
            var response = await _paymentService.InsertPayment(request, identityEntity);
            return Redirect(response.Response.CheckoutUrl);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error processing payment request");
            return new BadRequestObjectResult(new { Message = "An error occurred while processing the payment." });
        }
    }
}