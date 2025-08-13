using BackEnd.Models;
using BackEnd.Repositories;

namespace BackEnd.Services
{
    /// <summary>
    /// Service wrapper để gộp các repository liên quan đến TripSchedule
    /// </summary>
    public class TripScheduleRepositoryWrapper
    {
        public IBaseRepository<Trip, Guid> TripRepository { get; }
        public IBaseRepository<TripSchedule, Guid> TripScheduleRepository { get; }
        public IBaseRepository<TripDestination, Guid> TripDestinationRepository { get; }
        public IBaseRepository<AttractionDetail, Guid> AttractionDetailRepository { get; }
        public IBaseRepository<RestaurantDetail, Guid> RestaurantDetailRepository { get; }
        public IBaseRepository<Hotel, Guid> HotelRepository { get; }
        public IBaseRepository<HotelRoom, Guid> HotelRoomRepository { get; }
        
        public IBaseRepository<User, Guid> UserRepository { get; }

        public TripScheduleRepositoryWrapper(
            IBaseRepository<Trip, Guid> tripRepository,
            IBaseRepository<TripSchedule, Guid> tripScheduleRepository,
            IBaseRepository<TripDestination, Guid> tripDestinationRepository,
            IBaseRepository<AttractionDetail, Guid> attractionDetailRepository,
            IBaseRepository<RestaurantDetail, Guid> restaurantDetailRepository,
            IBaseRepository<Hotel, Guid> hotelRepository,
            IBaseRepository<HotelRoom, Guid> hotelRoomRepository, IBaseRepository<User, Guid> userRepository)
        {
            TripRepository = tripRepository;
            TripScheduleRepository = tripScheduleRepository;
            TripDestinationRepository = tripDestinationRepository;
            AttractionDetailRepository = attractionDetailRepository;
            RestaurantDetailRepository = restaurantDetailRepository;
            HotelRepository = hotelRepository;
            HotelRoomRepository = hotelRoomRepository;
            UserRepository = userRepository;
        }
    }
}
