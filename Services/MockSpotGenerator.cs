namespace ParkingProject.Services
{
    public class MockSpotGenerator
    {
        private readonly Random _random = new();
        private Dictionary<int, int> _currentFreeSpots = new();

        public int GetNextFreeSpots(int parkingLotId, int totalSpots)
        {
            if (!_currentFreeSpots.ContainsKey(parkingLotId))
            {
                // Initialisiere mit einem zufälligen, realistischen Wert
                _currentFreeSpots[parkingLotId] = totalSpots / 2;
            }

            int current = _currentFreeSpots[parkingLotId];
            
            // Simuliere leichte Fluktuation: +/- 5 Spots
            int change = _random.Next(-5, 6); 
            int next = current + change;

            // Stelle sicher, dass der Wert innerhalb der Grenzen liegt [0, totalSpots]
            next = Math.Max(0, Math.Min(totalSpots, next));

            _currentFreeSpots[parkingLotId] = next;
            return next;
        }
    }
}