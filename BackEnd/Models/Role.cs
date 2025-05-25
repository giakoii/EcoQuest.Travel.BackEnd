namespace BackEnd.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<Auth> Users { get; set; } = new List<Auth>();

    public virtual ICollection<Auth> UsersNavigation { get; set; } = new List<Auth>();
}
