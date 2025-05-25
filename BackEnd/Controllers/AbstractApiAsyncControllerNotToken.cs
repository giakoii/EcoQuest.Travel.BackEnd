using BackEnd.Utils;
using Microsoft.AspNetCore.Mvc;
using NLog;


namespace BackEnd.Controllers;

public abstract class AbstractApiAsyncControllerNotToken
    <T, U, V> : ControllerBase
    where T : class
    where U : AbstractApiResponse<V>
{
    /// <summary>
    /// API entry point
    /// </summary>
    public abstract Task<U> ProcessRequest(T request);

    /// <summary>
    /// Main processing
    /// </summary>
    protected abstract Task<U> Exec(T request);

    /// <summary>
    /// Error check
    /// </summary>
    protected internal abstract U ErrorCheck(T request, List<DetailError> detailErrorList);

    /// <summary>
    /// TemplateMethod
    /// </summary>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="returnValue"></param>
    /// <returns></returns>
    protected async Task<U> ProcessRequest(T request, Logger logger, U returnValue)
    {
        var loggingUtil = new LoggingUtil(logger, "System");
        loggingUtil.StartLog(request);
        try
        {
            // Error check
            var detailErrorList = AbstractFunction<U, V>.ErrorCheck(this.ModelState);
            returnValue = ErrorCheck(request, detailErrorList);

            // If there is no error, execute the main process
            if (returnValue.Success) returnValue = await Exec(request);
        }
        catch (Exception e)
        {
            return AbstractFunction<U, V>.GetReturnValue(returnValue, loggingUtil, e);
        }

        // Processing end log
        loggingUtil.EndLog(returnValue);
        return returnValue;
    }
}