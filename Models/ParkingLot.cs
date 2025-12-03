using System.ComponentModel.DataAnnotations; // Für Validierungen

namespace ParkingProject.Models
{
    public class ParkingLot
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public int TotalSpots { get; set; } // Die Gesamtkapazität ist Stammdaten
        
        // Geodaten
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Neue sinnvolle Felder für die DB:
        public string Address { get; set; } = string.Empty; // Adresse
        public bool IsUnderground { get; set; } // Tiefgarage?
        public decimal PricePerHour { get; set; } // Preis
    }
}