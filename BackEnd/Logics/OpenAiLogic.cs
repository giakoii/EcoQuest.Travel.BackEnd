using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BackEnd.DTOs.Ecq200;
using BackEnd.DTOs.Ecq210;
using BackEnd.DTOs.Ecq220;
using BackEnd.DTOs.Ecq230;
using BackEnd.Repositories;
using BackEnd.Utils.Const;
using DotNetEnv;

namespace BackEnd.Logics;

public class OpenAiLogic
{
    private readonly IBaseRepository<Models.SystemConfig, Guid> _systemConfigRepository;
    private readonly HttpClient _httpClient;

    public OpenAiLogic(HttpClient httpClient, IBaseRepository<Models.SystemConfig, Guid> systemConfigRepository)
    {
        _httpClient = httpClient;
        _systemConfigRepository = systemConfigRepository;
    }

    /// <summary>
    /// Ask a question to ChatGPT and get the response.
    /// </summary>
    /// <param name="userQuestion"></param>
    /// <returns></returns>
    public async Task<IntentResult> AskChatGpt(List<Ecq210HotelEntity> hotels, List<Ecq230AttractionEntity> attractions, List<Ecq220RestaurantEntity> restaurants, List<Ecq200DestinationEntity> destinations,
        string startDate, string endDate, string numberOfPeople, string totalEstimatedCost)
    {
        var apiKey = _systemConfigRepository
            .Find(x => x.Id == Utils.Const.SystemConfig.ApiKeyAi, false)
            .FirstOrDefault()?.Value;
        
        var url = "https://api.openai.com/v1/chat/completions";
        // var prompt = @$"Hãy dựa vào các dữ liệu từ app của tôi gồm có Nhà hàng (restaurant), Điểm tham quan (attraction), Khách sạn (hotel), và các điểm đến (destination):
        //         - Thông tin nhà hàng gồm có:
        //             + Tên nhà hàng: {string.Join(", ", restaurants.Select(x => x.))},";

        string prompt = string.Empty;
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "Bạn là hệ thống phân tích để hiểu ý định người dùng trong ứng dụng du lịch."
                },
                new { role = "user", content = prompt }
            },
            temperature = 0.3
        };

        var requestJson = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        var content = JsonDocument.Parse(result)
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        try
        {
            var parsed = JsonSerializer.Deserialize<IntentResult>(content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return parsed ?? new IntentResult { Intent = ConstantEnum.IntentType.unknown };
        }
        catch
        {
            return new IntentResult { Intent = ConstantEnum.IntentType.unknown };
        }
    }
}



public class IntentResult
{
    public ConstantEnum.IntentType Intent { get; set; }
    public List<string> Entities { get; set; }
}