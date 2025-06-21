using EnergyConsumTracker.Services;
using EnergyConsumTracker.App.Models;

namespace EnergyConsumTracker
{
    /// <summary>
    /// Example class demonstrating how to use the CSV Reader Service
    /// </summary>
    public class CsvReaderExample
    {
        private readonly ICsvReaderService _csvReaderService;
        private readonly ILogger<CsvReaderExample> _logger;

        public CsvReaderExample(ICsvReaderService csvReaderService, ILogger<CsvReaderExample> logger)
        {
            _csvReaderService = csvReaderService;
            _logger = logger;
        }

        /// <summary>
        /// Example method showing how to read IoT devices from a CSV file
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <returns>List of IoT devices</returns>
        public async Task<List<IoTDevice>> ReadDevicesFromCsvExample(string filePath)
        {
            try
            {
                _logger.LogInformation($"Starting to read IoT devices from CSV file: {filePath}");
                
                // Read devices from CSV file
                var devices = await _csvReaderService.ReadIoTDevicesFromCsvAsync(filePath);
                
                _logger.LogInformation($"Successfully read {devices.Count} devices from CSV");
                
                // Example: Print some statistics
                var deviceTypes = devices.GroupBy(d => d.DeviceType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count);
                
                _logger.LogInformation("Device type distribution:");
                foreach (var deviceType in deviceTypes)
                {
                    _logger.LogInformation($"  {deviceType.Type}: {deviceType.Count} devices");
                }
                
                // Example: Find devices with high power consumption
                var highPowerDevices = devices.Where(d => d.ActualPowerW > 1000).ToList();
                _logger.LogInformation($"Found {highPowerDevices.Count} devices with power consumption > 1000W");
                
                return devices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading devices from CSV");
                throw;
            }
        }

        /// <summary>
        /// Example method showing how to read IoT devices from a CSV stream
        /// </summary>
        /// <param name="csvStream">CSV data stream</param>
        /// <returns>List of IoT devices</returns>
        public async Task<List<IoTDevice>> ReadDevicesFromStreamExample(Stream csvStream)
        {
            try
            {
                _logger.LogInformation("Starting to read IoT devices from CSV stream");
                
                // Read devices from CSV stream
                var devices = await _csvReaderService.ReadIoTDevicesFromCsvAsync(csvStream);
                
                _logger.LogInformation($"Successfully read {devices.Count} devices from CSV stream");
                
                return devices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading devices from CSV stream");
                throw;
            }
        }

        /// <summary>
        /// Example method showing how to process CSV data in batches
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <param name="batchSize">Number of devices to process in each batch</param>
        /// <returns>Total number of devices processed</returns>
        public async Task<int> ProcessCsvInBatchesExample(string filePath, int batchSize = 100)
        {
            try
            {
                _logger.LogInformation($"Processing CSV file in batches of {batchSize}: {filePath}");
                
                var allDevices = await _csvReaderService.ReadIoTDevicesFromCsvAsync(filePath);
                var totalDevices = allDevices.Count;
                var processedCount = 0;
                
                // Process devices in batches
                for (int i = 0; i < totalDevices; i += batchSize)
                {
                    var batch = allDevices.Skip(i).Take(batchSize).ToList();
                    await ProcessDeviceBatch(batch, i / batchSize + 1);
                    processedCount += batch.Count;
                }
                
                _logger.LogInformation($"Successfully processed {processedCount} devices in batches");
                return processedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CSV in batches");
                throw;
            }
        }

        private async Task ProcessDeviceBatch(List<IoTDevice> devices, int batchNumber)
        {
            _logger.LogInformation($"Processing batch {batchNumber} with {devices.Count} devices");
            
            // Example processing logic
            var onlineDevices = devices.Where(d => d.Connectivity == "Online").Count();
            var activeDevices = devices.Where(d => d.Status == "ON").Count();
            var totalPower = devices.Sum(d => d.ActualPowerW);
            
            _logger.LogInformation($"Batch {batchNumber} stats: Online={onlineDevices}, Active={activeDevices}, TotalPower={totalPower}W");
            
            // Simulate some processing time
            await Task.Delay(100);
        }
    }
} 