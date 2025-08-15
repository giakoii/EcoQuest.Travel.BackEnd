using BackEnd.DTOs.Dashboard;
using BackEnd.Models;
using BackEnd.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Logics;

public class DashboardAnalyticsLogic
{
    private readonly AppDbContext _context;
    private readonly AiLogic _aiLogic;

    public DashboardAnalyticsLogic(AppDbContext context, AiLogic aiLogic)
    {
        _context = context;
        _aiLogic = aiLogic;
    }

    // Dự báo lượt khách theo vùng
    public async Task<List<RegionForecastDto>> GetRegionForecastAsync()
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
        var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

        var bookingsData = await _context.Bookings
            .Include(b => b.Trip)
            .ThenInclude(t => t.TripDestinations)
            .ThenInclude(td => td.Destination)
            .Where(b => b.CreatedAt.Year >= currentYear - 1)
            .GroupBy(b => new
            {
                Province = b.Trip.TripDestinations.FirstOrDefault().Destination.Province,
                District = b.Trip.TripDestinations.FirstOrDefault().Destination.District,
                Month = b.CreatedAt.Month,
                Year = b.CreatedAt.Year
            })
            .Select(g => new
            {
                g.Key.Province,
                g.Key.District,
                g.Key.Month,
                g.Key.Year,
                BookingCount = g.Count(),
                Revenue = g.Sum(b => b.TotalCost)
            })
            .ToListAsync();

        var regionGroups = bookingsData
            .GroupBy(b => new { b.Province, b.District })
            .Select(g => new RegionForecastDto
            {
                Province = g.Key.Province ?? "Unknown",
                District = g.Key.District ?? "Unknown",
                CurrentMonthBookings = g.Where(x => x.Month == currentMonth && x.Year == currentYear)
                    .Sum(x => x.BookingCount),
                PreviousMonthBookings = g.Where(x => x.Month == previousMonth && x.Year == previousYear)
                    .Sum(x => x.BookingCount),
                HistoricalData = g.Select(x => new MonthlyBookingData
                {
                    Month = new DateTime(x.Year, x.Month, 1),
                    BookingCount = x.BookingCount,
                    Revenue = x.Revenue
                }).OrderBy(x => x.Month).ToList()
            })
            .ToList();

        // Tính toán growth rate và dự báo
        foreach (var region in regionGroups)
        {
            if (region.PreviousMonthBookings > 0)
            {
                region.GrowthRate = ((decimal)(region.CurrentMonthBookings - region.PreviousMonthBookings) /
                                     region.PreviousMonthBookings) * 100;
            }

            // Dự báo đơn giản dựa trên xu hướng 3 tháng gần nhất
            var recentData = region.HistoricalData.TakeLast(3).ToList();
            if (recentData.Count >= 2)
            {
                var avgGrowth = recentData.Skip(1).Select((data, index) =>
                    recentData[index].BookingCount > 0
                        ? (decimal)(data.BookingCount - recentData[index].BookingCount) / recentData[index].BookingCount
                        : 0
                ).Average();

                region.PredictedNextMonthBookings = Math.Max(0, (int)(region.CurrentMonthBookings * (1 + avgGrowth)));
            }
            else
            {
                region.PredictedNextMonthBookings = region.CurrentMonthBookings;
            }
        }

