using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingProject.Data; 
using ParkingProject.Services; 
using ParkingProject.Models; 

[Route("api/[controller]")]
[ApiController]
public class ParkingController : ControllerBase
{
    private readonly ParkingLotDB _context;
    private readonly MockSpotGenerator _mockGenerator;
    // ... andere Services (z.B. Kafka)

    public ParkingController(ParkingLotDB context, MockSpotGenerator mockGenerator)
    {
        _context = context;
        _mockGenerator = mockGenerator;
    }

    // 1. Parkplatzdaten komplett auslesen (Über ID)
    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingLot>> GetParkingLot(int id)
    {
        // Kafka Protokollierung (Platzhalter)
        
        var parkingLot = await _context.ParkingLots.FindAsync(id);
        if (parkingLot == null) return NotFound();
        
        // Simuliere die ML-Auslesung des Wertes
        parkingLot.freeSpots = _mockGenerator.GetNextFreeSpots(parkingLot.Id, parkingLot.totalSpots);
        
        return parkingLot;
    }
    
    // 2. Freie Parkplätze auslesen
    [HttpGet("{id}/free")]
    public async Task<ActionResult<int>> GetFreeSpots(int id)
    {
        var parkingLot = await _context.ParkingLots.FindAsync(id);
        if (parkingLot == null) return NotFound();

        // Kafka Protokollierung
        
        return _mockGenerator.GetNextFreeSpots(parkingLot.Id, parkingLot.totalSpots);
    }
    
    // ... Weitere Methoden (Total, Geo, Name über ID) folgen analog ...
}