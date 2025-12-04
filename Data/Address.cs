namespace ParkingProject.Models
{
    public class Address
    {
        public int Id { get; set; } // Eigener Primary Key
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        
        // Navigation zurück (optional, aber gut für EF)
        public int ParkingLotId { get; set; }
        public ParkingLot? ParkingLot { get; set; }
    }
}