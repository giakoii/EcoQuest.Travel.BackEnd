using BackEnd.DTOs.Ecq210;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq210SelectHotelsController - Select hotels
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100SelectHotelsController : AbstractApiAsyncControllerNotToken<Ecq210SelectHotelsRequest, Ecq210SelectHotelsResponse, List<Ecq210HotelEntity>>
{
    private readonly IHotelService _hotelService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="hotelService"></param>
    public Ecq100SelectHotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
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
        return await _hotelService.Ecq100SelectHotels();
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
