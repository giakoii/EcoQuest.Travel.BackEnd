using BackEnd.DTOs.Ecq211;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq211;

/// <summary>
/// Ecq211UpdateHotelRoomController - Update hotel room
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq211UpdateHotelRoomController : AbstractApiAsyncController<Ecq211UpdateHotelRoomRequest, Ecq211UpdateHotelRoomResponse, string>
{
    private readonly IHotelRoomService _hotelRoomService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelRoomService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq211UpdateHotelRoomController(IHotelRoomService hotelRoomService, IIdentityApiClient identityApiClient)
    {
        _hotelRoomService = hotelRoomService;
        _identityApiClient = identityApiClient;
    }
    
    /// <summary>
    /// Incoming Put
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = ConstRole.Admin, AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq211UpdateHotelRoomResponse> ProcessRequest(Ecq211UpdateHotelRoomRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq211UpdateHotelRoomResponse());
    }
    
    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq211UpdateHotelRoomResponse> Exec(Ecq211UpdateHotelRoomRequest request)
    {
        return await _hotelRoomService.UpdateHotelRoom(request, _identityEntity);
    }
    
    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq211UpdateHotelRoomResponse ErrorCheck(Ecq211UpdateHotelRoomRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq211UpdateHotelRoomResponse { Success = false };
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
