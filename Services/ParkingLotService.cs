using ParkingProject.Data;
using ParkingProject.Models;
using Microsoft.Extensions.Configuration; //Für Aufgabe 9 benötigt

namespace ParkingProject.Services
{
    /// <summary>
    /// Service-Klasse, die die Geschäftslogik für Parkplätze kapselt.
    /// Hier findet der Zugriff auf die Datenbank statt.
    /// </summary>
    public class ParkingLotService : IParkingLotService
    {
        // DbContext für den Zugriff auf die PostgreSQL-Datenbank
        private readonly ParkingLotDB _context;

        // Aufgabe 9 Logprüfung
        private readonly bool _enableCreateLogging;

        /// <summary>
        /// Der DbContext wird per Dependency Injection bereitgestellt.
        /// </summary>
        public ParkingLotService(ParkingLotDB context, IConfiguration configuration)
        {
            _context = context;

            // Wert aus appsettings.json lesen
            _enableCreateLogging = configuration.GetValue<bool>(
                "ParkingSettings:EnableCreateLogging");
        }

        /// <summary>
        /// Sucht einen Parkplatz anhand der ID.
        /// Gibt null zurück, wenn kein Parkplatz gefunden wurde.
        /// </summary>
        public async Task<ParkingLot?> GetByIdAsync(int id)
        {
            // Direkter Zugriff auf das DbSet ParkingLots
            return await _context.ParkingLots.FindAsync(id);
        }

        /// <summary>
        /// Speichert einen neuen Parkplatz in der Datenbank.
        /// </summary>
        public async Task<ParkingLot> CreateAsync(ParkingLot parkingLot)
        {
            // Parkplatz zur Datenbank hinzufügen
            _context.ParkingLots.Add(parkingLot);

            // Änderungen dauerhaft speichern
            await _context.SaveChangesAsync();

            // Verhalten abhängig von appsettings.json
            if (_enableCreateLogging)
            {
                Console.WriteLine(
                    $"ParkingLot '{parkingLot.Name}' wurde erstellt (ID={parkingLot.Id})");
            }

            // Das gespeicherte Objekt zurückgeben (inkl. generierter ID)
            return parkingLot;
        }

        /// <summary>
        /// Löscht einen Parkplatz aus der Datenbank.
        /// </summary>
        public async Task<int> RemoveAsync(int id)
        {
            // Direkter Zugriff auf das DbSet ParkingLots
            var parkingLot = await _context.ParkingLots.FindAsync(id);
            if(parkingLot == null){
                // Verhalten abhängig von appsettings.json
                if (_enableCreateLogging)
                {
                    Console.WriteLine(
                        $"ParkingLot '{id}' existiert nicht");
                }
                return -1;
            }

            // Parkplatz aus Datenbank entfernen
            _context.ParkingLots.Remove(parkingLot);

            // Änderungen dauerhaft speichern
            await _context.SaveChangesAsync();

            // Verhalten abhängig von appsettings.json
            if (_enableCreateLogging)
            {
                Console.WriteLine(
                    $"ParkingLot '{id}' wurde entfernt");
            }

            return id;
        }            
    }
}
