using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingProject.Data; // Annahme: Du hast ParkingLotDB nach Data/ verschoben
using ParkingProject.Models; // Annahme: Du hast ParkingLot nach Models/ verschoben

[Route("api/admin/[controller]")] // Administrative Route
[ApiController]
public class ParkingLotCrudController : ControllerBase
{
    private readonly ParkingLotDB _context;

    public ParkingLotCrudController(ParkingLotDB context)
    {
        _context = context;
    }

    // POST: api/admin/ParkingLotCrud
    [HttpPost]
    public async Task<ActionResult<ParkingLot>> PostParkingLot(ParkingLot parkingLot)
    {
        _context.ParkingLots.Add(parkingLot);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetParkingLot), new { id = parkingLot.Id }, parkingLot);
    }
    
    // GET: api/admin/ParkingLotCrud/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingLot>> GetParkingLot(int id)
    {
        var parkingLot = await _context.ParkingLots.FindAsync(id);

        if (parkingLot == null)
        {
            return NotFound();
        }

        return parkingLot;
    }
    
    // ... Hier würden weitere Methoden wie Put und Delete folgen ...
}