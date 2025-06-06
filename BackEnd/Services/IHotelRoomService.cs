using BackEnd.DTOs.Ecq211;
using BackEnd.SystemClient;

namespace BackEnd.Services;

public interface IHotelRoomService
{
    /// <summary>
    /// Insert a new hotel room
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    Task<Ecq211InsertHotelRoomResponse> InsertHotelRoom(Ecq211InsertHotelRoomRequest request, IdentityEntity identityEntity);
    
    /// <summary>
    /// Select all rooms for a specific hotel
    /// </summary>
    /// <param name="hotelId"></param>
    /// <returns></returns>
    Task<Ecq211SelectHotelRoomsResponse> SelectHotelRooms(Guid hotelId);
    
    /// <summary>
    /// Select a specific hotel room by ID
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    Task<Ecq211SelectHotelRoomResponse> SelectHotelRoom(Guid roomId);
    
    /// <summary>
    /// Update an existing hotel room
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    Task<Ecq211UpdateHotelRoomResponse> UpdateHotelRoom(Ecq211UpdateHotelRoomRequest request, IdentityEntity identityEntity);
}
