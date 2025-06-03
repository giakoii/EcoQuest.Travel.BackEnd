using BackEnd.DTOs.Ecq210;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;

namespace BackEnd.Services;

public class HotelService : IHotelService
{
    private readonly IBaseRepository<Hotel, Guid> _hotelRepository;
    private readonly IPartnerService _partnerService;

    public HotelService(IBaseRepository<Hotel, Guid> hotelRepository, IPartnerService partnerService)
    {
        _hotelRepository = hotelRepository;
        _partnerService = partnerService;
    }

    /// <summary>
    /// Insert new hotel
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq210InsertHotelResponse> InsertHotel(Ecq210InsertHotelRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq210InsertHotelResponse {Success = false};
        
        // Validate request
        var partnerTypeValid = await _partnerService.SelectPartner(Guid.Parse(identityEntity.UserId));
        if (!partnerTypeValid.Response.PartnerType.Contains((byte) ConstantEnum.PartnerType.Hotel))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.PartnerTypeInvalid);
            return response;
        }
        
        // Begin transaction
        await _hotelRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new hotel
            var newHotel = new Hotel
            {
                Name = request.Name,
                Address = request.Address,
                Description = request.Description,
                OwnerId = Guid.Parse(identityEntity.UserId),
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DestinationId = request.DestinationId,
            };
            await _hotelRepository.AddAsync(newHotel);
            await _hotelRepository.SaveChangesAsync(identityEntity.Email);

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}