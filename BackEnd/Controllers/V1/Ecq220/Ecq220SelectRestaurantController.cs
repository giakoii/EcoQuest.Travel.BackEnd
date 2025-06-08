using BackEnd.DTOs.Ecq220;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq220;

/// <summary>
/// Ecq220SelectRestaurantController - Select restaurant
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq220SelectRestaurantController : AbstractApiAsyncControllerNotToken<Ecq220SelectRestaurantRequest, Ecq220SelectRestaurantResponse, Ecq220RestaurantDetailEntity>
{
    private readonly IRestaurantService _restaurantService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="restaurantService"></param>
    public Ecq220SelectRestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq220SelectRestaurantResponse> ProcessRequest([FromQuery] Ecq220SelectRestaurantRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq220SelectRestaurantResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq220SelectRestaurantResponse> Exec(Ecq220SelectRestaurantRequest request)
    {
        return await _restaurantService.SelectRestaurant(request.RestaurantId);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq220SelectRestaurantResponse ErrorCheck(Ecq220SelectRestaurantRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq220SelectRestaurantResponse() { Success = false };
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
