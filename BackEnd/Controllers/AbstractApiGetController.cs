using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;


namespace BackEnd.Controllers;

public abstract class AbstractApiGetController<T, U, V> : ControllerBase
    where U : AbstractApiResponse<V>
    where T : class 
{
    protected IIdentityApiClient _identityApiClient;

    protected IdentityEntity _identityEntity;

    public abstract U Get([FromQuery] T request);

    /// <summary> Main processing method for GET with filters </summary>
    protected abstract U Exec(T request);

    /// <summary> Error check method for GET </summary>
    protected internal abstract U ErrorCheck(T request,List<DetailError> detailErrorList);

    /// <summary>
    /// Template method for GET with filters
    /// </summary>
    protected U Get(T request, Logger logger, U returnValue)
    {        
        // Get identity information 
        _identityEntity = _identityApiClient.GetIdentity(User);

        var loggingUtil = new LoggingUtil(logger, _identityEntity?.Email ?? "System");
        loggingUtil.StartLog();

        // Check authentication information
        if (_identityEntity == null)
        {
            // Authentication error
            loggingUtil.FatalLog($"Authenticated, but information is missing.");
            returnValue.Success = false;
            returnValue.SetMessage(MessageId.E11006);
            loggingUtil.EndLog(returnValue);
            return returnValue;
        }
        try
        {
                var detailErrorList = AbstractFunction<U, V>.ErrorCheck(this.ModelState);
                returnValue = ErrorCheck(request, detailErrorList);

                if (returnValue.Success) returnValue = Exec(request);
        }
        catch (Exception e)
        {
            return AbstractFunction<U, V>.GetReturnValue(returnValue, loggingUtil, e);
        }

        loggingUtil.EndLog(returnValue);
        return returnValue;
    }
}