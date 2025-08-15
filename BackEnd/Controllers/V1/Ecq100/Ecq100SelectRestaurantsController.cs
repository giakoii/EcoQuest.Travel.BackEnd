using BackEnd.DTOs.Ecq100;
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
public class Ecq100SelectRestaurantsController : AbstractApiAsyncControllerNotToken<Ecq100SelectRestaurantsRequest, Ecq100SelectRestaurantsResponse, List<Ecq100SelectRestaurantsEntity>>
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
    public override async Task<Ecq100SelectRestaurantsResponse> ProcessRequest([FromQuery] Ecq100SelectRestaurantsRequest request)
    {
        return await ProcessRequest(request, _logger, new Ecq100SelectRestaurantsResponse());
    }

    /// <summary>
    /// Main processing
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected override async Task<Ecq100SelectRestaurantsResponse> Exec(Ecq100SelectRestaurantsRequest request)
    {
        return await _restaurantService.Ecq100SelectRestaurants(request);
    }

    /// <summary>
    /// Error Check
    /// </summary>
    /// <param name="request"></param>
    /// <param name="detailErrorList"></param>
    /// <returns></returns>
    protected internal override Ecq100SelectRestaurantsResponse ErrorCheck(Ecq100SelectRestaurantsRequest request, List<DetailError> detailErrorList)
    {
        var response = new Ecq100SelectRestaurantsResponse() { Success = false };
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
