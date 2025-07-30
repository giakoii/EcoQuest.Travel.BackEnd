using BackEnd.DTOs.Dashboard;
using BackEnd.Models;
using BackEnd.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Logics;

public class DashboardAnalyticsLogic
{
    private readonly AppDbContext _context;

    public DashboardAnalyticsLogic(AppDbContext context)
    {
        _context = context;
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
    public async Task<(List<TripPopularityDto> hotTrips, List<TripPopularityDto> decliningTrips)>
        GetTripPopularityAnalysisAsync()
    {
        var threeMonthsAgo = DateTime.Now.AddMonths(-3);

        var tripData = await _context.Bookings
            .Include(b => b.Trip)
            .ThenInclude(t => t.TripDestinations)
            .ThenInclude(td => td.Destination)
            .Where(b => b.CreatedAt >= threeMonthsAgo)
            .GroupBy(b => new { b.TripId, b.Trip.TripName })
            .Select(g => new
            {
                g.Key.TripId,
                g.Key.TripName,
                Destination = g.FirstOrDefault()!.Trip.TripDestinations.FirstOrDefault()!.Destination.Name,
                BookingCount = g.Count(),
                Revenue = g.Sum(b => b.TotalCost),
                MonthlyData = g.GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                    .Select(mg => new MonthlyTripData
                    {
                        Month = new DateTime(mg.Key.Year, mg.Key.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                        BookingCount = mg.Count(),
                        Revenue = mg.Sum(b => b.TotalCost)
                    }).ToList()
            })
            .ToListAsync();

        // Lấy rating trung bình từ các bảng rating
        var tripPopularityList = new List<TripPopularityDto>();

        foreach (var trip in tripData)
        {
            var tripPopularity = new TripPopularityDto
            {
                TripId = trip.TripId,
                TripName = trip.TripName ?? "Unknown Trip",
                Destination = trip.Destination ?? "Unknown Destination",
                BookingCount = trip.BookingCount,
                Revenue = trip.Revenue,
                TrendData = trip.MonthlyData.OrderBy(m => m.Month).ToList()
            };

            // Tính growth rate dựa trên 2 tháng gần nhất
            var recentMonths = tripPopularity.TrendData.TakeLast(2).ToList();
            if (recentMonths.Count == 2)
            {
                var previousMonthBookings = recentMonths[0].BookingCount;
                var currentMonthBookings = recentMonths[1].BookingCount;

                if (previousMonthBookings > 0)
                {
                    tripPopularity.GrowthRate =
                        ((decimal)(currentMonthBookings - previousMonthBookings) / previousMonthBookings) * 100;
                }
            }

            // Xác định trend status
            if (tripPopularity.GrowthRate > 20)
                tripPopularity.TrendStatus = "Hot";
            else if (tripPopularity.GrowthRate < -20)
                tripPopularity.TrendStatus = "Declining";
            else
                tripPopularity.TrendStatus = "Stable";

            tripPopularityList.Add(tripPopularity);
        }

        var hotTrips = tripPopularityList
            .Where(t => t.TrendStatus == "Hot" || t.BookingCount >= 10)
            .OrderByDescending(t => t.GrowthRate)
            .ThenByDescending(t => t.BookingCount)
            .Take(10)
            .ToList();

        var decliningTrips = tripPopularityList
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
            .Where(t => t.BookingCount < 5 || t.CancellationRate > 20) // Trips with low bookings or high cancellation
            .ToListAsync();

        foreach (var trip in problemTrips)
        {
            var optimization = new TripOptimizationDto
            {
                TripId = trip.TripId,
                TripName = trip.TripName ?? "Unknown Trip",
                Suggestions = new List<OptimizationSuggestion>()
            };

            // Phân tích vấn đề và đưa ra gợi ý
            if (trip.BookingCount < 5)
            {
                optimization.CurrentIssue = "Low booking volume";
                optimization.Priority = 4;

                // Gợi ý marketing
                optimization.Suggestions.Add(new OptimizationSuggestion
                {
                    Type = "Marketing",
                    Title = "Increase marketing efforts",
                    Description = "Consider running targeted ads or social media campaigns for this destination",
                    ExpectedImpact = 30,
                    Difficulty = "Medium"
                });

                // Gợi ý giá
                if (trip.EstimatedCost.HasValue && trip.AverageActualCost > trip.EstimatedCost * 1.2m)
                {
                    optimization.Suggestions.Add(new OptimizationSuggestion
                    {
                        Type = "Price",
                        Title = "Adjust pricing strategy",
                        Description = "Consider offering early bird discounts or package deals",
                        ExpectedImpact = 25,
                        Difficulty = "Easy"
                    });
                }
            }

            if (trip.CancellationRate > 20)
            {
                optimization.CurrentIssue = "High cancellation rate";
                optimization.Priority = 5;

                optimization.Suggestions.Add(new OptimizationSuggestion
                {
                    Type = "Service",
                    Title = "Improve service quality",
                    Description = "Review and enhance tour services based on customer feedback",
                    ExpectedImpact = 40,
                    Difficulty = "Hard"
                });

                optimization.Suggestions.Add(new OptimizationSuggestion
                {
                    Type = "Schedule",
                    Title = "Optimize tour schedule",
                    Description = "Adjust timing and duration based on customer preferences",
                    ExpectedImpact = 20,
                    Difficulty = "Medium"
                });
            }

            // Tính toán potential revenue increase
            optimization.PotentialRevenueIncrease =
                optimization.Suggestions.Sum(s => s.ExpectedImpact) * trip.TotalRevenue / 100;

            suggestions.Add(optimization);
        }

        return suggestions.OrderByDescending(s => s.Priority).ThenByDescending(s => s.PotentialRevenueIncrease)
            .ToList();
    }

    // Tổng hợp dashboard overview
    public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
    {
        var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var previousMonth = currentMonth.AddMonths(-1);
        var nextMonth = currentMonth.AddMonths(1);

        var currentMonthBookings = await _context.Bookings
            .Where(b => b.CreatedAt >= currentMonth && b.CreatedAt < nextMonth)
            .ToListAsync();

        var previousMonthBookings = await _context.Bookings
            .Where(b => b.CreatedAt >= previousMonth && b.CreatedAt < currentMonth)
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