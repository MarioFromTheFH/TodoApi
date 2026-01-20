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

        /// <summary>
        /// Löscht einen Parkplatz
        /// </summary>
        /// <param name="id">Die ID des zu löschenden Parkplatzes</param>
        /// <returns>id, wenn gelöscht; -1, wenn nicht gefunden.</returns>
        Task<int> RemoveAsync(int id);

        /// <summary>
        /// Update von Namen und Anzahl der Kapazität
        /// </summary>
        /// <param name="id">Die ID des zu ändernden Parkplatzes</param>
        /// <param name="newName">Der neue Name des Parkplatzes</param>
        /// <param name="newTotalSpots">Die neue Anzahl der Kapazität</param>
        /// <returns>True: Erfolg, False: Scheitern</returns>
        Task<bool> UpdateParkingLotAsync(int id, string newName, int newTotalSpots);
    }
}
