using BackEnd.DTOs.Ecq110;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services;

public class BookingService : IBookingService
{
    private readonly IBaseRepository<Booking, Guid> _bookingRepository;
    private readonly IBaseRepository<Trip, Guid> _tripRepository;
    private readonly IBaseRepository<TripSchedule, Guid> _tripScheduleRepository;
    private readonly IBaseRepository<HotelRoom, Guid> _hotelRoomRepository;
    private readonly IBaseRepository<Payment, Guid> _paymentRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="bookingRepository"></param>
    /// <param name="tripRepository"></param>
    /// <param name="tripScheduleRepository"></param>
    /// <param name="hotelRoomRepository"></param>
    /// <param name="paymentRepository"></param>
    public BookingService(IBaseRepository<Booking, Guid> bookingRepository, IBaseRepository<Trip, Guid> tripRepository,
        IBaseRepository<TripSchedule, Guid> tripScheduleRepository,
        IBaseRepository<HotelRoom, Guid> hotelRoomRepository, IBaseRepository<Payment, Guid> paymentRepository)
    {
        _bookingRepository = bookingRepository;
        _tripRepository = tripRepository;
        _tripScheduleRepository = tripScheduleRepository;
        _hotelRoomRepository = hotelRoomRepository;
        _paymentRepository = paymentRepository;
    }

    /// <summary>
    /// Insert 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<Ecq110InsertBookingTripResponse> InsertBookingTrip(Ecq110InsertBookingTripRequest request, IdentityEntity identityEntity)
    {
        var response = new Ecq110InsertBookingTripResponse { Success = false };

        // Validate trip and owner trip
        var trip = await _tripRepository.Find(x => x.TripId == request.TripId && x.IsActive == true).FirstOrDefaultAsync();
        if (trip == null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.TripNotFound);
            return response;
        }

        if (trip.UserId != Guid.Parse(identityEntity.UserId))
        {
            response.SetMessage(MessageId.I00000, CommonMessages.NotAuthorizedToManageTrip);
            return response;
        }
        
        // Check if trip is payment
        var payment = await _paymentRepository
            .Find(x => x.TripId == request.TripId && x.IsActive && x.Status != nameof(ConstantEnum.PaymentStatus.Cancelled))
            .FirstOrDefaultAsync();
        if (payment != null)
        {
            response.SetMessage(MessageId.I00000, "Trip is already paid.");
            return response;
        }

        // Check if booking already exists for the trip
        var bookingExist = await _bookingRepository.Find(x => x.TripId == request.TripId && x.IsActive == true).FirstOrDefaultAsync();
        if (bookingExist != null)
        {
            response.SetMessage(MessageId.I00000, CommonMessages.BookingAlreadyExists);
            return response;
        }

        // Validate trip schedule
        var tripSchedules = await _tripScheduleRepository
            .Find(x => x.TripId == trip.TripId && x.IsActive && x.ServiceId != null).ToListAsync();
        if (!tripSchedules.Any())
        {
            response.SetMessage(MessageId.I00000, CommonMessages.TripScheduleNotFound);
            return response;
        }

        // Begin transaction
        await _bookingRepository.ExecuteInTransactionAsync(async () =>
        {
            var bookings = new List<Booking>();

            var hotelGroups = tripSchedules
                .Where(x => x.ServiceType == ConstantEnum.EntityType.Hotel.ToString())
                .GroupBy(x => x.ServiceId);

            foreach (var group in hotelGroups)
            {
                var hotelId = group.Key!.Value;
                
                var schedulesForHotel = group.ToList();
                var checkinDate = schedulesForHotel.Min(x => x.ScheduleDate);
                var checkoutDate = schedulesForHotel.Max(x => x.ScheduleDate).AddDays(1);

                // Insert new booking
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    TripId = trip.TripId,
                    UserId = trip.UserId,
                    ServiceId = hotelId,
                    ServiceType = ConstantEnum.EntityType.Hotel.ToString(),
                    ScheduleDate = checkinDate,
                    TotalCost = 0,
                    NumberOfGuests = trip.NumberOfPeople ?? 0,
                    Status = ConstantEnum.BookingStatus.Pending.ToString(),
                };

                var hotelBookings = new List<HotelBooking>();
                decimal totalCost = 0;

                if (request.BookingHotelRooms != null)
                {
                    foreach (var hotelRoom in request.BookingHotelRooms)
                    {
                        // Validate hotel room
                        var room = await _hotelRoomRepository.Find(x =>
                            x.HotelId == hotelId &&
                            x.RoomId == hotelRoom.HotelRoomId &&
                            x.IsActive == true && x.NumberOfRoomsAvailable > 0).FirstOrDefaultAsync();

                        if (room == null)
                        {
                            response.SetMessage(MessageId.I00000, CommonMessages.HotelRoomNotFound);
                            return false;
                        }

                        // Calculate nights
                        var nights = (checkoutDate.DayNumber - checkinDate.DayNumber);
                        if (nights < 1) nights = 1;
                        
                        var numberOfGuests = trip.NumberOfPeople ?? 1;

                        // Calculate required rooms based on max guests per room
                        int requiredRooms = (int)Math.Ceiling((double) numberOfGuests / room.MaxGuests);
                        if (room.NumberOfRoomsAvailable < requiredRooms)
                        {
                            response.SetMessage(MessageId.I00000, "Not enough rooms available to accommodate all guests.");
                            return false;
                        }


                        // Insert hotel booking
                        var hotelBooking = new HotelBooking
                        {
                            HotelBookingId = Guid.NewGuid(),
                            BookingId = booking.BookingId,
                            RoomId = room.RoomId,
                            CheckinDate = checkinDate,
                            CheckoutDate = checkoutDate,
                            NumberOfRooms = requiredRooms,
                            PricePerNight = room.PricePerNight,
                        };

                        // Calculate total cost
                        totalCost += room.PricePerNight * nights;
                        hotelBookings.Add(hotelBooking);
                    }
                }

                booking.TotalCost = totalCost;
                booking.HotelBookings = hotelBookings;
                await _bookingRepository.AddAsync(booking);
            }
            
            // Insert other service bookings
            var otherServiceSchedules = tripSchedules
                .Where(x => x.ServiceType != ConstantEnum.EntityType.Hotel.ToString());

            foreach (var schedule in otherServiceSchedules)
            {
                
                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    UserId = Guid.Parse(identityEntity.UserId),
                    TripId = trip.TripId,
                    ServiceId = schedule.ServiceId,
                    ServiceType = schedule.ServiceType,
                    ScheduleDate = schedule.ScheduleDate,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    TotalCost = schedule.EstimatedCost ?? 0,
                    NumberOfGuests = trip.NumberOfPeople ?? 1,
                    Status = ConstantEnum.BookingStatus.Pending.ToString(),
                };
                await _bookingRepository.AddAsync(booking);
            }
            
            await _bookingRepository.SaveChangesAsync(identityEntity.Email);

            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
}