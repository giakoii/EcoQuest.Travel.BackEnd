using BackEnd.Controllers;

namespace BackEnd.DTOs.Ecq310;

public class Ecq310SelectPaymentResponse : AbstractApiResponse<Ecq310SelectPaymentEntity>
{
    public override Ecq310SelectPaymentEntity Response { get; set; }
}

public class Ecq310SelectPaymentEntity
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Status { get; set; }
    public Ecq310SelectPaymentTrip Trip { get; set; }
    public Ecq310SelectPaymentUser User { get; set; }
}

public class Ecq310SelectPaymentUser
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public byte? Gender { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public string UserType { get; set; } = null!;
}

public class Ecq310SelectPaymentTrip
{
    public Guid TripId { get; set; }

    public Guid UserId { get; set; }

    public string? TripName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? NumberOfPeople { get; set; }

    public string Description { get; set; }
}