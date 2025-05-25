namespace BackEnd.Models;

public partial class User
{
    public Guid Id { get; set; }

    public Guid AuthId { get; set; }

    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public byte? Gender { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }
    
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual Auth Auth { get; set; } = null!;
}