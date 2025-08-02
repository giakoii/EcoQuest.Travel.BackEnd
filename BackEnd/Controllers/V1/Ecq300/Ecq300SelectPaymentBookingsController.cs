using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq300;

/// <summary>
/// Ecq300SelectPaymentBookingsController - Select Payment Booking
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq300SelectPaymentBookingsController : AbstractApiAsyncController<Ecq300SelectPaymentBookingsRequest, Ecq300SelectPaymentBookingsResponse, List<Ecq300SelectPaymentBookingsEntity>>
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq300SelectPaymentBookingsController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
    {
        _identityApiClient = identityApiClient;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq300SelectPaymentBookingsResponse> ProcessRequest([FromQuery] Ecq300SelectPaymentBookingsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq300SelectPaymentBookingsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override Task<Ecq300SelectPaymentBookingsResponse> Exec(Ecq300SelectPaymentBookingsRequest request)
    {
        return _paymentService.SelectPaymentBooking(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq300SelectPaymentBookingsResponse ErrorCheck(Ecq300SelectPaymentBookingsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq300SelectPaymentBookingsResponse() { Success = false };
        if (detailErrorList.Count > 0)
        {
            // Error
            response.SetMessage(MessageId.E10000);
            response.DetailErrorList = detailErrorList;
            return response;
        }
        // True
        response.Success = true;
        return response;
    }
}