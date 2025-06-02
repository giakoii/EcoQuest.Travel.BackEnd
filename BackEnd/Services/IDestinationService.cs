using BackEnd.DTOs.Ecq200;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IDestinationService
{
    Task<Ecq200InsertDestinationResponse> InsertDestination(Ecq200InsertDestinationRequest request);
}