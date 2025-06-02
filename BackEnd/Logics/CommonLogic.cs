using System.Security.Cryptography;
using System.Text;
using BackEnd.Models;
using BackEnd.Models.Helpers;
using BackEnd.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Logics;

/// <summary>
/// Common logic
/// </summary>
public static class CommonLogic
{
    /// <summary>
    /// Encrypt the text
    /// </summary>
    /// <param name="beforeEncrypt"></param>
    /// <param name="systemConfigRepository"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string EncryptText(string beforeEncrypt, IBaseRepository<SystemConfig, string> systemConfigRepository)
    {
        // Check for null or empty
        ArgumentException.ThrowIfNullOrEmpty(beforeEncrypt);
        
        // Get the system config
        var key = systemConfigRepository.Find(x => x.Id == Utils.Const.SystemConfig.EncryptKey, false).FirstOrDefault()!.Value;
        var iv = systemConfigRepository.Find(x => x.Id == Utils.Const.SystemConfig.EncryptIv, false).FirstOrDefault()!.Value;
        // Check for null
        if (key == null)
        {
            throw new ArgumentException();
        }
        // Encrypt the text
        using (Aes aes = Aes.Create())
        {
            // Set the key and IV
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            
            // Encrypt
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(beforeEncrypt);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }   
        }
    }

    /// <summary>
    /// Decrypt the text
    /// </summary>
    /// <param name="beforeDecrypt"></param>
    /// <param name="systemConfigRepository"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string DecryptText(string beforeDecrypt, IBaseRepository<SystemConfig, string> systemConfigRepository)
    {
        // Check for null or empty
        ArgumentException.ThrowIfNullOrEmpty(beforeDecrypt);
        // Get the system config
        var key = systemConfigRepository.Find(x => x.Id == Utils.Const.SystemConfig.EncryptKey, false).FirstOrDefault()!.Value;
        var iv = systemConfigRepository.Find(x => x.Id == Utils.Const.SystemConfig.EncryptIv, false).FirstOrDefault()!.Value;
        // Check for null
        if (key == null)
        {
            throw new ArgumentException();
        }
        // Decrypt the text
        using (Aes aes = Aes.Create())
        {
            // Set the key and IV
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            // Decrypt
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(beforeDecrypt)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Generates a random password that meets complexity requirements
    /// </summary>
    /// <param name="length">Password length (minimum 8)</param>
    /// <returns>A secure random password</returns>
    public static string GenerateRandomPassword(int length = 12)
    {
        // Ensure minimum length of 8
        length = Math.Max(8, length);
        
        // Define character sets
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChars = "0123456789";
        const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";
        
        // Create a random number generator
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        
        // Generate one character from each required set
        var password = new char[length];
        password[0] = GetRandomChar(lowerChars, rng, bytes);
        password[1] = GetRandomChar(upperChars, rng, bytes);
        password[2] = GetRandomChar(numberChars, rng, bytes);
        password[3] = GetRandomChar(specialChars, rng, bytes);
        
        // Combine all character sets for remaining positions
        string allChars = lowerChars + upperChars + numberChars + specialChars;
        
        // Fill remaining positions with random characters
        for (int i = 4; i < length; i++)
        {
            password[i] = GetRandomChar(allChars, rng, bytes);
        }
        
        // Shuffle the password to avoid predictable patterns
        ShuffleArray(password, rng, bytes);
        
        return new string(password);
    }
    
    /// <summary>
    /// Gets a random character from the provided character set
    /// </summary>
    private static char GetRandomChar(string charSet, RandomNumberGenerator rng, byte[] bytes)
    {
        rng.GetBytes(bytes);
        uint num = BitConverter.ToUInt32(bytes, 0);
        return charSet[(int)(num % charSet.Length)];
    }
    
    /// <summary>
    /// Fisher-Yates shuffle algorithm to randomize character positions
    /// </summary>
    private static void ShuffleArray(char[] array, RandomNumberGenerator rng, byte[] bytes)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            rng.GetBytes(bytes);
            uint num = BitConverter.ToUInt32(bytes, 0);
            int j = (int)(num % (i + 1));
            
            // Swap elements
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}