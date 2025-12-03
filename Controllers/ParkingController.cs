[ApiController]
[Route("[controller]")]

public class ParkingLotController : ControllerBase{
    [HttpGet]
    public string GetTestController(){
        return "Test";

    }

}