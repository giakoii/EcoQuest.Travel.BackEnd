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
/// Ecq300SelectPaymentBookingController - Select Payment Booking Detail
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq300SelectPaymentBookingController : AbstractApiAsyncController<Ecq300SelectPaymentBookingRequest, Ecq300SelectPaymentBookingResponse, Ecq300SelectPaymentBookingEntity>
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq300SelectPaymentBookingController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq300SelectPaymentBookingResponse> ProcessRequest([FromQuery] Ecq300SelectPaymentBookingRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq300SelectPaymentBookingResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq300SelectPaymentBookingResponse> Exec(Ecq300SelectPaymentBookingRequest request)
    {
        // Call Service
        return await _paymentService.SelectPaymentBookingDetail(request, _identityEntity);
    }

    protected internal override Ecq300SelectPaymentBookingResponse ErrorCheck(Ecq300SelectPaymentBookingRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq300SelectPaymentBookingResponse() { Success = false };
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
