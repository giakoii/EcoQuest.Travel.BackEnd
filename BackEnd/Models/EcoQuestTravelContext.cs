using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models;

public partial class EcoQuestTravelContext : DbContext
{
    public EcoQuestTravelContext()
    {
    }

    public EcoQuestTravelContext(DbContextOptions<EcoQuestTravelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SystemConfig> SystemConfigs { get; set; }

    public virtual DbSet<Auth> Users { get; set; }

    public virtual DbSet<VwEmailTemplateVerifyUser> VwEmailTemplateVerifyUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            DotNetEnv.Env.Load(); 

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing CONNECTION_STRING environment variable");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseOpenIddict();

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailTemplate");

            entity.ToTable("EmailTemplate");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ScreenName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("screen_name");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("update_by");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updated_at");
        });
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<SystemConfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemConfig");

            entity.ToTable("SystemConfig");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.Value).HasColumnName("value");
        });

modelBuilder.Entity<Auth>(entity =>
{
    entity.ToTable("Auths");

    entity.HasKey(e => e.Id);

    entity.Property(e => e.Id)
        .HasDefaultValueSql("(newid())")
        .HasColumnName("id");

    entity.Property(e => e.RoleId)
        .IsRequired()
        .HasColumnName("role_id");

    entity.Property(e => e.Email)
        .HasMaxLength(256)
        .HasColumnName("email");

    entity.Property(e => e.EmailConfirmed)
        .IsRequired()
        .HasColumnName("email_confirmed");

    entity.Property(e => e.PasswordHash)
        .HasColumnName("password_hash");

    entity.Property(e => e.SecurityStamp)
        .HasColumnName("security_stamp");

    entity.Property(e => e.ConcurrencyStamp)
        .HasColumnName("concurrency_stamp");

    entity.Property(e => e.PhoneNumber)
        .HasColumnName("phone_number");

    entity.Property(e => e.LockoutEnd)
        .HasColumnName("lockout_end");

    entity.Property(e => e.CreatedAt)
        .IsRequired()
        .HasColumnName("created_at");

    entity.Property(e => e.CreatedBy)
        .IsRequired()
        .HasColumnName("created_by");

    entity.Property(e => e.IsActive)
        .IsRequired()
        .HasColumnName("is_active");

    entity.Property(e => e.UpdatedAt)
        .IsRequired()
        .HasColumnName("updated_at");

    entity.Property(e => e.UpdatedBy)
        .IsRequired()
        .HasColumnName("updated_by");

    entity.Property(e => e.AccessFailedCount)
        .IsRequired()
        .HasColumnName("access_failed_count");

    entity.Property(e => e.Key)
        .HasColumnName("key");

    // Index
    entity.HasIndex(e => e.RoleId)
        .HasDatabaseName("IX_Users_RoleId");

    // One-to-many: User → Role
    entity.HasOne(d => d.Role)
        .WithMany(p => p.Users)
        .HasForeignKey(d => d.RoleId)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .IsRequired();

    // Many-to-many: Users ⇄ Roles
    entity.HasMany(d => d.Roles)
        .WithMany(p => p.UsersNavigation)
        .UsingEntity<Dictionary<string, object>>(
            "UserRole",
            r => r.HasOne<Role>().WithMany()
                .HasForeignKey("RoleId")
                .HasConstraintName("FK_AspNetUserRoles_Roles_RoleId"),
            l => l.HasOne<Auth>().WithMany()
                .HasForeignKey("UserId")
                .HasConstraintName("FK_AspNetUserRoles_Users_UserId"),
            j =>
            {
                j.HasKey("UserId", "RoleId")
                    .HasName("PK_AspNetUserRoles");

                j.ToTable("UserRoles");

                j.Property<Guid>("UserId").HasColumnName("user_id");
                j.Property<Guid>("RoleId").HasColumnName("role_id");

                j.HasIndex("RoleId")
                    .HasDatabaseName("IX_AspNetUserRoles_RoleId");
            });
});

modelBuilder.Entity<User>(entity =>
{
    entity.ToTable("Users");

    entity.HasKey(e => e.Id);

    entity.Property(e => e.Id)
        .HasDefaultValueSql("(newid())")
        .HasColumnName("id");

    entity.Property(e => e.AuthId)
        .IsRequired()
        .HasColumnName("auth_id");

    entity.Property(e => e.FirstName)
        .HasMaxLength(100)
        .HasColumnName("first_name");
    
    entity.Property(e => e.LastName)
        .HasMaxLength(100)
        .HasColumnName("last_name");

    entity.Property(e => e.DateOfBirth)
        .HasColumnName("date_of_birth");

    entity.Property(e => e.Gender)
        .HasMaxLength(1)
        .HasColumnName("gender");

    entity.Property(e => e.Address)
        .HasMaxLength(256)
        .HasColumnName("address");

    entity.Property(e => e.AvatarUrl)
        .HasMaxLength(512)
        .HasColumnName("avatar_url");

    entity.Property(e => e.IsActive)
        .HasMaxLength(512)
        .HasColumnName("is_active");

    entity.Property(e => e.CreatedAt)
        .HasColumnName("created_at");

    entity.Property(e => e.CreatedBy)
        .HasMaxLength(100)
        .HasColumnName("created_by");

    entity.Property(e => e.UpdatedAt)
        .HasColumnName("updated_at");

    entity.Property(e => e.UpdatedBy)
        .HasMaxLength(100)
        .HasColumnName("updated_by");
    
    entity.HasOne(d => d.Auth)
        .WithMany()
        .HasForeignKey(d => d.AuthId)
        .OnDelete(DeleteBehavior.Cascade)
        .HasConstraintName("FK_Users_Auths_AuthId");

    entity.HasIndex(e => e.AuthId)
        .HasDatabaseName("IX_Users_AuthId")
        .IsUnique();
});

        modelBuilder.Entity<VwEmailTemplateVerifyUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_EmailTemplateVerifyUser");

            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreateBy)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("create_by");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ScreenName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("screen_name");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("update_by");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
