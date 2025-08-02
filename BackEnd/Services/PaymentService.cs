using BackEnd.DTOs.Ecq110;
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
    private readonly PayOsPaymentLogic _payOsPaymentLogic;

    public PaymentService(IBaseRepository<Payment, Guid> paymentRepository, IBaseRepository<Booking, Guid> bookingRepository, PayOsPaymentLogic payOsPaymentLogic, IBaseRepository<Trip, Guid> tripRepository)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
        _payOsPaymentLogic = payOsPaymentLogic;
        _tripRepository = tripRepository;
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

        // Begin transaction
        await _bookingRepository.ExecuteInTransactionAsync(async () =>
        {
            var totalCost = tripExist.Bookings.Sum(b => b.TotalCost);
            
            var payment = new Payment
            {
                TripId = request.TripId,
                Amount = totalCost,
                Status = ConstantEnum.BookingStatus.Pending.ToString(),
                Method = ConstantEnum.PaymentMethod.PayOs.ToString(),
                PaidAt = DateTime.Now,
                TransactionCode = "200",
            };
            await _paymentRepository.AddAsync(payment);
            
            var payBookingResult = await _payOsPaymentLogic.PayBooking(request.TripId, tripExist.TripName);

            await _paymentRepository.SaveChangesAsync(identityEntity.Email);
            
            response.Success = true;
            response.Response = payBookingResult.Response;
            response.SetMessage(MessageId.I00001);
            return true;
        });
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
    /// Select Payment Booking Detail by PaymentId
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
}