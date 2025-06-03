using BackEnd.DTOs.Ecq310;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IPartnerService
{
    Task<Ecq310InsertPartnerResponse> InsertPartner(Ecq310InsertPartnerRequest request, IdentityEntity identityEntity);
    
    Task<Ecq310SelectPartnersResponse> SelectPartners(Ecq310SelectPartnersRequest request);
    
    Task<Ecq310SelectPartnerResponse> SelectPartner(Guid partnerId);
    
    Task<Ecq310UpdatePartnerResponse> UpdatePartner(Ecq310UpdatePartnerRequest request, IdentityEntity identityEntity);
}