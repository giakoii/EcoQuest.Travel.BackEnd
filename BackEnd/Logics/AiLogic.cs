using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BackEnd.DTOs.Ecq200;
using BackEnd.DTOs.Ecq210;
using BackEnd.DTOs.Ecq220;
using BackEnd.DTOs.Ecq230;
using BackEnd.DTOs.Ecq110;
using BackEnd.Repositories;
using BackEnd.Utils.Const;
using DotNetEnv;

namespace BackEnd.Logics;

public class AiLogic
{
    private readonly IBaseRepository<Models.SystemConfig, Guid> _systemConfigRepository;
    private readonly HttpClient _httpClient;

    public AiLogic(HttpClient httpClient, IBaseRepository<Models.SystemConfig, Guid> systemConfigRepository)
    {
        _httpClient = httpClient;
        _systemConfigRepository = systemConfigRepository;
    }
    

    /// <summary>
    /// Ask AI to generate detailed trip schedule based on available data
    /// </summary>
    /// <param name="hotels"></param>
    /// <param name="attractions"></param>
    /// <param name="restaurants"></param>
    /// <param name="destinations"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="numberOfPeople"></param>
    /// <param name="totalEstimatedCost"></param>
    /// <param name="additionalPreferences"></param>
    /// <returns></returns>
    public async Task<List<Ecq110InsertTripScheduleResponseDetail>> AskAiModel(List<Ecq210HotelEntity> hotels, List<Ecq230AttractionDetailEntity> attractions, List<Ecq220RestaurantEntity> restaurants, List<Ecq200DestinationDetailEntity> destinations,
        string startDate, string endDate, string numberOfPeople, string totalEstimatedCost)
    {
        
        var apiKey = _systemConfigRepository
            .Find(x => x.Id == Utils.Const.SystemConfig.ApiKeyAi, false)
            .FirstOrDefault()?.Value;
        
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("AI API Key not configured in system settings");
        }

        var url = "https://api.groq.com/openai/v1/chat/completions";
        
        // Build detailed hotel information
        var hotelInfo = hotels.Any() ? string.Join("\n", hotels.Select(h => 
            $"- {h.Name}: Giá từ {h.MinPrice:C} - {h.MaxPrice:C}, Địa chỉ: {h.AddressLine}, {h.District}, {h.Province}, " +
            $"Đánh giá: {h.AverageRating}/5 ({h.TotalRatings} reviews), ID: {h.HotelId}")) : "Không có khách sạn khả dụng";

        // Build detailed attraction information  
        var attractionInfo = attractions.Any() ? string.Join("\n", attractions.Select(a => 
            $"- {a.AttractionName}: Phí vào cửa {a.TicketPrice:C}, Loại: {a.AttractionType}, " +
            $"Địa điểm: {a.DestinationName}, Đánh giá: {a.AverageRating}/5 ({a.TotalRatings} reviews), AttrationId: {a.AttractionId}")) : "Không có điểm tham quan khả dụng";

        // Build detailed restaurant information
        var restaurantInfo = restaurants.Any() ? string.Join("\n", restaurants.Select(r => 
            $"- {r.RestaurantName}: Giá từ {r.MinPrice:C} - {r.MaxPrice:C}, Giờ mở cửa: {r.OpenTime} - {r.CloseTime}, " +
            $"Địa chỉ: {r.AddressLine}, Địa điểm: {r.DestinationName}, Đánh giá: {r.AverageRating}/5 ({r.TotalRatings} reviews), RestaurantId: {r.RestaurantId}")) : "Không có nhà hàng khả dụng";

        // Build destination information
        var destinationInfo = destinations.Any() ? string.Join("\n", destinations.Select(d => 
            $"- {d.Name}: {d.Description}, Địa chỉ: {d.AddressLine}, {d.District}, {d.Province}, RestaurantId: {d.DestinationId}")) : "Không có điểm đến khả dụng";

        var prompt = $@"Bạn là một chuyên gia lập kế hoạch du lịch chuyên nghiệp. Hãy tạo một lịch trình du lịch chi tiết dựa trên các thông tin sau:

        **THÔNG TIN CHUYẾN ĐI:**
        - Ngày bắt đầu: {startDate}
        - Ngày kết thúc: {endDate}  
        - Số người: {numberOfPeople}
        - Tổng ngân sách ước tính: {totalEstimatedCost} VND

