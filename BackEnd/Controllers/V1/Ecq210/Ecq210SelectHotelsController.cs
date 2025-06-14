using BackEnd.DTOs.Ecq210;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OpenIddict.Validation.AspNetCore;

namespace BackEnd.Controllers.V1.Ecq210;

/// <summary>
/// Ecq210SelectHotelsController - Select hotels
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq210SelectHotelsController : AbstractApiAsyncController<Ecq210SelectHotelsRequest, Ecq210SelectHotelsResponse, List<Ecq210HotelEntity>>
{
    private readonly IHotelService _hotelService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq210SelectHotelsController(IHotelService hotelService, IIdentityApiClient identityApiClient)
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
    [Authorize(Roles = ConstRole.Partner, AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq210SelectHotelsResponse> ProcessRequest([FromQuery] Ecq210SelectHotelsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq210SelectHotelsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq210SelectHotelsResponse> Exec(Ecq210SelectHotelsRequest request)
    {
        return await _hotelService.Ecq210SelectHotels(_identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq210SelectHotelsResponse ErrorCheck(Ecq210SelectHotelsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq210SelectHotelsResponse() { Success = false };
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
