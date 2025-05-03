using mw_cw8_proj.Models.DTOs;

namespace mw_cw8_proj.Services;

public interface IClientsService
{
    Task<List<TripPaymentDTO>> GetClientTripsAsync(int id);
    Task<int> CreateClientAsync(CreateClientDTO client);

    Task RegisterClientForTripAsync(int id,int tripId);

    Task DeleteClientFromTripAsync(int id, int tripId);
}