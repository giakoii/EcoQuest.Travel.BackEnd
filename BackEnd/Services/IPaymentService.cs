using BackEnd.DTOs.Ecq110;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IPaymentService
{
    Task<Ecq110InsertPaymentResponse> InsertPayment(Ecq110InsertPaymentRequest request, IdentityEntity identityEntity);
    
    Task<Ecq300SelectPaymentBookingsResponse> SelectPaymentBooking(Ecq300SelectPaymentBookingsRequest request, IdentityEntity identityEntity);
    
    Task<Ecq300SelectPaymentBookingResponse> SelectPaymentBookingDetail(Ecq300SelectPaymentBookingRequest request, IdentityEntity identityEntity);
}