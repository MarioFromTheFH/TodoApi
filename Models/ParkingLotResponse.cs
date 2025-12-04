namespace ParkingProject.Models
{
    public class ParkingLotResponse : ParkingLot
    {
        // Hier fügen wir den dynamischen Wert hinzu
        public int CurrentFreeSpots { get; set; } 
    }
}