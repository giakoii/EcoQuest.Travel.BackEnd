using BackEnd.DTOs.Ecq110;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface ITripScheduleService
{
       
    Task<Ecq110InsertTripScheduleResponse> InsertTripSchedule(Ecq110InsertTripScheduleRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110InsertTripScheduleWithAiResponse> InsertTripScheduleUseAi(Ecq110InsertTripScheduleWithAiRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110SelectTripScheduleResponse> SelectTripSchedule(Guid tripId);
    
    Task<Ecq110SelectTripSchedulesResponse> SelectTripSchedules(Guid userId);
    
    Task<Ecq110UpdateTripScheduleResponse> UpdateTripSchedule(Ecq110UpdateTripScheduleRequest request, IdentityEntity identityEntity);
    
    Task<Ecq110DeleteTripScheduleResponse> DeleteTripSchedule(Ecq110DeleteTripScheduleRequest request, IdentityEntity identityEntity);

    Task<Ecq110SelectServiceResponse> SelectService(Ecq110SelectServiceRequest request);
}