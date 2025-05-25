namespace BackEnd.Utils;

/// <summary>
///  StringUtil
/// </summary>
public class StringUtil
{
    /// <summary>
    ///  Convert the first character of the string to uppercase
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToLowerCase(string value)
    {
        if (value == null) 
            return null;
        if (value.Length <= 0) return string.Empty;

        return char.ToLower(value[0]) + value.Substring(1);
    }
    
    
    #region ConvertToMoney
    /// <summary>
    /// Convert to VND
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToVND(decimal value)
    {
        return value.ToString("#,##0") + " VND";
    }
    
    /// <summary>
    /// Convert to VND
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToVND(int value)
    {
        return value.ToString("#,##0") + " VND";
    }
    
    /// <summary>
    /// Convert to VND
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToVND(long value)
    {
        return value.ToString("#,##0") + " VND";
    }
    
    /// <summary>
    /// Convert to VND
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToVND(double value)
    {
        return value.ToString("#,##0") + " VND";
    }

    /// <summary>
    /// Convert to percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToPercent(decimal value)
    {
        return value.ToString() + "%";
    }
    
    /// <summary>
    /// Convert to percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToPercent(decimal? value)
    {
        return value.HasValue ? value.Value.ToString() + "%" : "";
    }
    
    /// <summary>
    /// Convert to percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToPercent(int value)
    {
        return value.ToString() + "%";
    }
    
    /// <summary>
    /// Convert to percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToPercent(long value)
    {
        return value.ToString() + "%";
    }
    
    /// <summary>
    /// Convert to percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToPercent(double value)
    {
        return value.ToString() + "%";
    }
    
    /// <summary>
    /// Convert to percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertToPercent(float value)
    {
        return value.ToString() + "%";
    }
    #endregion

    #region Date Time

    /// <summary>
    /// Convert to DD/MM/YYYY
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ConvertToDateAsDdMmYyyy(DateOnly date)
    {
        return date.ToString("dd/MM/yyyy");
    }
    
    /// <summary>
    /// Convert to DD/MM/YYYY
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ConvertToDateAsDdMmYyyy(DateOnly? date)
    {
        return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : string.Empty;
    }
    
    /// <summary>
    /// Convert to DD/MM/YYYY
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ConvertToDateAsDdMmYyyy(DateTime? date)
    {
        return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : string.Empty;
    }
    
    /// <summary>
    /// Convert to MM/DD/YYYY
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ConvertToDateAsMmDdYyyy(DateOnly date)
    {
        return date.ToString("MM/dd/yyyy");
    }
    
    /// <summary>
    /// Convert to MM/DD/YYYY
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ConvertToDateAsMmDdYyyy(DateTime date)
    {
        return date.ToString("MM/dd/yyyy");
    }

    #endregion
}