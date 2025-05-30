﻿using System;
using System.Collections.Generic;

namespace BackEnd.Models;

public partial class Account
{
    public Guid AccountId { get; set; }

    public Guid RoleId { get; set; }

    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public int AccessFailedCount { get; set; }

    public string? Key { get; set; }

    public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();

    public virtual Role Role { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
