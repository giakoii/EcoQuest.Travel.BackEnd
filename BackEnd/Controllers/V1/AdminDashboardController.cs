using BackEnd.DTOs.Dashboard;
using BackEnd.Logics;
using BackEnd.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers.V1;

[ApiController]
[Route("api/v1/admin/[controller]")]
public class AdminDashboardController : ControllerBase
{
    private readonly DashboardAnalyticsLogic _dashboardLogic;
    private readonly AppDbContext _context;

    public AdminDashboardController(AppDbContext context, DashboardAnalyticsLogic dashboardLogic)
    {
        _context = context;
        _dashboardLogic = dashboardLogic;
    }

    /// <summary>
    /// Lấy tổng quan dashboard cho admin
    /// </summary>
    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewDto>> GetDashboardOverview()
    {
        try
        {
            var overview = await _dashboardLogic.GetDashboardOverviewAsync();
            return Ok(overview);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting dashboard overview: {ex.Message}");
        }
    }

    /// <summary>
    /// Dự báo lượt khách theo vùng
    /// </summary>
    [HttpGet("region-forecast")]
    public async Task<ActionResult<List<RegionForecastDto>>> GetRegionForecast()
    {
        try
        {
            var forecast = await _dashboardLogic.GetRegionForecastAsync();
            return Ok(forecast);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting region forecast: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy danh sách tour hot (phổ biến)
    /// </summary>
    [HttpGet("hot-trips")]
    public async Task<ActionResult<List<TripPopularityDto>>> GetHotTrips()
    {
        try
        {
            var (hotTrips, _) = await _dashboardLogic.GetTripPopularityAnalysisAsync();
            return Ok(hotTrips);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting hot trips: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy danh sách tour đang sụt giảm
    /// </summary>
    [HttpGet("declining-trips")]
    public async Task<ActionResult<List<TripPopularityDto>>> GetDecliningTrips()
    {
        try
        {
            var (_, decliningTrips) = await _dashboardLogic.GetTripPopularityAnalysisAsync();
            return Ok(decliningTrips);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting declining trips: {ex.Message}");
        }
    }

    /// <summary>
    /// Phân tích độ phổ biến của tất cả tour
    /// </summary>
    [HttpGet("trip-popularity-analysis")]
    public async Task<ActionResult<object>> GetTripPopularityAnalysis()
    {
        try
        {
            var (hotTrips, decliningTrips) = await _dashboardLogic.GetTripPopularityAnalysisAsync();
            return Ok(new
            {
                HotTrips = hotTrips,
                DecliningTrips = decliningTrips
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting trip popularity analysis: {ex.Message}");
        }
    }

    /// <summary>
    /// Gợi ý điều chỉnh tour
    /// </summary>
    [HttpGet("optimization-suggestions")]
    public async Task<ActionResult<List<TripOptimizationDto>>> GetOptimizationSuggestions()
    {
        try
        {
            var suggestions = await _dashboardLogic.GetTripOptimizationSuggestionsAsync();
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting optimization suggestions: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy tất cả dữ liệu dashboard (complete dashboard)
    /// </summary>
    [HttpGet("complete")]
    public async Task<ActionResult<AdminDashboardDto>> GetCompleteDashboard()
    {
        try
        {
            var dashboard = await _dashboardLogic.GetCompleteDashboardAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting complete dashboard: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy chi tiết phân tích cho một tour cụ thể
    /// </summary>
    [HttpGet("trip-analysis/{tripId}")]
    public async Task<ActionResult<object>> GetTripDetailAnalysis(Guid tripId)
    {
        try
        {
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);

            var tripBookings = await _context.Bookings
                .Where(b => b.TripId == tripId && b.CreatedAt >= threeMonthsAgo)
                .OrderBy(b => b.CreatedAt)
                .ToListAsync();

            if (!tripBookings.Any())
            {
                return NotFound("No booking data found for this trip in the last 3 months");
            }

            var trip = await _context.Trips
                .Include(t => t.TripDestinations)
                .ThenInclude(td => td.Destination)
                .FirstOrDefaultAsync(t => t.TripId == tripId);

            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            var monthlyData = tripBookings
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    BookingCount = g.Count(),
                    Revenue = g.Sum(b => b.TotalCost),
                    AverageBookingValue = g.Average(b => b.TotalCost),
                    CancellationRate = g.Count(b => b.Status == "Cancelled") * 100.0 / g.Count()
                })
                .OrderBy(m => m.Month)
                .ToList();

            var result = new
            {
                TripInfo = new
                {
                    TripId = trip.TripId,
                    TripName = trip.TripName,
                    Destinations = trip.TripDestinations.Select(td => td.Destination.Name).ToList(),
                    TotalEstimatedCost = trip.TotalEstimatedCost
                },
                Summary = new
                {
                    TotalBookings = tripBookings.Count,
                    TotalRevenue = tripBookings.Sum(b => b.TotalCost),
                    AverageBookingValue = tripBookings.Average(b => b.TotalCost),
                    CancellationRate = tripBookings.Count(b => b.Status == "Cancelled") * 100.0 / tripBookings.Count
                },
                MonthlyTrend = monthlyData
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting trip analysis: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy thống kê theo khoảng thời gian tùy chỉnh
    /// </summary>
    [HttpGet("custom-period")]
    public async Task<ActionResult<object>> GetCustomPeriodAnalysis(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate >= endDate)
            {
                return BadRequest("Start date must be before end date");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                .ThenInclude(t => t.TripDestinations)
                .ThenInclude(td => td.Destination)
                .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                .ToListAsync();

            var dailyStats = bookings
                .GroupBy(b => b.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    BookingCount = g.Count(),
                    Revenue = g.Sum(b => b.TotalCost),
                    UniqueTrips = g.Select(b => b.TripId).Distinct().Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            var topDestinations = bookings
                .SelectMany(b => b.Trip.TripDestinations.Select(td => td.Destination))
                .GroupBy(d => d.Province)
                .Select(g => new
                {
                    Province = g.Key,
                    BookingCount = g.Count(),
                    Revenue = bookings.Where(b =>
                        b.Trip.TripDestinations.Any(td => td.Destination.Province == g.Key)
                    ).Sum(b => b.TotalCost)
                })
                .OrderByDescending(d => d.BookingCount)
                .Take(10)
                .ToList();

            var result = new
            {
                Period = new { StartDate = startDate, EndDate = endDate },
                Summary = new
                {
                    TotalBookings = bookings.Count,
                    TotalRevenue = bookings.Sum(b => b.TotalCost),
                    AverageBookingValue = bookings.Any() ? bookings.Average(b => b.TotalCost) : 0,
                    UniqueTrips = bookings.Select(b => b.TripId).Distinct().Count()
                },
                DailyTrend = dailyStats,
                TopDestinations = topDestinations
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting custom period analysis: {ex.Message}");
        }
    }
}