        **DỮ LIỆU CÓ SẴN:**

        **KHÁCH SẠN:**
        {hotelInfo}

        **ĐIỂM THAM QUAN:**
        {attractionInfo}

        **NHÀ HÀNG:**
        {restaurantInfo}

        **ĐIỂM ĐẾN:**
        {destinationInfo}

        **YÊU CẦU TẠO LỊCH TRÌNH:**
        1. Tạo lịch trình chi tiết từng ngày từ {startDate} đến {endDate}
        2. Bao gồm thời gian cụ thể cho từng hoạt động (StartTime, EndTime)
        3. Ước tính chi phí hợp lý cho từng hoạt động
        4. Ưu tiên các địa điểm có đánh giá cao và phù hợp với ngân sách
        5. Sắp xếp hợp lý về mặt địa lý để tối ưu di chuyển
        6. Cân bằng giữa nghỉ ngơi, tham quan, và ăn uống
        7. Đảm bảo tổng chi phí không vượt quá ngân sách

        **ĐỊNH DẠNG PHẢN HỒI (JSON):**
        Trả về JSON với cấu trúc sau:
        ```json
        {{
          ""tripScheduleDetails"": [
            {{
              ""scheduleDate"": ""YYYY-MM-DD"",
              ""title"": ""Tên hoạt động"",
              ""description"": ""Mô tả chi tiết hoạt động"",
              ""startTime"": ""HH:mm"",
              ""endTime"": ""HH:mm"",
              ""address"": ""Địa chỉ cụ thể"",
              ""estimatedCost"": 000000 (số tiền phải ước tính được số người là {numberOfPeople} và giá dịch vụ),
              ""reasonEstimatedCost"": ""Lý do tính ra được mức ước tính chi phí dựa vào các thông tin trên và cách tính để đưa ra con số này"",
              ""serviceId"": ""guid-nếu-có"",
              ""serviceType"": ""Hotel/Restaurant/Attraction  ?? null""
            }}
          ],
        }}

        **LỰU Ý QUAN TRỌNG:**
        - Chỉ sử dụng các địa điểm có trong dữ liệu được cung cấp
        - Bao gồm serviceId khi sử dụng khách sạn, nhà hàng, hoặc điểm tham quan cụ thể
        - Thời gian phải hợp lý (VD: 09:00-12:00 tham quan, 12:00-13:30 ăn trưa)
        - Ước tính chi phí dựa trên giá được cung cấp và số người, phải đưa ra chi phí hợp lý nhất có thể
        - Trả về CHÍNH XÁC định dạng JSON, không có text bổ sung
        - serviceId và serviceType có mối liên hệ với nhau chứ không phải sử dụng id của dịch vụ khác, Nếu destination không có địa điểm thích hợp như mong muốn thì không cần trả về serviceId và serviceType
        - Nếu có check-in khách sạn thì phải có check-out khách sạn";

        var requestBody = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "Bạn là chuyên gia lập kế hoạch du lịch chuyên nghiệp. Luôn trả về JSON hợp lệ theo đúng format được yêu cầu."
                },
                new { role = "user", content = prompt }
            },
        };

        var requestJson = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonDocument.Parse(result);
            var content = jsonResponse
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            // Clean up the response to ensure it's valid JSON
            var cleanContent = content?.Trim();
            if (cleanContent?.StartsWith("```json") == true)
            {
                cleanContent = cleanContent.Substring(7);
            }
            if (cleanContent?.EndsWith("```") == true)
            {
                cleanContent = cleanContent.Substring(0, cleanContent.Length - 3);
            }
            cleanContent = cleanContent?.Trim();

