namespace apbd._9.DTOs;

public class TripDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<CountryDto> Countries { get; set; } = new List<CountryDto>();
    public IEnumerable<ClientDto> Clients { get; set; } = new List<ClientDto>();
}

public class CountryDto
{
    public string Name { get; set; } = string.Empty;
}

public class ClientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// public class ClientWithTripDetailsDto
// {
//     public int IdClient { get; set; }
//     public int IdTrip { get; set; }
//     public DateTime RegisteredAt { get; set; }
//     public DateTime? PaymentDate { get; set; }
// }