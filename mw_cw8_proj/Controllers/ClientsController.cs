using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using mw_cw8_proj.Exceptions;
using mw_cw8_proj.Models.DTOs;
using mw_cw8_proj.Services;

namespace mw_cw8_proj.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }

    
    [HttpGet("{id}/trips")]
    public async Task<ActionResult> GetClientTripsAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid ClientId");
        }

        try
        {
            var clientTrips = await _clientsService.GetClientTripsAsync(id);

            return Ok(clientTrips);
        }
        catch (ClientNotFoundException)
        {
            return NotFound("Client with id: " + id + " not found");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error occured");
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateClientAsync(CreateClientDTO client)
    {
        
        //VALIDATION HERE
        //VALIDATION HERE
        //VALIDATION HERE
        //VALIDATION HERE
        //VALIDATION HERE
        //VALIDATION HERE
        //VALIDATION HERE
        //VALIDATION HERE
        //VALIDATION HERE
        
        try
        {
            var clientId = await _clientsService.CreateClientAsync(client);
            return Ok(clientId);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error occured");
        }
    }

    [HttpPut("{id}/trips/{tripId}")]
    public async Task<ActionResult> RegisterClientForTripAsync(int id,int tripId)
    {
        if (id <= 0 || tripId <= 0)
            return BadRequest("Invalid ClientId or TripId");

        try
        {
            await _clientsService.RegisterClientForTripAsync(id, tripId);

            return Ok("Client with id: " + id + " successfully registered for trip " + tripId);
        }
        catch (ClientNotFoundException)
        {
            return NotFound("Client with id: " + id + " does not exist");
        }
        catch (TripNotFoundException)
        {
            return NotFound("Trip with id: " + tripId + " does not exist");
        }
        catch (MaxPeopleExceededException)
        {
            return Conflict("Max people exceeded for trip: " + tripId);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error occured");
        }
    }
    
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<ActionResult> DeleteClientFromTripAsync(int id, int tripId)
    {
           if(id <= 0 || tripId <= 0)
               return BadRequest("Invalid ClientId");

           try
           {
               await _clientsService.DeleteClientFromTripAsync(id, tripId);

               return Ok("Client with id: " + id + " successfully removed from trip with id: " + tripId);
           }
           catch (TripWithClientNotFoundException)
           {
               return NotFound("Cannot find trip id: " + tripId + " registered for client: " + id);
           }
           catch (Exception)
           {
               return StatusCode(500, "Internal server error occured");
           }
    }

    
    
}