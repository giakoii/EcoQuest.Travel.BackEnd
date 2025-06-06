using BackEnd.DTOs.Ecq211;
using BackEnd.Services;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq211;

/// <summary>
/// Ecq211InsertHotelRoomController - Insert hotel room
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq211InsertHotelRoomController : AbstractApiAsyncController<Ecq211InsertHotelRoomRequest, Ecq211InsertHotelRoomResponse, string>
{
    private readonly IHotelRoomService _hotelRoomService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelRoomService"></param>
    /// <param name="identityApiClient"></param>
    public Ecq211InsertHotelRoomController(IHotelRoomService hotelRoomService, IIdentityApiClient identityApiClient)
    {
        _hotelRoomService = hotelRoomService;
        _identityApiClient = identityApiClient;
    }
    
    /// <summary>
    /// Incoming Post
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = ConstRole.Partner, AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public override async Task<Ecq211InsertHotelRoomResponse> ProcessRequest(Ecq211InsertHotelRoomRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq211InsertHotelRoomResponse());
    }
    
    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq211InsertHotelRoomResponse> Exec(Ecq211InsertHotelRoomRequest request)
    {
        return await _hotelRoomService.InsertHotelRoom(request, _identityEntity);
    }
    
    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq211InsertHotelRoomResponse ErrorCheck(Ecq211InsertHotelRoomRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq211InsertHotelRoomResponse { Success = false };
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
