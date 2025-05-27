using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Account> Users { get; set; } = new List<Account>();
}
