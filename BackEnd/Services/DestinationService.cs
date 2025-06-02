using BackEnd.DTOs.Ecq200;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class DestinationService : IDestinationService
{
    private readonly IBaseRepository<Destination, Guid> _destinationRepository;
    private readonly IBaseRepository<DestinationImage, Guid> _destinationImageRepository;
    private readonly CloudinaryLogic _cloudinary;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="destinationRepository"></param>
    /// <param name="destinationImageRepository"></param>
    /// <param name="cloudinary"></param>
    public DestinationService(IBaseRepository<Destination, Guid> destinationRepository,
        IBaseRepository<DestinationImage, Guid> destinationImageRepository, CloudinaryLogic cloudinary)
    {
        _destinationRepository = destinationRepository;
        _destinationImageRepository = destinationImageRepository;
        _cloudinary = cloudinary;
    }

    /// <summary>
    /// Insert a new destination
    /// </summary>
    /// <param name="request"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<Ecq200InsertDestinationResponse> InsertDestination(Ecq200InsertDestinationRequest request)
    {
        var response = new Ecq200InsertDestinationResponse { Success = false };
        
        // Check if the destination name already exists
        var existingDestination = await _destinationRepository.Find(d => d.Name.Contains(request.Name)).FirstOrDefaultAsync();
        if (existingDestination != null)
        {
            if (existingDestination.AddressLine == request.AddressLine &&
                existingDestination.Ward == request.Ward && 
                existingDestination.District == request.District && 
                existingDestination.Province == request.Province)
            {
                response.SetMessage(MessageId.E00000, CommonMessages.DestinationAlreadyExists);
                return response;
            }
        }
            
        //Begin transaction
        await _destinationRepository.ExecuteInTransactionAsync(async () =>
        {
            var newDestination = new Destination
            {
                Name = request.Name,
                Description = request.Description,
                Ward = request.Ward,
                District = request.District,
                Province = request.Province,
                AddressLine = request.AddressLine,
            };
            await _destinationRepository.AddAsync(newDestination);
            await _destinationRepository.SaveChangesAsync("User");

            // Insert images
            if (request.DestinationImages != null && request.DestinationImages.Count > 0)
            {
                foreach (var image in request.DestinationImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var destinationImage = new DestinationImage
                    {
                        DestinationId = newDestination.DestinationId,
                        ImageUrl = imageUrl
                    };
                    await _destinationImageRepository.AddAsync(destinationImage);
                }
            }
            await _destinationImageRepository.SaveChangesAsync("User");
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}