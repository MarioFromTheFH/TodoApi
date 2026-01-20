using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingProject.Data; // Annahme: Du hast ParkingLotDB nach Data/ verschoben
using ParkingProject.Models; // Annahme: Du hast ParkingLot nach Models/ verschoben
using ParkingProject.Services; // Einbindungen der Services Klasse für DI

using ParkingProject.Filters;

[Route("api/admin/[controller]")] // Administrative Route
[ApiController]
[LogToKafka]
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveParkingLot(int id){
        var success = await _parkingLotService.RemoveAsync(id);
        if (success==-1)
            {
                // Status 404 zurückgeben, wenn die ID nicht existiert
                return NotFound($"Parkplatz mit ID {id} konnte nicht gefunden werden.");
            }

            // Status 204 (Erfolgreich, aber keine Daten im Response-Body)
            return NoContent();
    }

    // PUT: api/ParkingLotCrud/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateParkingLotRequest request)
    {
        if (request == null) return BadRequest();

        var success = await _parkingLotService.UpdateParkingLotAsync(id, request.Name, request.TotalSpots);

        if (!success)
        {
            return NotFound($"Parkplatz mit ID {id} konnte nicht gefunden werden.");
        }

        // Bei PUT gibt man oft "204 No Content" oder das aktualisierte Objekt zurück.
        return NoContent(); 
    }    
        
}