using EnergyConsumTracker.App.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyConsumTracker.Repositorty
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IoTDevice> IoTDevices { get; set; }
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<EnergyConsumption> EnergyConsumptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure IoTDevice entity
            modelBuilder.Entity<IoTDevice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DeviceType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NominalWattageW).HasPrecision(10, 2);
                entity.Property(e => e.ActualPowerW).HasPrecision(10, 2);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Connectivity).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CumulativeEnergyKwh).HasPrecision(10, 4);
                entity.Property(e => e.LastReadingReason).HasMaxLength(500);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Create unique index on DeviceId
                entity.HasIndex(e => e.DeviceId).IsUnique();

                // Configure relationship with Consumer
                entity.HasOne(e => e.Consumer)
                      .WithMany()
                      .HasForeignKey(e => e.ConsumerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Consumer entity
            modelBuilder.Entity<Consumer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Create unique index on Email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure EnergyConsumption entity
            modelBuilder.Entity<EnergyConsumption>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PowerConsumptionW).HasPrecision(10, 2);
                entity.Property(e => e.EnergyConsumptionKwh).HasPrecision(10, 4);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Configure relationship with IoTDevice
                entity.HasOne<IoTDevice>()
                      .WithMany()
                      .HasForeignKey(e => e.DeviceId)
                      .HasPrincipalKey(e => e.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDateTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Seed Consumers
            modelBuilder.Entity<Consumer>().HasData(
                new Consumer
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Address = "123 Main Street, City, State 12345",
                    PhoneNumber = "+1-555-0123",
                    CreatedAt = seedDateTime,
                    UpdatedAt = seedDateTime
                },
                new Consumer
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Address = "456 Oak Avenue, City, State 12345",
                    PhoneNumber = "+1-555-0456",
                    CreatedAt = seedDateTime,
                    UpdatedAt = seedDateTime
                }
            );

            // Seed IoT Devices
            var sampleDevices = new[]
            {
                new { Id = 1, DeviceId = "LIV-AC-001", DeviceType = "Air Conditioner", Location = "Living Room 1", NominalWattageW = 1500m, ConsumerId = 1 },
                new { Id = 2, DeviceId = "BED-AC-001", DeviceType = "Air Conditioner", Location = "Bedroom 1", NominalWattageW = 1200m, ConsumerId = 1 },
                new { Id = 3, DeviceId = "KIT-REF-001", DeviceType = "Refrigerator", Location = "Kitchen 1", NominalWattageW = 150m, ConsumerId = 1 },
                new { Id = 4, DeviceId = "OFF-LED-001", DeviceType = "LED Tube Light", Location = "Office Cabin 1", NominalWattageW = 18m, ConsumerId = 1 },
                new { Id = 5, DeviceId = "HAL-LED-001", DeviceType = "LED Tube Light", Location = "Hallway 1", NominalWattageW = 18m, ConsumerId = 1 },
                new { Id = 6, DeviceId = "BED-TV-001", DeviceType = "Smart TV", Location = "Bedroom 1", NominalWattageW = 80m, ConsumerId = 1 },
                new { Id = 7, DeviceId = "LIV-TV-001", DeviceType = "Smart TV", Location = "Living Room 1", NominalWattageW = 100m, ConsumerId = 1 },
                new { Id = 8, DeviceId = "UTL-WAS-001", DeviceType = "Washing Machine", Location = "Utility Room 1", NominalWattageW = 2000m, ConsumerId = 1 },
                new { Id = 9, DeviceId = "OFF-DES-001", DeviceType = "Desktop Computer", Location = "Office Desk 1", NominalWattageW = 300m, ConsumerId = 1 },
                new { Id = 10, DeviceId = "STU-DES-001", DeviceType = "Desktop Computer", Location = "Study 1", NominalWattageW = 250m, ConsumerId = 1 }
            };

            foreach (var deviceInfo in sampleDevices)
            {
                modelBuilder.Entity<IoTDevice>().HasData(new IoTDevice
                {
                    Id = deviceInfo.Id,
                    DeviceId = deviceInfo.DeviceId,
                    DeviceType = deviceInfo.DeviceType,
                    Location = deviceInfo.Location,
                    NominalWattageW = deviceInfo.NominalWattageW,
                    ActualPowerW = 0,
                    Status = "OFF",
                    Connectivity = "Online",
                    DurationOnSeconds = 0,
                    CumulativeEnergyKwh = 0,
                    LastReadingReason = "Device Registered",
                    Timestamp = seedDateTime,
                    ConsumerId = deviceInfo.ConsumerId,
                    CreatedAt = seedDateTime,
                    UpdatedAt = seedDateTime
                });
            }
        }
    }
} 