            // Parse the AI response to the correct structure
            var aiResponse = JsonSerializer.Deserialize<AiTripScheduleResponse>(cleanContent!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (aiResponse?.TripScheduleDetails == null)
            {
                return new List<Ecq110InsertTripScheduleResponseDetail>();
            }

            // Convert AI response format to our response format
            var tripScheduleDetails = aiResponse.TripScheduleDetails.Select(detail => new Ecq110InsertTripScheduleResponseDetail
            {
                ScheduleDate = DateOnly.ParseExact(detail.ScheduleDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                Title = detail.Title,
                Description = detail.Description,
                StartTime = TimeOnly.ParseExact(detail.StartTime, "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                EndTime = string.IsNullOrEmpty(detail.EndTime) ? null : TimeOnly.ParseExact(detail.EndTime, "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                Address = detail.Address,
                EstimatedCost = detail.EstimatedCost,
                ReasonEstimatedCost = detail.ReasonEstimatedCost,
                ServiceId = string.IsNullOrEmpty(detail.ServiceId) || detail.ServiceId == "null" ? null : Guid.Parse(detail.ServiceId),
                ServiceType = detail.ServiceType
            }).ToList();

            return tripScheduleDetails;
        }
        catch (Exception ex)
        { 
            throw new InvalidOperationException("Failed to call AI model for trip schedule generation", ex);
        }
    }

    /// <summary>
    /// Gửi thông tin tour có vấn đề cho AI và nhận về danh sách gợi ý tối ưu hóa (tiếng Việt)
    /// </summary>
    public async Task<List<OptimizationSuggestion>> AskAiForTripOptimization(string tripName, int bookingCount, double cancellationRate, decimal? estimatedCost, decimal averageActualCost, decimal totalRevenue)
    {
        var apiKey = _systemConfigRepository
            .Find(x => x.Id == Utils.Const.SystemConfig.ApiKeyAi, false)
            .FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("AI API Key not configured in system settings");

        var url = "https://api.groq.com/openai/v1/chat/completions";
        var prompt = $"Bạn là chuyên gia du lịch. Phân tích tour sau và đưa ra gợi ý tối ưu hóa bằng tiếng Việt.\n" +
                     $"Tên tour: {tripName}\n" +
                     $"Số lượng booking: {bookingCount}\n" +
                     $"Tỷ lệ hủy: {cancellationRate:F2}%\n" +
                     $"Chi phí ước tính: {estimatedCost}\n" +
                     $"Chi phí thực tế trung bình: {averageActualCost}\n" +
                     $"Tổng doanh thu: {totalRevenue}\n" +
                     $"Vấn đề: {(bookingCount < 5 ? "Ít lượt đặt" : "")}{(cancellationRate > 20 ? ", Tỷ lệ hủy cao" : "")}\n" +
                     "Hãy đề xuất các giải pháp cụ thể (marketing, giá, dịch vụ, lịch trình...) để cải thiện tour này. Trả về JSON dạng: [ { \"Type\": \"Loại đề xuất\", \"Title\": \"Tiêu đề\", \"Description\": \"Mô tả chi tiết\", \"ExpectedImpact\": \"Tác động kỳ vọng (%)\", \"Difficulty\": \"Độ khó\" } ]";

        var requestBody = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "system", content = "Bạn là chuyên gia lập kế hoạch du lịch chuyên nghiệp. Luôn trả về JSON hợp lệ theo đúng format được yêu cầu." },
                new { role = "user", content = prompt }
            },
        };
        var requestJson = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"AI API call failed: {response.StatusCode} - {errorContent}");
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseContent);
        var aiText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        var cleanText = aiText?.Trim();
        if (cleanText?.Contains("```json") == true)
        {
            var start = cleanText.IndexOf("```json") + 7;
            var end = cleanText.LastIndexOf("```");
            cleanText = cleanText.Substring(start, end - start).Trim();
        }
        else
        {
            var start = cleanText.IndexOf('[');
            var end = cleanText.LastIndexOf(']');
            if (start >= 0 && end > start)
                cleanText = cleanText.Substring(start, end - start + 1);
        }
        var suggestions = JsonSerializer.Deserialize<List<OptimizationSuggestion>>(cleanText ?? "[]");
        return suggestions ?? new List<OptimizationSuggestion>();
    }
}

public class AiTripScheduleResponse
{
    public List<TripScheduleDetail> TripScheduleDetails { get; set; } = new();
}

public class TripScheduleDetail
{
    public string ScheduleDate { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    
    public string? ReasonEstimatedCost { get; set; }
    public string? ServiceId { get; set; }
    public string? ServiceType { get; set; }
}

public class OptimizationSuggestion
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ExpectedImpact { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
}
