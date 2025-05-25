using BackEnd.Utils;
using Microsoft.AspNetCore.Mvc;
using NLog;


namespace BackEnd.Controllers;

public abstract class AbstractApiGetAsyncControllerNotToken<T, U, V> : ControllerBase
    where U : AbstractApiResponse<V>
    where T : class 
{
    public abstract Task<U> Get([FromQuery] T request);

    /// <summary>
    /// Main processing method for GET with filters
    /// </summary>
    protected abstract Task<U> Exec(T request);

    /// <summary>
    /// Error check method for GET
    /// </summary>
    protected internal abstract U ErrorCheck(T request,List<DetailError> detailErrorList);

    /// <summary>
    /// Template method for GET with filters
    /// </summary>
    protected async Task<U> Get(T request, Logger logger, U returnValue)
    {
        var loggingUtil = new LoggingUtil(logger, "System");
        loggingUtil.StartLog();
        try
        {
                var detailErrorList = AbstractFunction<U, V>.ErrorCheck(this.ModelState);
                returnValue = ErrorCheck(request, detailErrorList);

                if (returnValue.Success) returnValue = await Exec(request);
        }
        catch (Exception e)
        {
            return AbstractFunction<U, V>.GetReturnValue(returnValue, loggingUtil, e);
        }

        loggingUtil.EndLog(returnValue);
        return returnValue;
    }
}