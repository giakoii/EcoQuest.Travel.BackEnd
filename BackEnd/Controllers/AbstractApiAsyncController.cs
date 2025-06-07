using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace BackEnd.Controllers
{
    public abstract class AbstractApiAsyncController<T, U, V> : ControllerBase
        where T : AbstractApiRequest
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
        /// Authentication API client
        /// </summary>
        protected IIdentityApiClient _identityApiClient;

        /// <summary>
        /// Identity information
        /// </summary>
        protected IdentityEntity _identityEntity;

        /// <summary>
        /// TemplateMethod
        /// </summary>
        /// <param name="request"></param>
        /// <param name="logger"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        protected async Task<U> ProcessRequest(T request, Logger logger, U returnValue)
        {
            // Get identity information 
            _identityEntity = _identityApiClient.GetIdentity(User)!;
        
            var loggingUtil = new LoggingUtil(logger, _identityEntity?.Email!);
            loggingUtil.StartLog(request);

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
}