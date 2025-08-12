using System;
using System.Collections.Generic;
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

    public virtual DbSet<AttractionDetail> AttractionDetails { get; set; }

    public virtual DbSet<AttractionRating> AttractionRatings { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelBooking> HotelBookings { get; set; }

    public virtual DbSet<HotelRating> HotelRatings { get; set; }

    public virtual DbSet<HotelRoom> HotelRooms { get; set; }

    public virtual DbSet<Image> Images { get; set; }
    
    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<PartnerPartnerType> PartnerPartnerTypes { get; set; }

    public virtual DbSet<PartnerType> PartnerTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<RestaurantDetail> RestaurantDetails { get; set; }

    public virtual DbSet<RestaurantRating> RestaurantRatings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SystemConfig> SystemConfigs { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<TripDestination> TripDestinations { get; set; }

    public virtual DbSet<TripSchedule> TripSchedules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwAttraction> VwAttractions { get; set; }

    public virtual DbSet<VwAttractionRating> VwAttractionRatings { get; set; }

    public virtual DbSet<VwBlog> VwBlogs { get; set; }

    public virtual DbSet<VwBooking> VwBookings { get; set; }

    public virtual DbSet<VwComment> VwComments { get; set; }

    public virtual DbSet<VwDestination> VwDestinations { get; set; }

    public virtual DbSet<VwEcq310SelectPartner> VwEcq310SelectPartners { get; set; }

    public virtual DbSet<VwEmailTemplateAccountInformation> VwEmailTemplateAccountInformations { get; set; }

    public virtual DbSet<VwEmailTemplateVerifyUser> VwEmailTemplateVerifyUsers { get; set; }

    public virtual DbSet<VwHotel> VwHotels { get; set; }

    public virtual DbSet<VwHotelBooking> VwHotelBookings { get; set; }

    public virtual DbSet<VwHotelRoom> VwHotelRooms { get; set; }

    public virtual DbSet<VwImage> VwImages { get; set; }

    public virtual DbSet<VwPartnerPartnerType> VwPartnerPartnerTypes { get; set; }

    public virtual DbSet<VwPartnerType> VwPartnerTypes { get; set; }

    public virtual DbSet<VwPayment> VwPayments { get; set; }

    public virtual DbSet<VwPaymentBookingTrip> VwPaymentBookingTrips { get; set; }

    public virtual DbSet<VwRestaurant> VwRestaurants { get; set; }

    public virtual DbSet<VwRestaurantRating> VwRestaurantRatings { get; set; }

    public virtual DbSet<VwTrip> VwTrips { get; set; }

    public virtual DbSet<VwTripSchedule> VwTripSchedules { get; set; }

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

        modelBuilder.Entity<AttractionDetail>(entity =>
        {
            entity.HasKey(e => e.AttractionId);

            entity.Property(e => e.AttractionId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("attraction_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.AgeLimit).HasColumnName("age_limit");
            entity.Property(e => e.AttractionName)
                .HasMaxLength(100)
                .HasColumnName("attraction_name");
            entity.Property(e => e.AttractionType)
                .HasMaxLength(100)
                .HasColumnName("attraction_type");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.GuideAvailable)
                .HasDefaultValue(false)
                .HasColumnName("guide_available");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.TicketPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ticket_price");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Destination).WithMany(p => p.AttractionDetails)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK_AttractionDetails_Destinations");

            entity.HasOne(d => d.Partner).WithMany(p => p.AttractionDetails)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attractio__partn__2CF2ADDF");
        });

        modelBuilder.Entity<AttractionRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Attracti__D35B278BB997F4A9");

            entity.Property(e => e.RatingId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rating_id");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Attraction).WithMany(p => p.AttractionRatings)
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_AttractionRatings_AttractionDetails");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__2975AA28AF9F4DBF");

            entity.HasIndex(e => e.TripId, "UQ_Blogs_TripID").IsUnique();

            entity.Property(e => e.BlogId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("blog_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .HasMaxLength(300)
                .HasColumnName("title");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blogs__author_id__01142BA1");

            entity.HasOne(d => d.Trip).WithOne(p => p.Blog)
                .HasForeignKey<Blog>(d => d.TripId)
                .HasConstraintName("FK_Blogs_Trips");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__5DE3A5B11AAB583B");

            entity.Property(e => e.BookingId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.NumberOfGuests).HasColumnName("number_of_guests");
            entity.Property(e => e.ScheduleDate).HasColumnName("schedule_date");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("service_type");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_cost");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Trip).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__trip_i__3552E9B6");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__user_i__345EC57D");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.CommentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("comment_id");
            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Blog).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_Comments_Blogs");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DestinationId).HasName("PK__Destinat__55015391AE604262");

            entity.Property(e => e.DestinationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("destination_id");
            entity.Property(e => e.AddressLine)
                .HasMaxLength(255)
                .HasColumnName("address_line");
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
            entity.Property(e => e.MaxPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("max_price");
            entity.Property(e => e.MinPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("min_price");
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
                .HasConstraintName("FK_Hotels_Owner");
        });

        modelBuilder.Entity<HotelBooking>(entity =>
        {
            entity.HasKey(e => e.HotelBookingId).HasName("PK__HotelBoo__4D87FD92F97DE666");

            entity.Property(e => e.HotelBookingId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("hotel_booking_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CheckinDate).HasColumnName("checkin_date");
            entity.Property(e => e.CheckoutDate).HasColumnName("checkout_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.NumberOfRooms).HasColumnName("number_of_rooms");
            entity.Property(e => e.PricePerNight)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Booking).WithMany(p => p.HotelBookings)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HotelBook__booki__44952D46");

            entity.HasOne(d => d.Room).WithMany(p => p.HotelBookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HotelBook__room___4589517F");
        });

        modelBuilder.Entity<HotelRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__HotelRat__D35B278B5B2E1207");

            entity.Property(e => e.RatingId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rating_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelRatings)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_HotelRating_Hotels");

            entity.HasOne(d => d.User).WithMany(p => p.HotelRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelRating_Users");
        });

        modelBuilder.Entity<HotelRoom>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__HotelRoo__19675A8A9F4DAD98");

            entity.Property(e => e.RoomId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("room_id");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.BedType)
                .HasMaxLength(50)
                .HasColumnName("bed_type");
            entity.Property(e => e.CheckinTime).HasColumnName("checkin_time");
            entity.Property(e => e.CheckoutTime).HasColumnName("checkout_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FreeCancellationUntil).HasColumnName("free_cancellation_until");
            entity.Property(e => e.HasAirConditioner)
                .HasDefaultValue(true)
                .HasColumnName("has_air_conditioner");
            entity.Property(e => e.HasBalcony)
                .HasDefaultValue(false)
                .HasColumnName("has_balcony");
            entity.Property(e => e.HasBreakfast)
                .HasDefaultValue(false)
                .HasColumnName("has_breakfast");
            entity.Property(e => e.HasMinibar)
                .HasDefaultValue(false)
                .HasColumnName("has_minibar");
            entity.Property(e => e.HasPrivateBathroom)
                .HasDefaultValue(true)
                .HasColumnName("has_private_bathroom");
            entity.Property(e => e.HasTv)
                .HasDefaultValue(true)
                .HasColumnName("has_tv");
            entity.Property(e => e.HasWifi)
                .HasDefaultValue(true)
                .HasColumnName("has_wifi");
            entity.Property(e => e.HasWindow)
                .HasDefaultValue(true)
                .HasColumnName("has_window");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.IsRefundable)
                .HasDefaultValue(true)
                .HasColumnName("is_refundable");
            entity.Property(e => e.MaxGuests).HasColumnName("max_guests");
            entity.Property(e => e.NumberOfBeds).HasColumnName("number_of_beds");
            entity.Property(e => e.NumberOfRoomsAvailable).HasColumnName("number_of_rooms_available");
            entity.Property(e => e.PricePerNight)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomType)
                .HasMaxLength(100)
                .HasColumnName("room_type");
            entity.Property(e => e.SmokingAllowed)
                .HasDefaultValue(false)
                .HasColumnName("smoking_allowed");
            entity.Property(e => e.SpecialNote)
                .HasMaxLength(500)
                .HasColumnName("special_note");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelRooms)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HotelRoom__hotel__1DB06A4F");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Destinat__DC9AC955E496E1A0");

            entity.Property(e => e.ImageId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("image_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
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
        });
        
        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.PartnerId).HasName("PK__Partners__576F1B273FD02EC7");

            entity.Property(e => e.PartnerId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("partner_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactName)
                .HasMaxLength(100)
                .HasColumnName("contact_name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.Verified)
                .HasDefaultValue(false)
                .HasColumnName("verified");

            entity.HasOne(d => d.Account).WithMany(p => p.Partners)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Partners__accoun__17036CC0");
        });

        modelBuilder.Entity<PartnerPartnerType>(entity =>
        {
            entity.HasKey(e => new { e.PartnerId, e.TypeId }).HasName("PK__Partner___C5AF1B7EDF2815DF");

            entity.ToTable("Partner_PartnerType");

            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Partner).WithMany(p => p.PartnerPartnerTypes)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Partner_P__partn__339FAB6E");

            entity.HasOne(d => d.Type).WithMany(p => p.PartnerPartnerTypes)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Partner_P__type___3493CFA7");
        });

        modelBuilder.Entity<PartnerType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__PartnerT__2C000598803BD007");

            entity.Property(e => e.TypeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("type_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAD7261000");

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("method");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(100)
                .HasColumnName("transaction_code");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Trip).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Trips");
        });

        modelBuilder.Entity<RestaurantDetail>(entity =>
        {
            entity.HasKey(e => e.RestaurantId);

            entity.Property(e => e.RestaurantId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("restaurant_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.CuisineType)
                .HasMaxLength(100)
                .HasColumnName("cuisine_type");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.HasVegetarian).HasColumnName("has_vegetarian");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("max_price");
            entity.Property(e => e.MinPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("min_price");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.RestaurantName)
                .HasMaxLength(100)
                .HasColumnName("restaurant_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Destination).WithMany(p => p.RestaurantDetails)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK_RestaurantDetails_Destinations");

            entity.HasOne(d => d.Partner).WithMany(p => p.RestaurantDetails)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__partn__2180FB33");
        });

        modelBuilder.Entity<RestaurantRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Restaura__D35B278BB3748C56");

            entity.Property(e => e.RatingId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rating_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.RestaurantRatings)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RestaurantRatings_RestaurantDetails");

            entity.HasOne(d => d.User).WithMany(p => p.RestaurantRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RestaurantRatings_Users");
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
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.NumberOfPeople).HasColumnName("number_of_people");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartingPointAddress)
                .HasMaxLength(255)
                .HasColumnName("starting_point_address");
            entity.Property(e => e.Status).HasColumnName("status");
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

        modelBuilder.Entity<TripDestination>(entity =>
        {
            entity.HasKey(e => e.TripDestinationId).HasName("PK__TripDest__329279A66A899C6A");

            entity.Property(e => e.TripDestinationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("trip_destination_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Destination).WithMany(p => p.TripDestinations)
                .HasForeignKey(d => d.DestinationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TripDesti__desti__4E53A1AA");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripDestinations)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TripDesti__trip___4D5F7D71");
        });

        modelBuilder.Entity<TripSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__TripSche__C46A8A6F9AE6C18A");

            entity.Property(e => e.ScheduleId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("schedule_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.EstimatedCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("estimated_cost");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.ScheduleDate).HasColumnName("schedule_date");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .HasColumnName("service_type");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripSchedules)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TripSched__trip___531856C7");
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
            entity.Property(e => e.UserType)
                .HasDefaultValue(1)
                .HasColumnName("user_type");
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

        modelBuilder.Entity<VwAttraction>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Attraction");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.AgeLimit).HasColumnName("age_limit");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.AttractionName)
                .HasMaxLength(100)
                .HasColumnName("attraction_name");
            entity.Property(e => e.AttractionType)
                .HasMaxLength(100)
                .HasColumnName("attraction_type");
            entity.Property(e => e.AverageRating)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("average_rating");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.DestinationName)
                .HasMaxLength(200)
                .HasColumnName("destination_name");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.GuideAvailable).HasColumnName("guide_available");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.TicketPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ticket_price");
            entity.Property(e => e.TotalRatings).HasColumnName("total_ratings");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<VwAttractionRating>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_AttractionRating");

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(512)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RatingId).HasColumnName("rating_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<VwBlog>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Blog");

            entity.Property(e => e.AuthorAvatar)
                .HasMaxLength(512)
                .HasColumnName("author_avatar");
            entity.Property(e => e.AuthorFirstName)
                .HasMaxLength(100)
                .HasColumnName("author_first_name");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.AuthorLastName)
                .HasMaxLength(100)
                .HasColumnName("author_last_name");
            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DestinationId)
                .HasMaxLength(4000)
                .HasColumnName("destination_id");
            entity.Property(e => e.DestinationName)
                .HasMaxLength(4000)
                .HasColumnName("destination_name");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Title)
                .HasMaxLength(300)
                .HasColumnName("title");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.TripName)
                .HasMaxLength(200)
                .HasColumnName("trip_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<VwBooking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Booking");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.NumberOfGuests).HasColumnName("number_of_guests");
            entity.Property(e => e.ScheduleDate).HasColumnName("schedule_date");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("service_type");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_cost");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<VwComment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Comment");

            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.BlogTitle)
                .HasMaxLength(300)
                .HasColumnName("blog_title");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CommenterName)
                .HasMaxLength(201)
                .HasColumnName("commenter_name");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<VwDestination>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Destination");

            entity.Property(e => e.AddressLine)
                .HasMaxLength(255)
                .HasColumnName("address_line");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.District)
                .HasMaxLength(50)
                .HasColumnName("district");
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
        });

        modelBuilder.Entity<VwEcq310SelectPartner>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Ecq310SelectPartner");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactName)
                .HasMaxLength(100)
                .HasColumnName("contact_name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.Verified).HasColumnName("verified");
        });

        modelBuilder.Entity<VwEmailTemplateAccountInformation>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_EmailTemplate_Account_Information");

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

        modelBuilder.Entity<VwHotel>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Hotel");

            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.AddressLine)
                .HasMaxLength(255)
                .HasColumnName("address_line");
            entity.Property(e => e.AverageRating)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("average_rating");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.DestinationName)
                .HasMaxLength(200)
                .HasColumnName("destination_name");
            entity.Property(e => e.District)
                .HasMaxLength(50)
                .HasColumnName("district");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.HotelDescription).HasColumnName("hotel_description");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.HotelName)
                .HasMaxLength(200)
                .HasColumnName("hotel_name");
            entity.Property(e => e.MaxPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("max_price");
            entity.Property(e => e.MinPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("min_price");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.Province).HasColumnName("province");
            entity.Property(e => e.TotalRatings).HasColumnName("total_ratings");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.Ward)
                .HasMaxLength(50)
                .HasColumnName("ward");
        });

        modelBuilder.Entity<VwHotelBooking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_HotelBooking");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CheckinDate).HasColumnName("checkin_date");
            entity.Property(e => e.CheckoutDate).HasColumnName("checkout_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.HotelBookingId).HasColumnName("hotel_booking_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.NumberOfRooms).HasColumnName("number_of_rooms");
            entity.Property(e => e.PricePerNight)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<VwHotelRoom>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Hotel_Room");

            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.BedType)
                .HasMaxLength(50)
                .HasColumnName("bed_type");
            entity.Property(e => e.CheckinTime).HasColumnName("checkin_time");
            entity.Property(e => e.CheckoutTime).HasColumnName("checkout_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FreeCancellationUntil).HasColumnName("free_cancellation_until");
            entity.Property(e => e.HasAirConditioner).HasColumnName("has_air_conditioner");
            entity.Property(e => e.HasBalcony).HasColumnName("has_balcony");
            entity.Property(e => e.HasBreakfast).HasColumnName("has_breakfast");
            entity.Property(e => e.HasMinibar).HasColumnName("has_minibar");
            entity.Property(e => e.HasPrivateBathroom).HasColumnName("has_private_bathroom");
            entity.Property(e => e.HasTv).HasColumnName("has_tv");
            entity.Property(e => e.HasWifi).HasColumnName("has_wifi");
            entity.Property(e => e.HasWindow).HasColumnName("has_window");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsAvailable).HasColumnName("is_available");
            entity.Property(e => e.IsRefundable).HasColumnName("is_refundable");
            entity.Property(e => e.MaxGuests).HasColumnName("max_guests");
            entity.Property(e => e.NumberOfBeds).HasColumnName("number_of_beds");
            entity.Property(e => e.NumberOfRoomsAvailable).HasColumnName("number_of_rooms_available");
            entity.Property(e => e.PricePerNight)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price_per_night");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RoomType)
                .HasMaxLength(100)
                .HasColumnName("room_type");
            entity.Property(e => e.SmokingAllowed).HasColumnName("smoking_allowed");
            entity.Property(e => e.SpecialNote)
                .HasMaxLength(500)
                .HasColumnName("special_note");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<VwImage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Image");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<VwPartnerPartnerType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Partner_PartnerType");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<VwPartnerType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_PartnerType");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<VwPayment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Payment");

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("method");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("service_type");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(100)
                .HasColumnName("transaction_code");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<VwPaymentBookingTrip>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_PaymentBookingTrip");

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("method");
            entity.Property(e => e.NumberOfPeople).HasColumnName("number_of_people");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PaymentCreatedAt).HasColumnName("payment_created_at");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartingPointAddress)
                .HasMaxLength(255)
                .HasColumnName("starting_point_address");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalEstimatedCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_estimated_cost");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(100)
                .HasColumnName("transaction_code");
            entity.Property(e => e.TripDescription).HasColumnName("trip_description");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.TripName)
                .HasMaxLength(200)
                .HasColumnName("trip_name");
            entity.Property(e => e.TripStatus).HasColumnName("trip_status");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(256)
                .HasColumnName("user_email");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<VwRestaurant>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_Restaurant");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.AverageRating)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("average_rating");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CuisineType)
                .HasMaxLength(100)
                .HasColumnName("cuisine_type");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.DestinationName)
                .HasMaxLength(200)
                .HasColumnName("destination_name");
            entity.Property(e => e.HasVegetarian).HasColumnName("has_vegetarian");
            entity.Property(e => e.MaxPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("max_price");
            entity.Property(e => e.MinPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("min_price");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.RestaurantName)
                .HasMaxLength(100)
                .HasColumnName("restaurant_name");
            entity.Property(e => e.TotalRatings).HasColumnName("total_ratings");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<VwRestaurantRating>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_RestaurantRating");

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(512)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RatingId).HasColumnName("rating_id");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<VwTrip>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Trip");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.NumberOfPeople).HasColumnName("number_of_people");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartingPointAddress)
                .HasMaxLength(255)
                .HasColumnName("starting_point_address");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalEstimatedCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_estimated_cost");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.TripName)
                .HasMaxLength(200)
                .HasColumnName("trip_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<VwTripSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TripSchedule");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.EstimatedCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("estimated_cost");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.ScheduleDate).HasColumnName("schedule_date");
            entity.Property(e => e.ScheduleDescription).HasColumnName("schedule_description");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.ScheduleTitle)
                .HasMaxLength(200)
                .HasColumnName("schedule_title");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .HasColumnName("service_type");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.TripName)
                .HasMaxLength(200)
                .HasColumnName("trip_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
