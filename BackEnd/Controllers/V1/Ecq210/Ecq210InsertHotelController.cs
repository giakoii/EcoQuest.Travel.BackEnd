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
/// Ecq210InsertHotelController - Insert hotel
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq210InsertHotelController : AbstractApiAsyncController<Ecq210InsertHotelRequest, Ecq210InsertHotelResponse, string>
{
    private readonly IHotelService _hotelService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq210InsertHotelController(IHotelService hotelService, IIdentityApiClient identityApiClient)
    {
        _hotelService = hotelService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq210InsertHotelResponse> ProcessRequest(Ecq210InsertHotelRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq210InsertHotelResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq210InsertHotelResponse> Exec(Ecq210InsertHotelRequest request)
    {
        return await _hotelService.InsertHotel(request, _identityEntity);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq210InsertHotelResponse ErrorCheck(Ecq210InsertHotelRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq210InsertHotelResponse() { Success = false };
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