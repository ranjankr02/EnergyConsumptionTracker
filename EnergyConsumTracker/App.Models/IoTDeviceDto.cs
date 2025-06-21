namespace EnergyConsumTracker.App.Models
{
    public class IoTDeviceDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal NominalWattageW { get; set; }
        public decimal ActualPowerW { get; set; }
        public string Status { get; set; } = "OFF";
        public string Connectivity { get; set; } = "Online";
        public int DurationOnSeconds { get; set; }
        public decimal CumulativeEnergyKwh { get; set; }
        public string LastReadingReason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int ConsumerId { get; set; }
    }

    public class IoTDeviceCreateDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal NominalWattageW { get; set; }
        public int ConsumerId { get; set; }
    }

    public class IoTDeviceUpdateDto
    {
        public decimal ActualPowerW { get; set; }
        public string Status { get; set; } = "OFF";
        public string Connectivity { get; set; } = "Online";
        public int DurationOnSeconds { get; set; }
        public decimal CumulativeEnergyKwh { get; set; }
        public string LastReadingReason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
} 