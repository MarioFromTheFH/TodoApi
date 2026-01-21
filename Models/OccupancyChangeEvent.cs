public class OccupancyChangeEvent
{
    public int ParkingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FreeSpots { get; set; }
    public int TotalSpots { get; set; }
}