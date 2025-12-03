namespace ParkingProject{

public class ParkingLot{
    public int Id {get; set;}
    public string? Name {get; set;}
    public int totalSpots {get; set;}
    public int freeSpots {get; set;}

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
}