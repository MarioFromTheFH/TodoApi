using ParkingProject.Models;

namespace ParkingProject.Services
{
    /// <summary>
    /// Interface für die Business-Logik rund um Parkplätze.
    /// Der Controller kennt nur dieses Interface, nicht die Implementierung.
    /// </summary>
    public interface IParkingLotService
    {
        /// <summary>
        /// Liefert einen Parkplatz anhand seiner ID.
        /// </summary>
        Task<ParkingLot?> GetByIdAsync(int id);

        /// <summary>
        /// Legt einen neuen Parkplatz in der Datenbank an.
        /// </summary>
        Task<ParkingLot> CreateAsync(ParkingLot parkingLot);
    }
}
