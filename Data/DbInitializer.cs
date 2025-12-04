using ParkingProject.Models;

namespace ParkingProject.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ParkingLotDB context)
        {
            // 1. Stellt sicher, dass die DB existiert (wendet Migrationen an)
            context.Database.EnsureCreated();

            // 2. Prüfen, ob schon Parkplätze da sind
            if (context.ParkingLots.Any())
            {
                return;   // DB wurde bereits befüllt
            }

            // 3. Testdaten anlegen
            var parkingLots = new ParkingLot[]
            {
                new ParkingLot { 
                    Name = "Bahnhof Gratwein-Gratkorn", TotalSpots = 150,
                        Latitude = 47.12886524027774, Longitude = 15.323127554597933,                    
                    IsUnderground = true, 
                    PricePerHour = 3.50m 
                },
                new ParkingLot { 
                    Name = "Bahnhof Judendorf", TotalSpots = 80, 
                        Latitude = 47.118612439565815, Longitude = 15.34287170575213,
                    IsUnderground = false, 
                    PricePerHour = 2.00m 
                }
            };

            context.ParkingLots.AddRange(parkingLots);
            context.SaveChanges();
        }
    }
}