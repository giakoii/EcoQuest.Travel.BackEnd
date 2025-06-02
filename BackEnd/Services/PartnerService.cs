using BackEnd.Controllers;
using BackEnd.Controllers.V1.Ecq310;
using BackEnd.DTOs.Ecq310;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class PartnerService : IPartnerService
{
    private readonly IBaseRepository<Partner, Guid> _partnerRepository;
    private readonly IBaseRepository<Account, Guid> _accountRepository;
    private readonly IBaseRepository<PartnerType, Guid> _partnerTypeRepository;
    private readonly IBaseRepository<Role, Guid> _roleRepository;
    private readonly IBaseRepository<PartnerPartnerType, Guid> _partnerPartnerTypeRepository;
    private readonly IEmailTemplateRepository _emailTemplateRepository;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="partnerRepository"></param>
    /// <param name="accountRepository"></param>
    /// <param name="roleRepository"></param>
    /// <param name="partnerTypeRepository"></param>
    /// <param name="partnerPartnerTypeRepository"></param>
    /// <param name="emailTemplateRepository"></param>
    public PartnerService(IBaseRepository<Partner, Guid> partnerRepository,
        IBaseRepository<Account, Guid> accountRepository, IBaseRepository<Role, Guid> roleRepository,
        IBaseRepository<PartnerType, Guid> partnerTypeRepository,
        IBaseRepository<PartnerPartnerType, Guid> partnerPartnerTypeRepository,
        IEmailTemplateRepository emailTemplateRepository)
    {
        _partnerRepository = partnerRepository;
        _accountRepository = accountRepository;
        _roleRepository = roleRepository;
        _partnerTypeRepository = partnerTypeRepository;
        _partnerPartnerTypeRepository = partnerPartnerTypeRepository;
        _emailTemplateRepository = emailTemplateRepository;
    }

    /// <summary>
    /// Insert new partner
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq310InsertPartnerResponse> InsertPartner(Ecq310InsertPartnerRequest request,
        IdentityEntity identityEntity)
    {
        var response = new Ecq310InsertPartnerResponse { Success = false };

        // Check if account already exists
        var existingPartner = await _accountRepository.Find(x => x.Email == request.Email).FirstOrDefaultAsync();
        if (existingPartner != null)
        {
            response.SetMessage(MessageId.E11004);
            return response;
        }

        // Begin transaction
        await _partnerRepository.ExecuteInTransactionAsync(async () =>
        {
            var role = await _roleRepository.Find(x => x.Name == ConstantEnum.UserRole.Partner.ToString())
                .FirstOrDefaultAsync();

            var passwordGenerated = CommonLogic.GenerateRandomPassword();

            // Insert new account
            var newAccount = new Account
            {
                AccountId = new Guid(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordGenerated, workFactor: 12),
                LockoutEnd = null,
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                RoleId = role!.Id,
                PhoneNumber = request.PhoneNumber,
            };
            await _accountRepository.AddAsync(newAccount);
            await _accountRepository.SaveChangesAsync(identityEntity.Email);

            // Insert new partner
            var newPartner = new Partner
            {
                PartnerId = newAccount.AccountId,
                AccountId = newAccount.AccountId,
                Description = request.Description,
                Verified = true,
                CreatedAt = DateTime.UtcNow,
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
            };

            await _partnerRepository.AddAsync(newPartner);
            await _partnerRepository.SaveChangesAsync(identityEntity.Email);

            foreach (var typeId in request.PartnerType)
            {
                var enumValue = (ConstantEnum.PartnerType)typeId;

                var enumName = Enum.GetName(typeof(ConstantEnum.PartnerType), enumValue);

                var partnerTypeEntity =
                    await _partnerTypeRepository.Find(x => x.TypeName == enumName).FirstOrDefaultAsync();

                var partnerPartnerType = new PartnerPartnerType
                {
                    PartnerId = newPartner.PartnerId,
                    TypeId = partnerTypeEntity!.TypeId,
                };
                await _partnerPartnerTypeRepository.AddAsync(partnerPartnerType);
            }

            await _partnerPartnerTypeRepository.SaveChangesAsync(identityEntity.Email);

            var detailErrorList = new List<DetailError>();
            await Ecq310InsertPartnerSendmail.SendMailAccountInformation(_emailTemplateRepository, newAccount.Email!,
                passwordGenerated, detailErrorList);

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    /// <summary>
    /// Select partner list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Ecq310SelectPartnersResponse> SelectPartners(Ecq310SelectPartnersRequest request)
    {
        var response = new Ecq310SelectPartnersResponse { Success = false };

        // Select partners
        var partnersSelect = await _partnerRepository.GetView<VwEcq310SelectPartner>()
            .Select(x => new Ecq310SelectPartnersEntity
            {
                PartnerId = x.PartnerId,
                Description = x.Description,
                Verified = x.Verified,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                CompanyName = x.CompanyName,
                ContactName = x.ContactName,
            })
            .ToListAsync();

        foreach (var partner in partnersSelect)
        {
            // Get partner types
            var partnerTypeNames = await _partnerRepository
                .GetView<VwPartnerPartnerType>()
                .Where(x => x.PartnerId == partner.PartnerId)
                .Select(x => x.TypeName)
                .ToListAsync();

            // Convert partner type names to byte values
            var partnerTypeEnums = partnerTypeNames
                .Select(name => (byte)Enum.Parse<ConstantEnum.PartnerType>(name!))
                .ToList();

            partner.PartnerType = partnerTypeEnums;
        }

        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        response.Response = partnersSelect;
        return response;
    }

    /// <summary>
    /// Select partner by Id
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Ecq310SelectPartnerResponse> SelectPartner(Ecq310SelectPartnerRequest request)
    {
        var response = new Ecq310SelectPartnerResponse { Success = false };
        // Select partners
        var partnerSelect = await _partnerRepository.GetView<VwEcq310SelectPartner>()
            .Select(x => new Ecq310SelectPartnerEntity
            {
                PartnerId = x.PartnerId,
                AccountId = x.AccountId,
                Description = x.Description,
                Verified = x.Verified,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(x.CreatedAt),
                CompanyName = x.CompanyName,
                ContactName = x.ContactName,
            })
            .FirstOrDefaultAsync(x => x.PartnerId == request.PartnerId);
        if (partnerSelect == null)
        {
            response.SetMessage(MessageId.E00000, CommonMessages.PartnerNotFound);
            return response;
        }

        // Get partner types
        var partnerTypeNames = await _partnerRepository
            .GetView<VwPartnerPartnerType>()
            .Where(x => x.PartnerId == partnerSelect.PartnerId)
            .Select(x => x.TypeName)
            .ToListAsync();

        // Convert partner type names to byte values
        partnerSelect.PartnerType = partnerTypeNames
            .Select(name => (byte)Enum.Parse<ConstantEnum.PartnerType>(name!))
            .ToList();
        
        // Select hotels
        partnerSelect.Hotels = await _partnerRepository
            .GetView<VwHotel>()
            .Where(h => h.OwnerId == partnerSelect.PartnerId)
            .Select(h => new Ecq310SelectPartnerEntityHotel
            {
                HotelId = h.HotelId,
                HotelName = h.HotelName,
                HotelDescription = h.HotelDescription,
                Address = h.Address,
                PhoneNumber = h.PhoneNumber,
                Email = h.Email,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(h.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(h.UpdatedAt),
                OwnerId = h.OwnerId,
                DestinationId = h.DestinationId,
                DestinationName = h.DestinationName,
                AddressLine = h.AddressLine,
                Ward = h.Ward,
                City = h.City,
                District = h.District,
                Province = h.Province,
            })
            .ToListAsync();
        
        // Select hotel rooms
        foreach (var hotel in partnerSelect.Hotels)
        {
            hotel.Rooms = await _partnerRepository
                .GetView<VwHotelRoom>()
                .Where(r => r.HotelId == hotel.HotelId)
                .Select(r => new Ecq310SelectPartnerEntityHotelRoom
                {
                    RoomId = r.RoomId,
                    Description = r.Description,
                    HotelId = r.HotelId,
                    IsAvailable = r.IsAvailable,
                    MaxGuests = r.MaxGuests,
                    RoomType = r.RoomType,
                    PricePerNight = r.PricePerNight,
                })
                .ToListAsync();
        }
        
        // Select restaurants
        partnerSelect.Restaurants = await _partnerRepository
            .GetView<VwRestaurant>()
            .Where(r => r.PartnerId == partnerSelect.PartnerId)
            .Select(r => new Ecq310SelectPartnerEntityRestaurant
            {
                PartnerId = r.PartnerId,
                CuisineType = r.CuisineType,
                HasVegetarian = r.HasVegetarian,
                OpenTime = StringUtil.ConvertToDateAsDdMmYyyy(r.OpenTime),
                CloseTime = StringUtil.ConvertToDateAsDdMmYyyy(r.CloseTime),
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(r.UpdatedAt),
                MinPrice = r.MinPrice,
                MaxPrice = r.MaxPrice,
                DestinationId = r.DestinationId,
                DestinationName = r.DestinationName,
                AddressLine = r.AddressLine,
                Province = r.Province,
                District = r.District,
                City = r.City,
                Ward = r.Ward,
            })
            .ToListAsync();

        // Select attractions
        partnerSelect.AttractionDetails = await _partnerRepository
            .GetView<VwAttraction>()
            .Where(a => a.PartnerId == partnerSelect.PartnerId)
            .Select(a => new Ecq310SelectPartnerEntityAttractionDetail
            {
                PartnerId = a.PartnerId,
                AttractionType = a.AttractionType,
                TicketPrice = a.TicketPrice,
                OpenTime = StringUtil.ConvertToDateAsDdMmYyyy(a.OpenTime),
                CloseTime = StringUtil.ConvertToDateAsDdMmYyyy(a.CloseTime),
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(a.UpdatedAt),
                DestinationId = a.DestinationId,
                DestinationName = a.DestinationName,
                AddressLine = a.AddressLine,
                Ward = a.Ward,
                City = a.City,
                District = a.District,
                Province = a.Province
            })
            .ToListAsync();
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        response.Response = partnerSelect;
        return response;
    }
}