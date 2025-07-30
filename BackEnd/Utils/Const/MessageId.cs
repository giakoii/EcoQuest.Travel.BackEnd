namespace BackEnd.Utils.Const;

/// <summary>
///  Message Id
/// </summary>
public class MessageId
{
    /// <summary>
    /// Information error
    /// </summary>
    public readonly static string I00000 = "I00000";
    
    /// <summary>
    /// Processing successful,I00001,{0} Processing completed successfully. {1},,
    /// </summary>
    public readonly static string I00001 = "I00001";
    
    /// <summary>
    /// 
    /// </summary>
    public readonly static string I00002 = "I00002";
    
    /// <summary>
    ///
    /// </summary>
    public readonly static string I00003 = "I00003";
    
    /// <summary>
    /// 
    /// </summary>
    public readonly static string I00004 = "I00004";

    /// <summary>
    /// Specific error,E00000, {0},,
    /// </summary>
    public readonly static string E00000 = "E00000";
    
    /// <summary>
    /// Api connection error,E99001, API connection error. ,,
    /// </summary>
    public readonly static string E99001 = "E99001";
    
    /// <summary>
    /// Data conflict error
    /// </summary>
    public readonly static string E99002 = "E99002";
    
    /// <summary>
    /// SQL timeout error
    /// </summary>
    public readonly static string E99003 = "E99003";
    
    /// <summary>
    /// View definition changed
    /// </summary>
    public readonly static string E99004 = "E99004";
    
    /// <summary>
    /// Unknown SQL error
    /// </summary>
    public readonly static string E99005 = "E99005";
    
    /// <summary>
    /// Other error: A system error occurred.\nPlease contact your system administrator.
    /// </summary>
    public readonly static string E99999 = "E99999";
    
    /// <summary>
    /// Required error,E10000, Input error. Check the message of the error item. ,,
    /// </summary>
    public readonly static string E10000 = "E10000";

    /// <summary>
    /// User not found
    /// </summary>
    public readonly static string E11001 = "E11001";
    
    /// <summary>
    /// Password is incorrect
    /// </summary>
    public readonly static string E11002 = "E11002";
    
    /// <summary>
    /// Role is invalid
    /// </summary>
    public readonly static string E11003 = "E11003";
    
    /// <summary>
    /// User is exist
    /// </summary>
    public readonly static string E11004 = "E11004";
    
    /// <summary>
    /// Password is invalid
    /// </summary>
    public readonly static string E11005 = "E11005";
    
    /// <summary>
    /// Failed to get user information.
    /// </summary>
    public readonly static string E11006 = "E11006";
}