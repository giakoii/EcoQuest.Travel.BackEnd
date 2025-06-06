using BackEnd.DTOs.Ecq211;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq211;

/// <summary>
/// Ecq211SelectHotelRoomsController - Select all rooms for a specific hotel
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq211SelectHotelRoomsController : AbstractApiAsyncControllerNotToken<Ecq211SelectHotelRoomsRequest, Ecq211SelectHotelRoomsResponse, List<Ecq211HotelRoomEntity>>
{
    private readonly IHotelRoomService _hotelRoomService;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelRoomService"></param>
    public Ecq211SelectHotelRoomsController(IHotelRoomService hotelRoomService)
    {
        _hotelRoomService = hotelRoomService;
    }
    
    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq211SelectHotelRoomsResponse> ProcessRequest([FromQuery] Ecq211SelectHotelRoomsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq211SelectHotelRoomsResponse());
    }
    
    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq211SelectHotelRoomsResponse> Exec(Ecq211SelectHotelRoomsRequest request)
    {
        return await _hotelRoomService.SelectHotelRooms(request.HotelId);
    }
    
    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq211SelectHotelRoomsResponse ErrorCheck(Ecq211SelectHotelRoomsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq211SelectHotelRoomsResponse { Success = false };
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
