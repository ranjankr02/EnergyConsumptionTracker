namespace EnergyConsumTracker.App.Models
{
    public class Consumer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<IoTDevice> IoTDevices { get; set; } = new List<IoTDevice>();
        public ICollection<EnergyConsumption> EnergyConsumptions { get; set; } = new List<EnergyConsumption>();
    }
}
