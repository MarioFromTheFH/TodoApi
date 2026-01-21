using Microsoft.AspNetCore.Mvc;
using ParkingProject.Data;
using ParkingProject.Models;
using ParkingProject.Services;
using ParkingProject.Filters;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using System.Text.Json;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[LogToKafka] // <--- Das hier loggt jetzt ALLES in diesem Controller automatisch!
public class ParkingController : ControllerBase
{
    private readonly ParkingLotDB _context;
    private readonly MockSpotGenerator _mockGenerator;
    private readonly IProducer<Null, string> _kafkaProducer;

    // Ändere HIER 'Ignore' zu 'Null':
    public ParkingController(ParkingLotDB context, MockSpotGenerator mockGen, IProducer<Null, string> kafka)
    {
        _context = context;
        _mockGenerator = mockGen;
        _kafkaProducer = kafka; // Jetzt passen die Typen zusammen!
    }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ParkingLotSummary>), 200)]
        public async Task<ActionResult<IEnumerable<ParkingLotSummary>>> GetAllParkingLotsSummary()
        {
            // Führt eine Projektion durch: Wählt nur die Felder Id und Name aus der DB.
            var parkingLotSummaries = await _context.ParkingLots
                .Select(p => new ParkingLotSummary
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();

            // Der LogToKafka-Filter protokolliert diesen erfolgreichen 200 OK Status automatisch.
            return Ok(parkingLotSummaries);
        }

    // 1) Parkplatzdaten komplett auslesen (mit Mock-Daten)
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ParkingLotResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ParkingLotResponse>> GetFullParkingLot(int id)
    {
        var lot = await _context.ParkingLots.FindAsync(id);
        
        if (lot == null) return NotFound($"Parkplatz mit ID {id} nicht gefunden.");

        // Mapping auf das DTO (Stammdaten + Live-Daten)
        var response = new ParkingLotResponse
        {
            Id = lot.Id,
            Name = lot.Name,
            TotalSpots = lot.TotalSpots,
            Latitude = lot.Latitude,
            Longitude = lot.Longitude,
            Address = lot.Address,
            IsUnderground = lot.IsUnderground,
            PricePerHour = lot.PricePerHour,
            
            // Hier kommt der Mock/ML Wert rein:
            CurrentFreeSpots = _mockGenerator.GetNextFreeSpots(lot.Id, lot.TotalSpots)
        };

        return Ok(response);
    }

    // 2) Nur freie Parkplätze auslesen
    [HttpGet("{id}/free")]
    public async Task<ActionResult<int>> GetFreeSpots(int id)
    {
        var lot = await _context.ParkingLots.FindAsync(id);
        if (lot == null) return NotFound();

        int free = _mockGenerator.GetNextFreeSpots(lot.Id, lot.TotalSpots);


    // 2. Event an Kafka senden

            var eventData = new { 
            parkingLotId = id, 
            currentFreeSpots = free,
            totalSpots = lot.TotalSpots
        };

        // 2. Sende es direkt an Kafka
        await _kafkaProducer.ProduceAsync("parking-occupancy-changed", new Message<Null, string> 
        { 
            Value = JsonSerializer.Serialize(eventData) 
        });
                        
        return Ok(free);
    }

    // 3) Parkplätze total auslesen
    [HttpGet("{id}/total")]
    public async Task<ActionResult<int>> GetTotalSpots(int id)
    {
        var lot = await _context.ParkingLots.FindAsync(id);
        if (lot == null) return NotFound();

        return Ok(lot.TotalSpots);
    }

    // 4) Geodaten auslesen
    [HttpGet("{id}/geo")]
    public async Task<ActionResult<object>> GetGeoLocation(int id)
    {
        var lot = await _context.ParkingLots.FindAsync(id);
        if (lot == null) return NotFound();

        // Anonymes Objekt zurückgeben
        return Ok(new { lat = lot.Latitude, lng = lot.Longitude });
    }

    // 5) Name über ID
    [HttpGet("{id}/name")]
    public async Task<ActionResult<string>> GetName(int id)
    {
        var lot = await _context.ParkingLots.FindAsync(id);
        if (lot == null) return NotFound();
        
        return Ok(lot.Name);
    }


    // ---------------------------------------------------------
    // ZUGRIFF ÜBER NAMEN
    // ---------------------------------------------------------

    // api/Parking/by-name/Hauptbahnhof
    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<ParkingLotResponse>> GetFullParkingLotByName(string name)
    {
        // Wir suchen den ersten Parkplatz, der diesen Namen hat
        var lot = await _context.ParkingLots.FirstOrDefaultAsync(p => p.Name == name);
        
        if (lot == null) return NotFound($"Parkplatz '{name}' nicht gefunden.");

        var response = new ParkingLotResponse
        {
            Id = lot.Id,
            Name = lot.Name,
            TotalSpots = lot.TotalSpots,
            Latitude = lot.Latitude,
            Longitude = lot.Longitude,
            Address = lot.Address, 
            CurrentFreeSpots = _mockGenerator.GetNextFreeSpots(lot.Id, lot.TotalSpots)
        };
        return Ok(response);
    }

    // api/Parking/by-name/Hauptbahnhof/free
    [HttpGet("by-name/{name}/free")]
    public async Task<ActionResult<int>> GetFreeSpotsByName(string name)
    {
        var lot = await _context.ParkingLots.FirstOrDefaultAsync(p => p.Name == name);
        if (lot == null) return NotFound();

        return Ok(_mockGenerator.GetNextFreeSpots(lot.Id, lot.TotalSpots));
    }

    // api/Parking/by-name/Hauptbahnhof/total
    [HttpGet("by-name/{name}/total")]
    public async Task<ActionResult<int>> GetTotalSpotsByName(string name)
    {
        var lot = await _context.ParkingLots.FirstOrDefaultAsync(p => p.Name == name);
        if (lot == null) return NotFound();
        return Ok(lot.TotalSpots);
    }

    // api/Parking/by-name/Hauptbahnhof/geo
    [HttpGet("by-name/{name}/geo")]
    public async Task<ActionResult<object>> GetGeoByName(string name)
    {
        var lot = await _context.ParkingLots.FirstOrDefaultAsync(p => p.Name == name);
        if (lot == null) return NotFound();
        return Ok(new { Lat = lot.Latitude, Lng = lot.Longitude });
    }
    
}