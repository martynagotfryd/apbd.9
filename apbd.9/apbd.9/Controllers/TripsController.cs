using apbd._9.Data;
using apbd._9.DTOs;
using apbd._9.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apbd._9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly Apbd4Context _context;

    public TripsController(Apbd4Context context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTripsSortedByStartDate([FromQuery] int? pageNum, int pageSize = 10)
    {
        int pageNumber = pageNum ?? 1;

        var trips = _context.Trips.Select(e => new TripDto()
            {
                Name = e.Name,
                Description = e.Description,
                DateFrom = e.DateFrom,
                DateTo = e.DateTo,
                Countries = e.IdCountries.Select(c => new CountryDto()
                {
                    Name = c.Name
                }),
                Clients = e.ClientTrips.Select(e => new ClientDto()
                {
                    FirstName = e.IdClientNavigation.FirstName,
                    LastName = e.IdClientNavigation.LastName
                })
            })
            .OrderBy(e => e.DateFrom);
        
        var paginatedTrips = await trips
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(paginatedTrips);
    }
}
