namespace ParkingProject.Models
{
    public class ApiAccessLog
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Action { get; set; } = string.Empty; // z.B. "GetFreeSpots"
        public int? ParkingLotId { get; set; } // Welcher Parkplatz?
        public string UserAgent { get; set; } = string.Empty; // Wer fragt an?
        public string ResultStatus { get; set; } = string.Empty; // "Success", "NotFound"
        public string? ResultValue { get; set; } = string.Empty;// z.B. "15 freie Plätze"
    }
}