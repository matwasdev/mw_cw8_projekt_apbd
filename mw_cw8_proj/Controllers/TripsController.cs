using Microsoft.AspNetCore.Mvc;
using mw_cw8_proj.Services;

namespace mw_cw8_proj.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }
    

    // Ten endpoint pobiera wszystkie dostepne wycieczki oraz podstawowe informacje o nich.
    // Dodatkowo zwracamy rowniez DTO krajów dla kazdej wycieczki.
    [HttpGet]
    public async Task<ActionResult> GetTripsAsync()
    {
        try
        {
            var trips = await _tripsService.GetTripsAsync();
            return Ok(trips);
        }
        catch (Exception)
        {
            return StatusCode(500,"Internal server error occured");
        }
    }
    
    
}