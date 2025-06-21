namespace EnergyConsumTracker.App.Models
{
    public class EnergyConsumption
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public int IoTDeviceId { get; set; }
        public decimal PowerConsumptionW { get; set; }
        public decimal EnergyConsumptionKwh { get; set; }
        public DateTime ReadingTime { get; set; }
        public string DeviceStatus { get; set; } = string.Empty;
        public string DeviceConnectivity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Consumer? Consumer { get; set; }
        public IoTDevice? IoTDevice { get; set; }
    }
} 