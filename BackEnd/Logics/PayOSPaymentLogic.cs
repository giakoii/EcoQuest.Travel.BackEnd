using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BackEnd.DTOs.Ecq110;
using BackEnd.Repositories;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Mvc;
using SystemConfig = BackEnd.Models.SystemConfig;

namespace BackEnd.Logics;

public class PayOsPaymentLogic
{
    private readonly IBaseRepository<SystemConfig, Guid> _systemConfigRepository;

    public PayOsPaymentLogic(IBaseRepository<SystemConfig, Guid> systemConfigRepository)
    {
        _systemConfigRepository = systemConfigRepository;
    }

    public async Task<Ecq110InsertPaymentResponse> PayBooking(Guid bookingId, string tripName)
    {
        var response = new Ecq110InsertPaymentResponse { Success = false };

        // Tạo request cho PayOS
        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); 
        
        // Limit description to 25 characters
        var description = $"Thanh toán {tripName}";
        if (description.Length > 25)
            description = description.Substring(0, 25);    
        
        var returnUrl = $"http://160.250.246.33:5269/api/v1/Ecq100SelectHotels";
        var cancelUrl = $"http://160.250.246.33:5269/api/v1/Ecq100SelectHotels";
        
        var payOsCheckSumKey = _systemConfigRepository.Find(x => x.Id == Utils.Const.SystemConfig.PayOsCheckSumKey).FirstOrDefault()!.Value;
        var payOsApiKey = _systemConfigRepository.Find(x => x.Id == Utils.Const.SystemConfig.PayOsApiKey).FirstOrDefault()!.Value;
        var payOsClientId = _systemConfigRepository.Find(x => x.Id == Utils.Const.SystemConfig.PayOsClientId).FirstOrDefault()!.Value;

        
        var data = $"amount={2000}&cancelUrl={cancelUrl}" +
                   $"&description={description}" +
                   $"&orderCode={orderCode}" +
                   $"&returnUrl={returnUrl}";
        string signature = ComputeHmacSha256(data, payOsCheckSumKey);

        var payRequest = new
        {
            orderCode = orderCode,
            amount = 2000,
            description = description,
            returnUrl = returnUrl,
            cancelUrl = cancelUrl,
            signature = signature,
        };
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-client-id", payOsClientId);
        client.DefaultRequestHeaders.Add("x-api-key", payOsApiKey);

        var jsonContent = new StringContent(JsonSerializer.Serialize(payRequest), Encoding.UTF8, "application/json");

        var payosResponse = await client.PostAsync("https://api-merchant.payos.vn/v2/payment-requests", jsonContent);

        if (!payosResponse.IsSuccessStatusCode)
        {
            response.SetMessage(MessageId.E00000, "Failed to create payment request");
            return response;
        }

        var responseContent = await payosResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseContent);
        var root = jsonDoc.RootElement;

        var dataElement = root.GetProperty("data");

        var checkoutUrl = dataElement.GetProperty("checkoutUrl").GetString();
        var qrCode = dataElement.GetProperty("qrCode").GetString();
        if (string.IsNullOrEmpty(checkoutUrl) || string.IsNullOrEmpty(qrCode))
        {
            response.SetMessage(MessageId.E00000, "Failed to retrieve payment data");
            return response;
        }

        var entityResponse = new Ecq110InsertPaymentEntity
        {
            CheckoutUrl = checkoutUrl,
            QrCode = qrCode,
        };

        response.Success = true;
        response.Response = entityResponse;
        return response;
    }
    
    public static string ComputeHmacSha256(string message, string secretKey)
    {
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
        if (string.IsNullOrWhiteSpace(secretKey)) throw new ArgumentNullException(nameof(secretKey));

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

        // Convert to lowercase hex string without hyphens
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hashString;
    }
}