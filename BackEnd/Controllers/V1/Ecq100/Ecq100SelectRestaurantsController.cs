using BackEnd.DTOs.Ecq220;
using BackEnd.Services;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers.V1.Ecq100;

/// <summary>
/// Ecq220SelectRestaurantsController - Select restaurants
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class Ecq100SelectRestaurantsController : AbstractApiAsyncControllerNotToken<Ecq220SelectRestaurantsRequest, Ecq220SelectRestaurantsResponse, List<Ecq220RestaurantEntity>>
{
    private readonly IRestaurantService _restaurantService;
    private Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="restaurantService"></param>
    public Ecq100SelectRestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    /// <summary>
    /// Incoming Get
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public override async Task<Ecq220SelectRestaurantsResponse> ProcessRequest([FromQuery] Ecq220SelectRestaurantsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq220SelectRestaurantsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq220SelectRestaurantsResponse> Exec(Ecq220SelectRestaurantsRequest request)
    {
        return await _restaurantService.Ecq100SelectRestaurants();
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq220SelectRestaurantsResponse ErrorCheck(Ecq220SelectRestaurantsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq220SelectRestaurantsResponse() { Success = false };
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
