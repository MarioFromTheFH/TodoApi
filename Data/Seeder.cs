namespace ParkingProject.Data
{
    public static class Seeder
    {
        public static void SeedInitialData(ParkingLotDB context)
        {
            if (!context.ParkingLots.Any())
            {
                context.ParkingLots.AddRange(
                    new ParkingLot { 
                        Name = "Bahnhof Gratwein-Gratkorn", totalSpots = 150, freeSpots = 100,
                        Latitude = 47.12886524027774, Longitude = 15.323127554597933 
                    },
                    new ParkingLot { 
                        Name = "Bahnhof Judendorf", totalSpots = 80, freeSpots = 50,
                        Latitude = 47.118612439565815, Longitude = 15.34287170575213 
                    }
                );
                context.SaveChanges();
            }
        }
    }
}