        return regionGroups.OrderByDescending(r => r.CurrentMonthBookings).ToList();
    }

    // Phân tích tour hot và tour sụt giảm
    // public async Task<(List<TripPopularityDto> hotTrips, List<TripPopularityDto> decliningTrips)> GetTripPopularityAnalysisAsync()
    // {
    //     var threeMonthsAgo = DateTime.Now.AddMonths(-3);
    //
    //     var tripData = await _context.Bookings
    //         .Include(b => b.Trip)
    //         .ThenInclude(t => t.TripDestinations)
    //         .ThenInclude(td => td.Destination)
    //         .Where(b => b.CreatedAt >= threeMonthsAgo)
    //         .GroupBy(b => new { b.TripId, b.Trip.TripName })
    //         .Select(g => new
    //         {
    //             g.Key.TripId,
    //             g.Key.TripName,
    //             Destination = g.FirstOrDefault()!.Trip.TripDestinations.FirstOrDefault()!.Destination.Name,
    //             BookingCount = g.Count(),
    //             Revenue = g.Sum(b => b.TotalCost),
    //             MonthlyData = g.GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
    //                 .Select(mg => new MonthlyTripData
    //                 {
    //                     Month = new DateTime(mg.Key.Year, mg.Key.Month, 1, 0, 0, 0, DateTimeKind.Utc),
    //                     BookingCount = mg.Count(),
    //                     Revenue = mg.Sum(b => b.TotalCost)
    //                 }).ToList()
    //         })
    //         .ToListAsync();
    //
    //     // Lấy rating trung bình từ các bảng rating
    //     var tripPopularityList = new List<TripPopularityDto>();
    //
    //     foreach (var trip in tripData)
    //     {
    //         var tripPopularity = new TripPopularityDto
    //         {
    //             TripId = trip.TripId,
    //             TripName = trip.TripName ?? "Unknown Trip",
    //             Destination = trip.Destination ?? "Unknown Destination",
    //             BookingCount = trip.BookingCount,
    //             Revenue = trip.Revenue,
    //             TrendData = trip.MonthlyData.OrderBy(m => m.Month).ToList()
    //         };
    //
    //         // Tính growth rate dựa trên 2 tháng gần nhất
    //         var recentMonths = tripPopularity.TrendData.TakeLast(2).ToList();
    //         if (recentMonths.Count == 2)
    //         {
    //             var previousMonthBookings = recentMonths[0].BookingCount;
    //             var currentMonthBookings = recentMonths[1].BookingCount;
    //
    //             if (previousMonthBookings > 0)
    //             {
    //                 tripPopularity.GrowthRate =
    //                     ((decimal)(currentMonthBookings - previousMonthBookings) / previousMonthBookings) * 100;
    //             }
    //         }
    //
    //         // Xác định trend status
    //         if (tripPopularity.GrowthRate > 5)
    //             tripPopularity.TrendStatus = "Hot";
    //         else if (tripPopularity.GrowthRate < -5)
    //             tripPopularity.TrendStatus = "Declining";
    //         else
    //             tripPopularity.TrendStatus = "Stable";
    //
    //         tripPopularityList.Add(tripPopularity);
    //     }
    //
    //     var hotTrips = tripPopularityList
    //         .Where(t => t.TrendStatus == "Hot" || t.BookingCount >= 10)
    //         .OrderByDescending(t => t.GrowthRate)
    //         .ThenByDescending(t => t.BookingCount)
    //         .Take(10)
    //         .ToList();
    //
    //     var decliningTrips = tripPopularityList
    //         .Where(t => t.TrendStatus == "Declining")
    //         .OrderBy(t => t.GrowthRate)
    //         .Take(10)
    //         .ToList();
    //
    //     return (hotTrips, decliningTrips);
    // }
    
