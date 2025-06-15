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
/// Ecq210UpdateHotelController - Update hotel
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq210UpdateHotelController : AbstractApiAsyncController<Ecq210UpdateHotelRequest, Ecq210UpdateHotelResponse, string>
{
    private readonly IHotelService _hotelService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq210UpdateHotelController(IHotelService hotelService, IIdentityApiClient identityApiClient)
    {
        _hotelService = hotelService;
        _identityApiClient = identityApiClient;
    }

    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq210UpdateHotelResponse> ProcessRequest(Ecq210UpdateHotelRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq210UpdateHotelResponse());
    }
    
    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq210UpdateHotelResponse> Exec(Ecq210UpdateHotelRequest request)
    {
        return await _hotelService.UpdateHotel(request, _identityEntity);
    }

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq210UpdateHotelResponse ErrorCheck(Ecq210UpdateHotelRequest request,
        List<DetailError> detailErrorList)
    {
        var response = new Ecq210UpdateHotelResponse() { Success = false };
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
