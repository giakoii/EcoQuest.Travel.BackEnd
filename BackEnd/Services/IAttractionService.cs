using BackEnd.DTOs.Ecq230;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IAttractionService
{
    Task<Ecq230SelectAttractionResponse> SelectAttraction(Guid attractionId);
    
    Task<Ecq230SelectAttractionsResponse> SelectAttractions();
}