public async Task<(List<TripPopularityDto> hotTrips, List<TripPopularityDto> decliningTrips)> GetTripPopularityAnalysisAsync()
{
    // Tạo dữ liệu ảo thay vì query database
    var mockTripData = new List<TripPopularityDto>
    {
        // Hot Trips
        new TripPopularityDto
        {
            TripId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            TripName = "Khám phá Sapa mùa thu",
            Destination = "Sapa, Lào Cai",
            BookingCount = 45,
            Revenue = 675000000m,
            AverageRating = 4.8m,
            GrowthRate = 35.7m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 8, Revenue = 120000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 14, Revenue = 210000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 23, Revenue = 345000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f23456789012"),
            TripName = "Thiên đường biển đảo Phú Quốc",
            Destination = "Phú Quốc, Kiên Giang",
            BookingCount = 38,
            Revenue = 950000000m,
            AverageRating = 4.7m,
            GrowthRate = 26.7m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 10, Revenue = 250000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 15, Revenue = 375000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 13, Revenue = 325000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-345678901234"),
            TripName = "Miền Trung di sản văn hóa",
            Destination = "Hội An - Huế - Phong Nha",
            BookingCount = 32,
            Revenue = 800000000m,
            AverageRating = 4.6m,
            GrowthRate = 23.1m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 9, Revenue = 225000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 13, Revenue = 325000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 10, Revenue = 250000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-456789012345"),
            TripName = "Vịnh Hạ Long kỳ quan",
            Destination = "Hạ Long, Quảng Ninh",
            BookingCount = 41,
            Revenue = 615000000m,
            AverageRating = 4.5m,
            GrowthRate = 17.6m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 12, Revenue = 180000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 17, Revenue = 255000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 12, Revenue = 180000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("e5f6a7b8-c9d0-1234-efab-567890123456"),
            TripName = "Đà Lạt thành phố ngàn hoa",
            Destination = "Đà Lạt, Lâm Đồng",
            BookingCount = 29,
            Revenue = 435000000m,
            AverageRating = 4.4m,
            GrowthRate = 16.0m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 8, Revenue = 120000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 10, Revenue = 150000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 11, Revenue = 165000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-678901234567"),
            TripName = "Miền Tây sông nước",
            Destination = "Cần Thơ - An Giang",
            BookingCount = 25,
            Revenue = 375000000m,
            AverageRating = 4.3m,
            GrowthRate = 13.6m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 7, Revenue = 105000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 9, Revenue = 135000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 9, Revenue = 135000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("a7b8c9d0-e1f2-3456-abcd-789012345678"),
            TripName = "Nha Trang biển xanh",
            Destination = "Nha Trang, Khánh Hòa",
            BookingCount = 36,
            Revenue = 540000000m,
            AverageRating = 4.2m,
            GrowthRate = 12.5m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 11, Revenue = 165000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 13, Revenue = 195000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 12, Revenue = 180000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("b8c9d0e1-f2a3-4567-bcde-890123456789"),
            TripName = "Mộc Châu mùa hoa",
            Destination = "Mộc Châu, Sơn La",
            BookingCount = 18,
            Revenue = 270000000m,
            AverageRating = 4.6m,
            GrowthRate = 8.8m,
            TrendStatus = "Hot",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 5, Revenue = 75000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 6, Revenue = 90000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 7, Revenue = 105000000m }
            }
        },
        // Stable trips
        new TripPopularityDto
        {
            TripId = Guid.Parse("c9d0e1f2-a3b4-5678-cdef-901234567890"),
            TripName = "Phan Thiết resort",
            Destination = "Phan Thiết, Bình Thuận",
            BookingCount = 22,
            Revenue = 330000000m,
            AverageRating = 4.1m,
            GrowthRate = 2.3m,
            TrendStatus = "Stable",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 7, Revenue = 105000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 8, Revenue = 120000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 7, Revenue = 105000000m }
            }
        },
        // Declining trips
        new TripPopularityDto
        {
            TripId = Guid.Parse("abcdef12-3456-3210-abcd-fed987654321"),
            TripName = "Tour thành phố Hà Nội",
            Destination = "Hà Nội",
            BookingCount = 15,
            Revenue = 225000000m,
            AverageRating = 3.8m,
            GrowthRate = -18.2m,
            TrendStatus = "Declining",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 8, Revenue = 120000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 5, Revenue = 75000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 2, Revenue = 30000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("abcdef12-3456-2109-abcd-edc876543210"),
            TripName = "Vũng Tàu cuối tuần",
            Destination = "Vũng Tàu, Bà Rịa - Vũng Tàu",
            BookingCount = 12,
            Revenue = 180000000m,
            AverageRating = 3.6m,
            GrowthRate = -25.0m,
            TrendStatus = "Declining",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 6, Revenue = 90000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 4, Revenue = 60000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 2, Revenue = 30000000m }
            }
        },
        new TripPopularityDto
        {
            TripId = Guid.Parse("abcdef12-3456-1098-abcd-dcb765432109"),
            TripName = "Cao nguyên Di Linh",
            Destination = "Di Linh, Lâm Đồng",
            BookingCount = 8,
            Revenue = 120000000m,
            AverageRating = 3.9m,
            GrowthRate = -33.3m,
            TrendStatus = "Declining",
            TrendData = new List<MonthlyTripData>
            {
                new MonthlyTripData { Month = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 4, Revenue = 60000000m },
                new MonthlyTripData { Month = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 3, Revenue = 45000000m },
                new MonthlyTripData { Month = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc), BookingCount = 1, Revenue = 15000000m }
            }
        }
    };

    await Task.Delay(100);

    var hotTrips = mockTripData
        .Where(t => t.TrendStatus == "Hot" || t.BookingCount >= 10)
        .OrderByDescending(t => t.GrowthRate)
        .ThenByDescending(t => t.BookingCount)
        .Take(10)
        .ToList();

    var decliningTrips = mockTripData
        .Where(t => t.TrendStatus == "Declining")
        .OrderBy(t => t.GrowthRate)
        .Take(10)
        .ToList();

    return (hotTrips, decliningTrips);
}
    // Gợi ý điều chỉnh tour
    public async Task<List<TripOptimizationDto>> GetTripOptimizationSuggestionsAsync()
    {
        var suggestions = new List<TripOptimizationDto>();
        var threeMonthsAgo = DateTime.Now.AddMonths(-3);

        // Lấy dữ liệu trip có vấn đề
        var problemTrips = await _context.Bookings
            .Include(b => b.Trip)
            .Where(b => b.CreatedAt >= threeMonthsAgo)
            .GroupBy(b => new { b.TripId, b.Trip.TripName, b.Trip.TotalEstimatedCost })
            .Select(g => new
            {
                TripId = g.Key.TripId,
                TripName = g.Key.TripName,
                EstimatedCost = g.Key.TotalEstimatedCost,
                BookingCount = g.Count(),
                AverageActualCost = g.Average(b => b.TotalCost),
                TotalRevenue = g.Sum(b => b.TotalCost),
                CancellationRate = g.Count(b => b.Status == "Cancelled") * 100.0 / g.Count()
            })
            .Where(t => t.BookingCount < 5 || t.CancellationRate > 20)
            .ToListAsync();

        foreach (var trip in problemTrips)
        {
            List<OptimizationSuggestion> aiSuggestions = new();
            try
            {
                aiSuggestions = await _aiLogic.AskAiForTripOptimization(
                    trip.TripName ?? "Unknown",
                    trip.BookingCount,
                    trip.CancellationRate,
                    trip.EstimatedCost,
                    trip.AverageActualCost,
                    trip.TotalRevenue
                );
            }
            catch
            {
                // Nếu AI lỗi, fallback về gợi ý mẫu
                if (trip.BookingCount < 5)
                {
                    aiSuggestions.Add(new OptimizationSuggestion
                    {
                        Type = "Marketing",
                        Title = "Tăng cường quảng bá",
                        Description = "Chạy quảng cáo Facebook, hợp tác KOLs để tăng nhận diện.",
                        ExpectedImpact = "30",
                        Difficulty = "Trung bình"
                    });
                }
                if (trip.CancellationRate > 20)
                {
                    aiSuggestions.Add(new OptimizationSuggestion
                    {
                        Type = "Dịch vụ",
                        Title = "Nâng cao chất lượng dịch vụ",
                        Description = "Khảo sát khách hàng, cải thiện trải nghiệm tour.",
                        ExpectedImpact = "40",
                        Difficulty = "Khó"
                    });
                }
            }

            // Chuyển đổi kiểu dữ liệu cho Suggestions
            var mappedSuggestions = aiSuggestions.Select(s => new BackEnd.DTOs.Dashboard.OptimizationSuggestion
            {
                Type = s.Type,
                Title = s.Title,
                Description = s.Description,
                ExpectedImpact = decimal.TryParse(s.ExpectedImpact?.Replace("%", "").Trim(), out var v) ? v : 0,
                Difficulty = s.Difficulty
            }).ToList();

            var optimization = new TripOptimizationDto
            {
                TripId = trip.TripId,
                TripName = trip.TripName ?? "Unknown Trip",
                Suggestions = mappedSuggestions,
                CurrentIssue = (trip.BookingCount < 5 ? "Ít lượt đặt" : "") + (trip.CancellationRate > 20 ? ", Tỷ lệ hủy cao" : ""),
                Priority = trip.CancellationRate > 20 ? 5 : 4,
                PotentialRevenueIncrease = mappedSuggestions.Sum(s => s.ExpectedImpact) * trip.TotalRevenue / 100
            };
            suggestions.Add(optimization);
        }

        return suggestions.OrderByDescending(s => s.Priority).ThenByDescending(s => s.PotentialRevenueIncrease).ToList();
    }

    // Tổng hợp dashboard overview
    public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
    {
        var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var previousMonth = currentMonth.AddMonths(-1);
        var nextMonth = currentMonth.AddMonths(1);

        var currentMonthBookings = await _context.Bookings
            .Where(b => b.CreatedAt >= currentMonth && b.CreatedAt < nextMonth && b.IsActive)
            .ToListAsync();

        var previousMonthBookings = await _context.Bookings
            .Where(b => b.CreatedAt >= previousMonth && b.CreatedAt < currentMonth && b.IsActive)
            .ToListAsync();

        var activeTrips = await _context.Trips.CountAsync(t => t.IsActive == true);
        var totalDestinations = await _context.Destinations.CountAsync(d => d.IsActive == true);

        var currentMonthRevenue = currentMonthBookings.Sum(b => b.TotalCost);
        var previousMonthRevenue = previousMonthBookings.Sum(b => b.TotalCost);

        return new DashboardOverviewDto
        {
            TotalBookingsThisMonth = currentMonthBookings.Count,
            TotalBookingsPreviousMonth = previousMonthBookings.Count,
            TotalRevenueThisMonth = currentMonthRevenue,
            TotalRevenuePreviousMonth = previousMonthRevenue,
            BookingGrowthRate = previousMonthBookings.Count > 0
                ? ((decimal)(currentMonthBookings.Count - previousMonthBookings.Count) / previousMonthBookings.Count) *
                  100
                : 0,
            RevenueGrowthRate = previousMonthRevenue > 0
                ? ((currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue) * 100
                : 0,
            ActiveTrips = activeTrips,
            TotalDestinations = totalDestinations,
            AverageBookingValue = currentMonthBookings.Count > 0 ? currentMonthRevenue / currentMonthBookings.Count : 0
        };
    }

    // Tổng hợp tất cả dữ liệu cho dashboard
    public async Task<AdminDashboardDto> GetCompleteDashboardAsync()
    {
        var overview = await GetDashboardOverviewAsync();
        var regionForecasts = await GetRegionForecastAsync();
        var (hotTrips, decliningTrips) = await GetTripPopularityAnalysisAsync();
        var optimizationSuggestions = await GetTripOptimizationSuggestionsAsync();

        return new AdminDashboardDto
        {
            Overview = overview,
            RegionForecasts = regionForecasts,
            HotTrips = hotTrips,
            DecliningTrips = decliningTrips,
            OptimizationSuggestions = optimizationSuggestions
        };
    }
}