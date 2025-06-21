using EnergyConsumTracker.App.Models;
using System.Timers;

namespace EnergyConsumTracker.Services
{
    public class IoTDataSimulationService : IHostedService, IDisposable
    {
      //  private Timer? _timer;
        private readonly ILogger<IoTDataSimulationService> _logger;
        private readonly Random _random = new Random();
        private readonly List<IoTDevice> _devices = new List<IoTDevice>();
        private readonly object _lockObject = new object();

        public IoTDataSimulationService(ILogger<IoTDataSimulationService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("IoT Data Simulation Service is starting.");

            // Initialize sample devices
            InitializeSampleDevices();

            // Start timer to simulate data updates every 30 seconds
            _timer = new Timer(30000); // 30 seconds
            _timer.Elapsed += SimulateDeviceData;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("IoT Data Simulation Service is stopping.");

            _timer?.Stop();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public List<IoTDevice> GetDevices()
        {
            lock (_lockObject)
            {
                return _devices.ToList();
            }
        }

        public IoTDevice? GetDevice(string deviceId)
        {
            lock (_lockObject)
            {
                return _devices.FirstOrDefault(d => d.DeviceId == deviceId);
            }
        }

        public void UpdateDevice(string deviceId, IoTDeviceUpdateDto updateDto)
        {
            lock (_lockObject)
            {
                var device = _devices.FirstOrDefault(d => d.DeviceId == deviceId);
                if (device != null)
                {
                    device.ActualPowerW = updateDto.ActualPowerW;
                    device.Status = updateDto.Status;
                    device.Connectivity = updateDto.Connectivity;
                    device.DurationOnSeconds = updateDto.DurationOnSeconds;
                    device.CumulativeEnergyKwh = updateDto.CumulativeEnergyKwh;
                    device.LastReadingReason = updateDto.LastReadingReason;
                    device.Timestamp = updateDto.Timestamp;
                    device.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        private void SimulateDeviceData(object? sender, ElapsedEventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    foreach (var device in _devices)
                    {
                        SimulateDeviceUpdate(device);
                    }
                }

                _logger.LogInformation($"Simulated data update for {_devices.Count} devices at {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during IoT data simulation");
            }
        }

        private void SimulateDeviceUpdate(IoTDevice device)
        {
            var currentTime = DateTime.UtcNow;

            // Simulate status change (10% chance)
            if (_random.NextDouble() < 0.1)
            {
                device.Status = device.Status == "ON" ? "OFF" : "ON";
                device.LastReadingReason = $"Status Change: {device.Status}";
            }

            // Simulate connectivity change (5% chance)
            if (_random.NextDouble() < 0.05)
            {
                device.Connectivity = device.Connectivity == "Online" ? "Offline" : "Online";
                device.LastReadingReason = $"Connectivity Change: {device.Connectivity}";
            }

            // Update power consumption based on status and connectivity
            if (device.Connectivity == "Online" && device.Status == "ON")
            {
                // Simulate slight wattage variation (±5%)
                var variation = _random.NextDouble() * 0.1 - 0.05; // -5% to +5%
                device.ActualPowerW = Math.Round(device.NominalWattageW * (1 + (decimal)variation), 2);
                
                // Update duration and cumulative energy
                device.DurationOnSeconds += 30; // 30 seconds since last update
                var energyIncrement = (device.ActualPowerW * 30) / (3600 * 1000); // Convert to kWh
                device.CumulativeEnergyKwh += energyIncrement;
            }
            else
            {
                device.ActualPowerW = 0;
                if (device.Status == "OFF")
                {
                    device.DurationOnSeconds = 0;
                }
            }

            device.Timestamp = currentTime;
            device.UpdatedAt = currentTime;

            if (string.IsNullOrEmpty(device.LastReadingReason))
            {
                device.LastReadingReason = "Regular Interval";
            }
        }

        private void InitializeSampleDevices()
        {
            var sampleDevices = new[]
            {
                new { DeviceId = "LIV-AC-001", DeviceType = "Air Conditioner", Location = "Living Room 1", NominalWattageW = 1500m, ConsumerId = 1 },
                new { DeviceId = "BED-AC-001", DeviceType = "Air Conditioner", Location = "Bedroom 1", NominalWattageW = 1200m, ConsumerId = 1 },
                new { DeviceId = "KIT-REF-001", DeviceType = "Refrigerator", Location = "Kitchen 1", NominalWattageW = 150m, ConsumerId = 1 },
                new { DeviceId = "OFF-LED-001", DeviceType = "LED Tube Light", Location = "Office Cabin 1", NominalWattageW = 18m, ConsumerId = 1 },
                new { DeviceId = "HAL-LED-001", DeviceType = "LED Tube Light", Location = "Hallway 1", NominalWattageW = 18m, ConsumerId = 1 },
                new { DeviceId = "BED-TV-001", DeviceType = "Smart TV", Location = "Bedroom 1", NominalWattageW = 80m, ConsumerId = 1 },
                new { DeviceId = "LIV-TV-001", DeviceType = "Smart TV", Location = "Living Room 1", NominalWattageW = 100m, ConsumerId = 1 },
                new { DeviceId = "UTL-WAS-001", DeviceType = "Washing Machine", Location = "Utility Room 1", NominalWattageW = 2000m, ConsumerId = 1 },
                new { DeviceId = "OFF-DES-001", DeviceType = "Desktop Computer", Location = "Office Desk 1", NominalWattageW = 300m, ConsumerId = 1 },
                new { DeviceId = "STU-DES-001", DeviceType = "Desktop Computer", Location = "Study 1", NominalWattageW = 250m, ConsumerId = 1 }
            };

            int id = 1;
            foreach (var deviceInfo in sampleDevices)
            {
                var device = new IoTDevice
                {
                    Id = id++,
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
                    Timestamp = DateTime.UtcNow,
                    ConsumerId = deviceInfo.ConsumerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _devices.Add(device);
            }

            _logger.LogInformation($"Initialized {_devices.Count} sample IoT devices");
        }
    }
} 