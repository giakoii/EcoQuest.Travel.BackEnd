using System.Text;
using System.Text.Json;
using BackEnd.Utils.Const;
using DotNetEnv;

namespace BackEnd.Logics;

public class OpenAiLogic
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    
    public OpenAiLogic(HttpClient httpClient)
    {
        _httpClient =  httpClient;
        Env.Load();
        _apiKey = Environment.GetEnvironmentVariable(EnvConst.OpenAiKey)!;
    }
    
    /// <summary>
    /// Ask a question to ChatGPT and get the response.
    /// </summary>
    /// <param name="userQuestion"></param>
    /// <returns></returns>
    public async Task<IntentResult> AskChatGpt(string userQuestion)
    {
        var url = "https://api.openai.com/v1/chat/completions";
        var prompt = @$"Hãy phân tích câu hỏi sau và trả về JSON theo mẫu sau:{{ ""Intent"": ""..."", ""Entities"": [""...""] }}Câu hỏi: ""{userQuestion}""";
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "Bạn là một hệ thống phân tích câu hỏi để hiểu Intent và Entities." },
                new { role = "user", content = userQuestion }
            },
            temperature = 0.7
        };

        var requestJson = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(result);

        var jsonString = JsonDocument.Parse(result)
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
        
        var intentResult = JsonSerializer.Deserialize<IntentResult>(jsonString);
        return intentResult!;
    }
}

public class IntentResult
{
    public string Intent { get; set; }
    public List<string> Entities { get; set; }
}