using BackEnd.DTOs.Ecq110;
using BackEnd.DTOs.User;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class PaymentService : IPaymentService
{
    private readonly IBaseRepository<Payment, Guid> _paymentRepository;
    private readonly IBaseRepository<Booking, Guid> _bookingRepository;
    private readonly IBaseRepository<Trip, Guid> _tripRepository;
    private readonly IBaseRepository<User, Guid> _userRepository;
    private readonly PayOsPaymentLogic _payOsPaymentLogic;

    public PaymentService(IBaseRepository<Payment, Guid> paymentRepository, IBaseRepository<Booking, Guid> bookingRepository, PayOsPaymentLogic payOsPaymentLogic, IBaseRepository<Trip, Guid> tripRepository, IBaseRepository<User, Guid> userRepository)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
        _payOsPaymentLogic = payOsPaymentLogic;
        _tripRepository = tripRepository;
        _userRepository = userRepository;
    }

    public async Task<Ecq110InsertPaymentResponse> InsertPayment(Ecq110InsertPaymentRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110InsertPaymentResponse {Success = false};
        
        // Validate booking and owner booking
        var tripExist = await _tripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true, true, x => x.Bookings).FirstOrDefaultAsync();
        if (tripExist == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.BookingNotFound);
            return response;
        }
        if (tripExist.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageTrip);
            return response;
        }
        
        
        // Check for duplicate payment
        var existingPayment = await _paymentRepository.Find(x => x.TripId == request.TripId && x.Status != nameof(ConstantEnum.BookingStatus.Cancelled)).FirstOrDefaultAsync();
        if (existingPayment != null)
        {
            response.SetMessage(MessageId.I00000, "Payment already exists for this trip.");
            return response;
        }

        // Begin transaction
        await _bookingRepository.ExecuteInTransactionAsync(async () =>
        {
            var totalCost = tripExist.Bookings.Sum(b => b.TotalCost);
            
            var payment = new Payment
            {
                TripId = request.TripId,
                Amount = totalCost,
                Status = nameof(ConstantEnum.BookingStatus.Pending),
                Method = nameof(ConstantEnum.PaymentMethod.PayOs),
                PaidAt = DateTime.Now,
                TransactionCode = "200",
            };
            await _paymentRepository.AddAsync(payment);
            
            var payBookingResult = await _payOsPaymentLogic.PayBooking(request.TripId, tripExist.TripName!);

            await _paymentRepository.SaveChangesAsync(identityEntity.Email);
            
            response.Success = true;
            response.Response = payBookingResult.Response;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    /// <summary>
    /// Payment Premier Account
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq300PaymentPremierAccountResponse> PaymentPremierAccount(Ecq300PaymentPremierAccountRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq300PaymentPremierAccountResponse { Success = false };
        
        var userId = Guid.Parse(identityEntity.UserId);
        var user = await _userRepository.Find(x => x.UserId == userId && x.IsActive == true).FirstOrDefaultAsync();
        if (user == null)
        {
            response.SetMessage(MessageId.I00000, "User not found or not available.");
            return response;
        }
        
        if (user.UserType == (byte) ConstantEnum.UserType.Premier)
        {
            response.SetMessage(MessageId.I00000, "You are already a premier user.");
            return response;
        }

        var payment = await _payOsPaymentLogic.PaymentPremierAccount(userId);
        if (!payment.Success)
        {
            response.SetMessage(MessageId.E00000, "Failed to create payment request.");
            return response;
        }
        
        // True
        response.Success = true;
        response.Response = payment.Response;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Select Payment Booking of a user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq300SelectPaymentBookingsResponse> SelectPaymentBooking(Ecq300SelectPaymentBookingsRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq300SelectPaymentBookingsResponse { Success = false, };
        // Select Payment
        response.Response = await _paymentRepository.Find(x => x.IsActive
                                                               && x.Trip.UserId == Guid.Parse((identityEntity.UserId)),
                false, x => x.Trip)
            .Select(x => new Ecq300SelectPaymentBookingsEntity
            {
                PaymentId = x.PaymentId!,
                Amount = x.Amount,
                Method = x.Method,
                PaidAt = x.PaidAt,
                Status = x.Status,
                TripId = x.TripId,
            })
            .ToListAsync();

        // True
       response.Success = true;
       response.SetMessage(MessageId.I00001);
       return response;
    }

    /// <summary>
    /// Select Payment Booking Detail by TripId
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq300SelectPaymentBookingResponse> SelectPaymentBookingDetail(Ecq300SelectPaymentBookingRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq300SelectPaymentBookingResponse
        {
            Success = false
        };

        // Get payment booking detail from VwPaymentBookingTrip
        var paymentBookingDetail = await _paymentRepository
            .GetView<VwPaymentBookingTrip>()
            .Where(x => x.PaymentId == request.PaymentId && x.UserId == Guid.Parse(identityEntity.UserId))
            .FirstOrDefaultAsync();

        // Check if payment booking exists
        if (paymentBookingDetail == null)
        {
            response.SetMessage(MessageId.I00000, "Payment booking not found or you are not authorized to view this booking.");
            return response;
        }

        // Map to response entity
        response.Response = new Ecq300SelectPaymentBookingEntity
        {
            PaymentId = paymentBookingDetail.PaymentId,
            TripId = paymentBookingDetail.TripId,
            TripName = paymentBookingDetail.TripName,
            UserId = paymentBookingDetail.UserId,
            FirstName = paymentBookingDetail.FirstName,
            LastName = paymentBookingDetail.LastName,
            UserEmail = paymentBookingDetail.UserEmail,
            Amount = paymentBookingDetail.Amount,
            Method = paymentBookingDetail.Method,
            Status = paymentBookingDetail.Status,
            TransactionCode = paymentBookingDetail.TransactionCode,
            PaidAt = paymentBookingDetail.PaidAt,
            PaymentCreatedAt = paymentBookingDetail.PaymentCreatedAt,
            StartDate = paymentBookingDetail.StartDate,
            EndDate = paymentBookingDetail.EndDate,
            NumberOfPeople = paymentBookingDetail.NumberOfPeople,
            TotalEstimatedCost = paymentBookingDetail.TotalEstimatedCost,
            StartingPointAddress = paymentBookingDetail.StartingPointAddress,
            TripDescription = paymentBookingDetail.TripDescription,
            TripStatus = paymentBookingDetail.TripStatus
        };

        // Success
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return response;
    }

    /// <summary>
    /// Payment Callback from PayOs
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq110PaymentCallbackResponse> PaymentCallback(Ecq110PaymentCallbackRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110PaymentCallbackResponse { Success = false };
        
        // Get payment record
        var payment = await _paymentRepository
            .Find(x => x.TripId == request.TripId && x.IsActive && x.Status == nameof(ConstantEnum.PaymentStatus.Pending)
                , isTracking: true
                , includes: x => x.Trip)
            .FirstOrDefaultAsync();        
        if (payment == null)
        {
            response.SetMessage(MessageId.I00000, "Payment not found or not available.");
            return response;
        }

        if (payment.Trip.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, "You are not authorized to manage this payment.");
            return response;
        }
        
        // Begin transaction
        await _paymentRepository.ExecuteInTransactionAsync(async () =>
        {
            // Payment failed
            if(request.Code != "00")
            {
                var paymentResponse = await _payOsPaymentLogic.PayBooking(request.TripId, payment.Trip.TripName!);
                if (!paymentResponse.Success)
                {
                    response.SetMessage(MessageId.E00000, "Payment failed.");
                    return false;
                }

                response.Response = new Ecq110PaymentCallbackEntity
                {
                    CheckoutUrl = paymentResponse.Response.CheckoutUrl,
                    QrCode = paymentResponse.Response.QrCode,
                };
                
                // Set response
                response.Success = false;
                response.SetMessage(MessageId.I00000, "Payment returned successfully.");
                return false;
            }
            
            // If cancel is true, create new payment record
            if (request.Cancel)
            {
                payment.Status = nameof(ConstantEnum.PaymentStatus.Cancelled);
            }
            else
            {
                payment.Status = nameof(ConstantEnum.PaymentStatus.Completed);
            }
            
            // Save changes
            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync(identityEntity.Email);
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    public async Task<Ecq110PaymentCallbackResponse> PaymentPremierAccountCallBack(Ecq110PaymentCallbackRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110PaymentCallbackResponse { Success = false };
        
        // Get payment record
        var user = await _userRepository
            .Find(x => x.UserId == request.TripId && x.IsActive == true, isTracking: true)
            .FirstOrDefaultAsync();        
        if (user == null)
        {
            response.SetMessage(MessageId.I00000, "User not found or not available.");
            return response;
        }
        
        // Begin transaction
        await _paymentRepository.ExecuteInTransactionAsync(async () =>
        {
            // Payment failed
            if(request.Code != "00")
            {
                var paymentResponse = await _payOsPaymentLogic.PaymentPremierAccount(request.TripId);
                if (!paymentResponse.Success)
                {
                    response.SetMessage(MessageId.E00000, "Payment failed.");
                    return false;
                }

                response.Response = new Ecq110PaymentCallbackEntity
                {
                    CheckoutUrl = paymentResponse.Response.CheckoutUrl,
                    QrCode = paymentResponse.Response.QrCode,
                };
                
                // Set response
                response.Success = false;
                response.SetMessage(MessageId.I00000, "Payment returned successfully.");
                return false;
            }
            
            user.UserType = (byte) ConstantEnum.UserType.Premier;
            
            // Save changes
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync(identityEntity.Email);
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}