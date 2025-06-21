using BackEnd.DTOs.Ecq110;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq110;

/// <summary>
/// Ecq110InsertBookingTripController - Insert a new booking trip
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq110InsertBookingTripController : AbstractApiAsyncController<Ecq110InsertBookingTripRequest, Ecq110InsertBookingTripResponse, string>
{
    private readonly IBookingService _bookingService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="bookingService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq110InsertBookingTripController(IBookingService bookingService, IIdentityApiClient identityApiClient)
    {
        _bookingService = bookingService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq110InsertBookingTripResponse> ProcessRequest(Ecq110InsertBookingTripRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq110InsertBookingTripResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq110InsertBookingTripResponse> Exec(Ecq110InsertBookingTripRequest request)
    {
        return await _bookingService.InsertBookingTrip(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq110InsertBookingTripResponse ErrorCheck(Ecq110InsertBookingTripRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq110InsertBookingTripResponse { Success = false };
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