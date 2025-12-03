using Microsoft.EntityFrameworkCore;

namespace ParkingProject{
class ParkingLotDB : DbContext
{
    public ParkingLotDB(DbContextOptions<ParkingLotDB> options)
        : base(options) { }

    public DbSet<ParkingLot> ParkingLots => Set<ParkingLot>();
}
}