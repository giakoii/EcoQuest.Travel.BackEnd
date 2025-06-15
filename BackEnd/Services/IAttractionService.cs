using BackEnd.DTOs.Ecq230;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IAttractionService
{
    Task<Ecq230SelectAttractionResponse> Ecq100SelectAttraction(Guid attractionId);
    
    Task<Ecq230SelectAttractionResponse> Ecq230SelectAttraction(Guid attractionId, IdentityEntity identityEntity);
    
    Task<Ecq230SelectAttractionsResponse> Ecq100SelectAttractions();
    
    Task<Ecq230SelectAttractionsResponse> Ecq230SelectAttractions(IdentityEntity identityEntity);
    
    Task<Ecq230InsertAttractionResponse> InsertAttraction(Ecq230InsertAttractionRequest request, IdentityEntity identityEntity);
    
    Task<Ecq230UpdateAttractionResponse> Ecq230UpdateAttraction(Ecq230UpdateAttractionRequest request, IdentityEntity identityEntity);
}
