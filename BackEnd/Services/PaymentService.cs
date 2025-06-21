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
}