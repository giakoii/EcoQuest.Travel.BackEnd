using BackEnd.DTOs.Ecq210;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq210;

/// <summary>
/// Ecq210SelectHotelController - Select hotel
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq210SelectHotelController : AbstractApiAsyncController<Ecq210SelectHotelRequest, Ecq210SelectHotelResponse, Ecq210SelectHotelEntity>
{
    private readonly IHotelService _hotelService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq210SelectHotelController(IHotelService hotelService, IIdentityApiClient identityApiClient)
    {
        _hotelService = hotelService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq210SelectHotelResponse> ProcessRequest([FromQuery] Ecq210SelectHotelRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq210SelectHotelResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq210SelectHotelResponse> Exec(Ecq210SelectHotelRequest request)
    {
        return await _hotelService.SelectHotel(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq210SelectHotelResponse ErrorCheck(Ecq210SelectHotelRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq210SelectHotelResponse() { Success = false };
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