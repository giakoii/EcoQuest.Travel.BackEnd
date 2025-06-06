using BackEnd.DTOs.Ecq211;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq211;

/// <summary>
/// Ecq211SelectHotelRoomController - Select a specific hotel room by ID
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq211SelectHotelRoomController : AbstractApiAsyncControllerNotToken<Ecq211SelectHotelRoomRequest, Ecq211SelectHotelRoomResponse, Ecq211HotelRoomDetailEntity>
{
    private readonly IHotelRoomService _hotelRoomService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelRoomService"></param>
    public Ecq211SelectHotelRoomController(IHotelRoomService hotelRoomService)
    {
        _hotelRoomService = hotelRoomService;
    }
    
    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq211SelectHotelRoomResponse> ProcessRequest([FromQuery] Ecq211SelectHotelRoomRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq211SelectHotelRoomResponse());
    }
    
    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq211SelectHotelRoomResponse> Exec(Ecq211SelectHotelRoomRequest request)
    {
        return await _hotelRoomService.SelectHotelRoom(request.RoomId);
    }
    
    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq211SelectHotelRoomResponse ErrorCheck(Ecq211SelectHotelRoomRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq211SelectHotelRoomResponse { Success = false };
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
