using apbd._9.Data;
using apbd._9.DTOs;
using apbd._9.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, AssignClientDTO newClient)
    {
        var client = await _context.Clients.SingleOrDefaultAsync(e => e.Pesel == newClient.Pesel);
        var trip = await _context.Trips.SingleOrDefaultAsync(e => e.IdTrip == idTrip);

        if (trip == null)
        {
            if (trip.DateFrom < DateTime.Now)
            {
                return BadRequest("Trip with given ID already took place");
            }

            return NotFound("Trip with given ID doesnt exist");
        }

        if (client != null)
        {
            if (client.ClientTrips.Any(e => e.IdTrip == idTrip))
            {
                return BadRequest($"Client with given PESEL is already assigned to this trip");
            }

            return BadRequest($"Client with the given PESEL already exists");
        }

        client = new Client()
        {
            FirstName = newClient.FirstName,
            LastName = newClient.LastName,
            Email = newClient.Email,
            Pesel = newClient.Pesel,
            Telephone = newClient.Telephone
        };
        
        client.ClientTrips.Add(new ClientTrip()
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            PaymentDate = newClient.PaymentDate,
            RegisteredAt = DateTime.Now,
            IdClientNavigation = client,
            IdTripNavigation = trip
        });

        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        
        return Ok();
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClientWithId(int idClient)
    {
        var client = await _context.Clients.SingleOrDefaultAsync(e => e.IdClient == idClient);

        if (client == null)
        {
            return NotFound("Client with the given ID does not exist.");
        }

        if (client.ClientTrips.Any())
        {
            return BadRequest("Client cannot be deleted because they have assigned trips.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        
        return Ok();
    }

}
