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
        // Navigation properties can be added here if needed
        // public ICollection<EnergyConsumption> EnergyConsumptions { get; set; }
    }
}
