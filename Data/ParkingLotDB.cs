using Microsoft.EntityFrameworkCore;
using ParkingProject.Models;

namespace ParkingProject.Data{
public class ParkingLotDB : DbContext
{
    public ParkingLotDB(DbContextOptions<ParkingLotDB> options)
        : base(options) { }

    public DbSet<ParkingLot> ParkingLots => Set<ParkingLot>();
}
}