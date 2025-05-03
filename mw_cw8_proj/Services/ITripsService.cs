using mw_cw8_proj.Models.DTOs;

namespace mw_cw8_proj.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetTripsAsync();
    
}