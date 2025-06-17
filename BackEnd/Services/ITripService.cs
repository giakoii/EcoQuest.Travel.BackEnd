using BackEnd.DTOs.Chatbot;
using BackEnd.DTOs.Ecq110;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface ITripService
{
    Task<Ecq110SelectTripResponse> SelectTrip(Guid tripId);
    
    Task<Ecq110SelectTripsResponse> SelectTrips();
    
    Task<Ecq110InsertTripResponse> InsertTrip(Ecq110InsertTripRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110UpdateTripResponse> UpdateTrip(Ecq110UpdateTripRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110DeleteTripResponse> DeleteTrip(Ecq110DeleteTripRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110InsertTripScheduleResponse> InsertTripSchedule(Ecq110InsertTripScheduleRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110InsertTripScheduleWithAiResponse> InsertTripScheduleUseAi(Ecq110InsertTripScheduleWithAiRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110SelectTripScheduleResponse> SelectTripSchedule(Guid tripId);
    
    Task<Ecq110SelectTripSchedulesResponse> SelectTripSchedules(Guid userId);
    
    Task<Ecq110UpdateTripScheduleResponse> UpdateTripSchedule(Ecq110UpdateTripScheduleRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110DeleteTripScheduleResponse> DeleteTripSchedule(Ecq110DeleteTripScheduleRequest request, IdentityEntity identityEntity);
}
