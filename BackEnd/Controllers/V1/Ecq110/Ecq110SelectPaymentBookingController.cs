using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110SelectPaymentBookingController - Select Payment Booking
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110SelectPaymentBookingController : AbstractApiAsyncController<Ecq110SelectPaymentBookingRequest, Ecq110SelectPaymentBookingResponse, Ecq110SelectPaymentBookingEntity>
{
    private readonly IPaymentService _paymentService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110SelectPaymentBookingController(IPaymentService paymentService, IIdentityApiClient identityApiClient)
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
    public override async Task<Ecq110SelectPaymentBookingResponse> ProcessRequest([FromQuery] Ecq110SelectPaymentBookingRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110SelectPaymentBookingResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override Task<Ecq110SelectPaymentBookingResponse> Exec(Ecq110SelectPaymentBookingRequest request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110SelectPaymentBookingResponse ErrorCheck(Ecq110SelectPaymentBookingRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110SelectPaymentBookingResponse() { Success = false };
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