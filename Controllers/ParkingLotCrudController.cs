using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingProject.Data; // Annahme: Du hast ParkingLotDB nach Data/ verschoben
using ParkingProject.Models; // Annahme: Du hast ParkingLot nach Models/ verschoben
using ParkingProject.Services; // Einbindungen der Services Klasse für DI

[Route("api/admin/[controller]")] // Administrative Route
[ApiController]
public class ParkingLotCrudController : ControllerBase
{
    private readonly IParkingLotService _parkingLotService;

    public ParkingLotCrudController(IParkingLotService parkingLotService)
    {
        _parkingLotService = parkingLotService;
    }

    // POST: api/admin/ParkingLotCrud
    [HttpPost]
    public async Task<ActionResult<ParkingLot>> PostParkingLot(ParkingLot parkingLot)
    {
        // Aufruf der Business-Logik im Service
        var created = await _parkingLotService.CreateAsync(parkingLot);

        // HTTP 201 Created mit Location-Header
        return CreatedAtAction(nameof(GetParkingLot), new { id = created.Id }, created);
    }
    
    // GET: api/admin/ParkingLotCrud/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingLot>> GetParkingLot(int id)
    {
        // Parkplatz über den Service abrufen
        var parkingLot = await _parkingLotService.GetByIdAsync(id);

        // Falls nicht gefunden → HTTP 404
        if (parkingLot == null)
        {
            return NotFound();
        }

        // Erfolgreiche Rückgabe → HTTP 200
        return parkingLot;
    }
    
    // ... Hier würden weitere Methoden wie Put und Delete folgen ...
}