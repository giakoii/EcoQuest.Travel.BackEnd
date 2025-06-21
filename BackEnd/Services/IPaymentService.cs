using BackEnd.DTOs.Ecq110;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IPaymentService
{
    Task<Ecq110InsertPaymentResponse> InsertPayment(Ecq110InsertPaymentRequest request, IdentityEntity identityEntity);
}