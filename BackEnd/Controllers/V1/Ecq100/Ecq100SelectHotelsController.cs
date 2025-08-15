using BackEnd.DTOs.Ecq100;
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
public class Ecq100SelectHotelsController : AbstractApiAsyncControllerNotToken<Ecq100SelectHotelsRequest, Ecq100SelectHotelsResponse, List<Ecq100SelectHotelsEntity>>
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
    public override async Task<Ecq100SelectHotelsResponse> ProcessRequest([FromQuery] Ecq100SelectHotelsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100SelectHotelsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100SelectHotelsResponse> Exec(Ecq100SelectHotelsRequest request)
    {
        return await _hotelService.Ecq100SelectHotels(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100SelectHotelsResponse ErrorCheck(Ecq100SelectHotelsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100SelectHotelsResponse() { Success = false };
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
