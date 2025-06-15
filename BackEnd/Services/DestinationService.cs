using BackEnd.DTOs.Ecq200;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class DestinationService : IDestinationService
{
    private readonly IBaseRepository<Destination, Guid> _destinationRepository;
    private readonly IBaseRepository<Image, Guid> _imageRepository;
    private readonly CloudinaryLogic _cloudinary;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="destinationRepository"></param>
    /// <param name="imageRepository"></param>
    /// <param name="cloudinary"></param>
    public DestinationService(IBaseRepository<Destination, Guid> destinationRepository,
        IBaseRepository<Image, Guid> imageRepository, CloudinaryLogic cloudinary)
    {
        _destinationRepository = destinationRepository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
    }

    /// <summary>
    /// Insert a new destination
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Ecq200InsertDestinationResponse> InsertDestination(Ecq200InsertDestinationRequest request)
    {
        var response = new Ecq200InsertDestinationResponse { Success = false };

        // Check if the destination name already exists
        var existingDestination =
            await _destinationRepository.Find(d => d.Name.Contains(request.Name)).FirstOrDefaultAsync();
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
                    var destinationImage = new Image
                    {
                        EntityId = newDestination.DestinationId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityImage.Destination.ToString(),
                    };
                    await _imageRepository.AddAsync(destinationImage);
                }
            }

            await _imageRepository.SaveChangesAsync("User");

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    /// <summary>
    /// Select a specific destination by ID
    /// </summary>
    /// <param name="destinationId">ID of the destination</param>
    /// <returns>Destination details</returns>
    public async Task<Ecq200SelectDestinationResponse> SelectDestination(Guid destinationId)
    {
        var response = new Ecq200SelectDestinationResponse { Success = false };

        // Get destination by ID
        var destination = await _destinationRepository.GetView<VwDestination>(d => d.DestinationId == destinationId)
            .Select(d => new Ecq200DestinationDetailEntity
            {
                DestinationId = d.DestinationId,
                Name = d.Name,
                Description = d.Description,
                AddressLine = d.AddressLine,
                Ward = d.Ward,
                District = d.District,
                Province = d.Province,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(d.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(d.UpdatedAt)
            })
            .FirstOrDefaultAsync();

        if (destination == null)
        {
            response.SetMessage(MessageId.I00000, "Destination not found");
            return response;
        }

        // Fetch destination images
        destination.DestinationImages = (await _imageRepository
            .GetView<VwImage>(img =>
                img.EntityId == destination.DestinationId &&
                img.EntityType == ConstantEnum.EntityImage.Destination.ToString())
            .Select(img => img.ImageUrl)
            .ToListAsync())!;

        // Set response
        response.Response = destination;
        response.Success = true;
        response.SetMessage(MessageId.I00001);

        return response;
    }

    /// <summary>
    /// Select all destinations
    /// </summary>
    /// <returns>List of all destinations</returns>
    public async Task<Ecq200SelectDestinationsResponse> SelectDestinations()
    {
        var response = new Ecq200SelectDestinationsResponse { Success = false };

        // Select destinations
        var destinations = await _destinationRepository.GetView<VwDestination>()
            .Select(d => new Ecq200DestinationEntity
            {
                DestinationId = d.DestinationId,
                Name = d.Name,
                Description = d.Description,
                AddressLine = d.AddressLine,
                Ward = d.Ward,
                District = d.District,
                Province = d.Province,
                CreatedAt = StringUtil.ConvertToDateAsDdMmYyyy(d.CreatedAt),
                UpdatedAt = StringUtil.ConvertToDateAsDdMmYyyy(d.UpdatedAt)
            })
            .ToListAsync();

        // For each destination, fetch images and related entity counts
        foreach (var destination in destinations)
        {
            // Fetch destination images
            destination.DestinationImages = (await _imageRepository
                .GetView<VwImage>(img =>
                    img.EntityId == destination.DestinationId &&
                    img.EntityType == ConstantEnum.EntityImage.Destination.ToString())
                .Select(img => img.ImageUrl)
                .ToListAsync())!;
        }

        // Set response
        response.Response = destinations;
        response.Success = true;
        response.SetMessage(MessageId.I00001);

        return response;
    }

    /// <summary>
    /// Update an existing destination
    /// </summary>
    /// <param name="request">Update request</param>
    /// <returns>Update result</returns>
    public async Task<Ecq200UpdateDestinationResponse> UpdateDestination(Ecq200UpdateDestinationRequest request)
    {
        var response = new Ecq200UpdateDestinationResponse { Success = false };

        // Find the destination by ID
        var destination = await _destinationRepository
            .Find(d => d.DestinationId == request.DestinationId && d.IsActive == true).FirstOrDefaultAsync();
        if (destination == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.DestinationNotFound);
            return response;
        }

        // Check for duplicate destination (same name and address but different ID)
        var duplicateDestination = await _destinationRepository.Find(d =>
                d.Name == request.Name &&
                d.AddressLine == request.AddressLine &&
                d.Ward == request.Ward &&
                d.District == request.District &&
                d.Province == request.Province &&
                d.DestinationId != request.DestinationId &&
                d.IsActive == true)
            .FirstOrDefaultAsync();
        if (duplicateDestination != null)
        {
            response.SetMessage(MessageId.E00000, CommonMessages.DestinationAlreadyExists);
            return response;
        }

        // Begin transaction
        await _destinationRepository.ExecuteInTransactionAsync(async () =>
        {
            // Update destination properties
            destination.Name = request.Name;
            destination.Description = request.Description;
            destination.AddressLine = request.AddressLine;
            destination.Ward = request.Ward;
            destination.District = request.District;
            destination.Province = request.Province;
            destination.UpdatedBy = "User";

            await _destinationRepository.UpdateAsync(destination);
            await _destinationRepository.SaveChangesAsync("User");

            // Process images to remove
            if (request.ImagesToRemove != null && request.ImagesToRemove.Count > 0)
            {
                foreach (var imageUrl in request.ImagesToRemove)
                {
                    var imageToDelete = await _imageRepository.Find(img =>
                            img.EntityId == request.DestinationId &&
                            img.EntityType == ConstantEnum.EntityImage.Destination.ToString() &&
                            img.ImageUrl == imageUrl)
                        .FirstOrDefaultAsync();

                    if (imageToDelete != null)
                    {
                        await _imageRepository.UpdateAsync(imageToDelete);
                    }
                }
                await _imageRepository.SaveChangesAsync("user", true);
            }

            // Process new images to add
            if (request.NewDestinationImages != null && request.NewDestinationImages.Count > 0)
            {
                foreach (var image in request.NewDestinationImages)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(image);
                    var destinationImage = new Image
                    {
                        EntityId = destination.DestinationId,
                        ImageUrl = imageUrl,
                        EntityType = ConstantEnum.EntityImage.Destination.ToString(),
                    };
                    await _imageRepository.AddAsync(destinationImage);
                }

                await _imageRepository.SaveChangesAsync("User");
            }

            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });

        return response;
    }
}