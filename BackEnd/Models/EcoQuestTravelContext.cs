using DotNetEnv;
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

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<DestinationImage> DestinationImages { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SystemConfig> SystemConfigs { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwEmailTemplateVerifyUser?> VwEmailTemplateVerifyUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            Env.Load(); 

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing CONNECTION_STRING environment variable");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseOpenIddict();
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK_Auths");

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.Property(e => e.AccountId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("account_id");
            entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Auths_Roles_role_id");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_AspNetUserRoles_Roles_RoleId"),
                    l => l.HasOne<Account>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_AspNetUserRoles_Users_UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK_AspNetUserRoles");
                        j.ToTable("UserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<Guid>("RoleId").HasColumnName("role_id");
                    });
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__2975AA28AF9F4DBF");

            entity.Property(e => e.BlogId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("blog_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .HasMaxLength(300)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blogs__author_id__01142BA1");

            entity.HasOne(d => d.Destination).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK__Blogs__destinati__02084FDA");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DestinationId).HasName("PK__Destinat__55015391AE604262");

            entity.Property(e => e.DestinationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("destination_id");
            entity.Property(e => e.AddedBy).HasColumnName("added_by");
            entity.Property(e => e.AddressLine)
                .HasMaxLength(255)
                .HasColumnName("address_line");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.District)
                .HasMaxLength(50)
                .HasColumnName("district");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Province).HasColumnName("province");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.Ward)
                .HasMaxLength(50)
                .HasColumnName("ward");

            entity.HasOne(d => d.AddedByNavigation).WithMany(p => p.Destinations)
                .HasForeignKey(d => d.AddedBy)
                .HasConstraintName("FK__Destinati__added__71D1E811");
        });

        modelBuilder.Entity<DestinationImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Destinat__DC9AC955E496E1A0");

            entity.Property(e => e.ImageId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("image_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Destination).WithMany(p => p.DestinationImages)
                .HasForeignKey(d => d.DestinationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Destinati__desti__06CD04F7");
        });

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

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PK__Hotels__45FE7E26BB1E4578");

            entity.Property(e => e.HotelId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("hotel_id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Destination).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK__Hotels__destinat__76969D2E");

            entity.HasOne(d => d.Owner).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK__Hotels__owner_id__778AC167");
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

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("PK__Trips__302A5D9E6F00BEEE");

            entity.Property(e => e.TripId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("trip_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.NumberOfPeople).HasColumnName("number_of_people");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TotalEstimatedCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_estimated_cost");
            entity.Property(e => e.TripName)
                .HasMaxLength(200)
                .HasColumnName("trip_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Trips)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Trips__user_id__7C4F7684");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.AuthId, "IX_Users_AuthId").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(256)
                .HasColumnName("address");
            entity.Property(e => e.AuthId).HasColumnName("auth_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(512)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Auth).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.AuthId)
                .HasConstraintName("FK_Users_Auths_AuthId");
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
