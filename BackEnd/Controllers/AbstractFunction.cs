using System.Text.RegularExpressions;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers;

public class AbstractFunction<U, V>
    where U : AbstractApiResponse<V>
{
    /// <summary>
    /// Return value
    /// </summary>
    public static U GetReturnValue(U returnValue, LoggingUtil loggingUtil, Exception e)
    {
        switch (e)
        {
            case AggregateException:
                loggingUtil.FatalLog($"Report API connection error ：{e.Message}");
                returnValue.SetMessage(MessageId.E99001);
                break;
            case DbUpdateConcurrencyException:
                loggingUtil.ErrorLog($"Exclusive error ：{e.Message}");
                returnValue.SetMessage(MessageId.E99002);
                break;
            case InvalidOperationException:
                if (e.InnerException?.HResult == -2146233088)
                {
                    // Exclusive control error Another update in the transaction
                    loggingUtil.ErrorLog($"Exclusive error ：{e.Message}");
                    returnValue.SetMessage(MessageId.E99002);
                }
                else
                {
                    loggingUtil.FatalLog($"A system error has occurred.：{e.Message} {e.StackTrace} {e.InnerException}");
                    returnValue.SetMessage(MessageId.E99999);
                }

                break;
            case SqlException ex:
                if (ex.InnerException?.HResult == -2147467259)
                {
                    // SQL timeout
                    loggingUtil.ErrorLog($"SQL timeout error ：{ex.Message} {ex.StackTrace} {ex.InnerException}");
                    returnValue.SetMessage(MessageId.E99003);
                }
                else if (ex.SqlState == "42P01") // Example: Undefined table
                {
                    loggingUtil.ErrorLog(
                        $"The definition of a view, etc. was changed during processing ：{ex.Message} {ex.StackTrace} {ex.InnerException}");
                    returnValue.SetMessage(MessageId.E99004);
                }
                else
                {
                    loggingUtil.ErrorLog($"A system error has occurred ：{ex.Message} {ex.StackTrace} {ex.InnerException}");
                    returnValue.SetMessage(MessageId.E99005);
                }
                break;
            case Exception:
                // System error
                loggingUtil.ErrorLog($"A system error has occurred ：{e.Message} {e.StackTrace} {e.InnerException}");
                returnValue.SetMessage(MessageId.E99999);
                break;
        }

        returnValue.Success = false;
        loggingUtil.EndLog(returnValue);
        return returnValue;
    }


    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="modelState"></param>
    /// <returns></returns>
    public static List<DetailError> ErrorCheck(ModelStateDictionary modelState)
    {
        var detailErrorList = new List<DetailError>();

        // If there is no error, return
        if (modelState.IsValid)
            return detailErrorList;

        foreach (var entry in modelState)
        {
            var key = entry.Key;
            var modelStateEntity = entry.Value;

            if (modelStateEntity == null || modelStateEntity.ValidationState == ModelValidationState.Valid)
                continue;

            // Remove the prefix "Value." from the key
            var keyReplace = Regex.Replace(key, @"^Value\.", "");
            keyReplace = Regex.Replace(keyReplace, @"^Value\[\d+\]\.", "");

            // Get error message
            var errorMessage = string.Join("; ", modelStateEntity.Errors.Select(e => e.ErrorMessage));

            var detailError = new DetailError();
            Match matchesKey;

            // Extract information from the key in the structure: object[index].property
            if ((matchesKey = new Regex(@"^(.*?)\[(\d+)\]\.(.*?)$").Match(keyReplace)).Success)
            {
                // In the case of a list
                detailError.field = matchesKey.Groups[1].Value;
            }
            else
            {
                // In the case of a single item
                detailError.field = keyReplace.Split('.').LastOrDefault();
            }

            // Convert the field name to lowercase
            detailError.field = StringUtil.ToLowerCase(detailError.field);

            // Set the error message
            detailError.ErrorMessage = errorMessage;

            detailErrorList.Add(detailError);
        }

        return detailErrorList;
    }
}