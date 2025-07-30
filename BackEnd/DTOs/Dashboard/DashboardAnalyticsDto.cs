using System;
using System.Collections.Generic;

namespace BackEnd.DTOs.Dashboard
{
    // DTO cho dự báo lượt khách theo vùng
    public class RegionForecastDto
    {
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public int CurrentMonthBookings { get; set; }
        public int PreviousMonthBookings { get; set; }
        public decimal GrowthRate { get; set; }
        public int PredictedNextMonthBookings { get; set; }
        public List<MonthlyBookingData> HistoricalData { get; set; } = new();
    }

    public class MonthlyBookingData
    {
        public DateTime Month { get; set; }
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
    }

    // DTO cho tour hot và điểm sụt giảm
    public class TripPopularityDto
    {
        public Guid TripId { get; set; }
        public string TripName { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageRating { get; set; }
        public decimal GrowthRate { get; set; }
        public string TrendStatus { get; set; } = null!; // "Hot", "Declining", "Stable"
        public List<MonthlyTripData> TrendData { get; set; } = new();
    }

    public class MonthlyTripData
    {
        public DateTime Month { get; set; }
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
    }

    // DTO cho gợi ý điều chỉnh tour
    public class TripOptimizationDto
    {
        public Guid TripId { get; set; }
        public string TripName { get; set; } = null!;
        public string CurrentIssue { get; set; } = null!;
        public List<OptimizationSuggestion> Suggestions { get; set; } = new();
        public decimal PotentialRevenueIncrease { get; set; }
        public int Priority { get; set; } // 1-5, với 5 là ưu tiên cao nhất
    }

    public class OptimizationSuggestion
    {
        public string Type { get; set; } = null!; // "Price", "Schedule", "Service", "Marketing"
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal ExpectedImpact { get; set; }
        public string Difficulty { get; set; } = null!; // "Easy", "Medium", "Hard"
    }

    // DTO tổng hợp cho dashboard
    public class AdminDashboardDto
    {
        public DashboardOverviewDto Overview { get; set; } = new();
        public List<RegionForecastDto> RegionForecasts { get; set; } = new();
        public List<TripPopularityDto> HotTrips { get; set; } = new();
        public List<TripPopularityDto> DecliningTrips { get; set; } = new();
        public List<TripOptimizationDto> OptimizationSuggestions { get; set; } = new();
    }

    public class DashboardOverviewDto
    {
        public int TotalBookingsThisMonth { get; set; }
        public int TotalBookingsPreviousMonth { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalRevenuePreviousMonth { get; set; }
        public decimal BookingGrowthRate { get; set; }
        public decimal RevenueGrowthRate { get; set; }
        public int ActiveTrips { get; set; }
        public int TotalDestinations { get; set; }
        public decimal AverageBookingValue { get; set; }
    }
}
