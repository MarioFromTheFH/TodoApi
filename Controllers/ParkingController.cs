using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingProject;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParkingController : ControllerBase
{
    private readonly ParkingLotDB _context;

    public ParkingController(ParkingLotDB context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult getParking()
    {            
        return Ok("Test");
    }
}