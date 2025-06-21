namespace EnergyConsumTracker.App.Models
{
    public class IoTDevice
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal NominalWattageW { get; set; }
        public decimal ActualPowerW { get; set; }
        public string Status { get; set; } = "OFF"; // ON, OFF
        public string Connectivity { get; set; } = "Online"; // Online, Offline
        public int DurationOnSeconds { get; set; }
        public decimal CumulativeEnergyKwh { get; set; }
        public string LastReadingReason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property to Consumer
        public int ConsumerId { get; set; }
        public Consumer? Consumer { get; set; }
    }
} 