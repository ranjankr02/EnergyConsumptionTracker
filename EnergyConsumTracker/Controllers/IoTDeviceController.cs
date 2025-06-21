using Microsoft.AspNetCore.Mvc;
using EnergyConsumTracker.App.Models;
using EnergyConsumTracker.Services;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EnergyConsumTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IoTDeviceController : ControllerBase
    {
        private readonly IoTDataSimulationService _simulationService;
        private readonly ICsvReaderService _csvReaderService;
        private readonly ILogger<IoTDeviceController> _logger;

        public IoTDeviceController(
            IoTDataSimulationService simulationService, 
            ICsvReaderService csvReaderService,
            ILogger<IoTDeviceController> logger)
        {
            _simulationService = simulationService;
            _csvReaderService = csvReaderService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<IoTDeviceDto>> GetAllDevices()
        {
            var devices = _simulationService.GetDevices().Select(d => new IoTDeviceDto
            {
                DeviceId = d.DeviceId,
                DeviceType = d.DeviceType,
                Location = d.Location,
                NominalWattageW = d.NominalWattageW,
                ActualPowerW = d.ActualPowerW,
                Status = d.Status,
                Connectivity = d.Connectivity,
                DurationOnSeconds = d.DurationOnSeconds,
                CumulativeEnergyKwh = d.CumulativeEnergyKwh,
                LastReadingReason = d.LastReadingReason,
                Timestamp = d.Timestamp,
                ConsumerId = d.ConsumerId
            });

            return Ok(devices);
        }

        [HttpGet("{deviceId}")]
        public ActionResult<IoTDeviceDto> GetDevice(string deviceId)
        {
            var device = _simulationService.GetDevice(deviceId);
            if (device == null)
                return NotFound();

            var deviceDto = new IoTDeviceDto
            {
                DeviceId = device.DeviceId,
                DeviceType = device.DeviceType,
                Location = device.Location,
                NominalWattageW = device.NominalWattageW,
                ActualPowerW = device.ActualPowerW,
                Status = device.Status,
                Connectivity = device.Connectivity,
                DurationOnSeconds = device.DurationOnSeconds,
                CumulativeEnergyKwh = device.CumulativeEnergyKwh,
                LastReadingReason = device.LastReadingReason,
                Timestamp = device.Timestamp,
                ConsumerId = device.ConsumerId
            };

            return Ok(deviceDto);
        }

        [HttpPost]
        public ActionResult<IoTDeviceDto> CreateDevice(IoTDeviceCreateDto createDto)
        {
            // For now, we'll return a success response but the device won't be added to the simulation
            // In a real implementation, you would add the device to the simulation service
            var deviceDto = new IoTDeviceDto
            {
                DeviceId = createDto.DeviceId,
                DeviceType = createDto.DeviceType,
                Location = createDto.Location,
                NominalWattageW = createDto.NominalWattageW,
                ActualPowerW = 0,
                Status = "OFF",
                Connectivity = "Online",
                DurationOnSeconds = 0,
                CumulativeEnergyKwh = 0,
                LastReadingReason = "Device Registered",
                Timestamp = DateTime.UtcNow,
                ConsumerId = createDto.ConsumerId
            };

            _logger.LogInformation($"Device creation requested: {createDto.DeviceId}");

            return CreatedAtAction(nameof(GetDevice), new { deviceId = device.DeviceId }, deviceDto);
        }

        [HttpPut("{deviceId}")]
        public ActionResult<IoTDeviceDto> UpdateDevice(string deviceId, IoTDeviceUpdateDto updateDto)
        {
            _simulationService.UpdateDevice(deviceId, updateDto);

            var device = _simulationService.GetDevice(deviceId);
            if (device == null)
                return NotFound();

            var deviceDto = new IoTDeviceDto
            {
                DeviceId = device.DeviceId,
                DeviceType = device.DeviceType,
                Location = device.Location,
                NominalWattageW = device.NominalWattageW,
                ActualPowerW = device.ActualPowerW,
                Status = device.Status,
                Connectivity = device.Connectivity,
                DurationOnSeconds = device.DurationOnSeconds,
                CumulativeEnergyKwh = device.CumulativeEnergyKwh,
                LastReadingReason = device.LastReadingReason,
                Timestamp = device.Timestamp,
                ConsumerId = device.ConsumerId
            };

            return Ok(deviceDto);
        }

        [HttpDelete("{deviceId}")]
        public ActionResult DeleteDevice(string deviceId)
        {
            // For now, we'll return a success response
            // In a real implementation, you would remove the device from the simulation service
            _logger.LogInformation($"Device deletion requested: {deviceId}");
            return NoContent();
        }

        [HttpGet("consumer/{consumerId}")]
        public ActionResult<IEnumerable<IoTDeviceDto>> GetDevicesByConsumer(int consumerId)
        {
            var devices = _simulationService.GetDevices()
                .Where(d => d.ConsumerId == consumerId)
                .Select(d => new IoTDeviceDto
                {
                    DeviceId = d.DeviceId,
                    DeviceType = d.DeviceType,
                    Location = d.Location,
                    NominalWattageW = d.NominalWattageW,
                    ActualPowerW = d.ActualPowerW,
                    Status = d.Status,
                    Connectivity = d.Connectivity,
                    DurationOnSeconds = d.DurationOnSeconds,
                    CumulativeEnergyKwh = d.CumulativeEnergyKwh,
                    LastReadingReason = d.LastReadingReason,
                    Timestamp = d.Timestamp,
                    ConsumerId = d.ConsumerId
                });

            return Ok(devices);
        }

        [HttpGet("status/online")]
        public ActionResult<IEnumerable<IoTDeviceDto>> GetOnlineDevices()
        {
            var devices = _simulationService.GetDevices()
                .Where(d => d.Connectivity == "Online")
                .Select(d => new IoTDeviceDto
                {
                    DeviceId = d.DeviceId,
                    DeviceType = d.DeviceType,
                    Location = d.Location,
                    NominalWattageW = d.NominalWattageW,
                    ActualPowerW = d.ActualPowerW,
                    Status = d.Status,
                    Connectivity = d.Connectivity,
                    DurationOnSeconds = d.DurationOnSeconds,
                    CumulativeEnergyKwh = d.CumulativeEnergyKwh,
                    LastReadingReason = d.LastReadingReason,
                    Timestamp = d.Timestamp,
                    ConsumerId = d.ConsumerId
                });

            return Ok(devices);
        }

        [HttpGet("status/active")]
        public ActionResult<IEnumerable<IoTDeviceDto>> GetActiveDevices()
        {
            var devices = _simulationService.GetDevices()
                .Where(d => d.Status == "ON" && d.Connectivity == "Online")
                .Select(d => new IoTDeviceDto
                {
                    DeviceId = d.DeviceId,
                    DeviceType = d.DeviceType,
                    Location = d.Location,
                    NominalWattageW = d.NominalWattageW,
                    ActualPowerW = d.ActualPowerW,
                    Status = d.Status,
                    Connectivity = d.Connectivity,
                    DurationOnSeconds = d.DurationOnSeconds,
                    CumulativeEnergyKwh = d.CumulativeEnergyKwh,
                    LastReadingReason = d.LastReadingReason,
                    Timestamp = d.Timestamp,
                    ConsumerId = d.ConsumerId
                });

            return Ok(devices);
        }

        [HttpGet("analytics/summary")]
        public ActionResult<object> GetAnalyticsSummary()
        {
            var devices = _simulationService.GetDevices();
            
            var summary = new
            {
                TotalDevices = devices.Count,
                OnlineDevices = devices.Count(d => d.Connectivity == "Online"),
                ActiveDevices = devices.Count(d => d.Status == "ON" && d.Connectivity == "Online"),
                TotalPowerConsumption = devices.Sum(d => d.ActualPowerW),
                TotalEnergyConsumption = devices.Sum(d => d.CumulativeEnergyKwh),
                DevicesByType = devices.GroupBy(d => d.DeviceType)
                    .Select(g => new { DeviceType = g.Key, Count = g.Count() }),
                DevicesByLocation = devices.GroupBy(d => d.Location)
                    .Select(g => new { Location = g.Key, Count = g.Count() })
            };

            return Ok(summary);
        }

        [HttpPost("import/csv")]
        public async Task<ActionResult<object>> ImportDevicesFromCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("File must be a CSV file");
                }

                using var stream = file.OpenReadStream();
                var devices = await _csvReaderService.ReadIoTDevicesFromCsvAsync(stream);

                var result = new
                {
                    Message = "CSV import completed successfully",
                    TotalDevices = devices.Count,
                    Devices = devices.Select(d => new IoTDeviceDto
                    {
                        DeviceId = d.DeviceId,
                        DeviceType = d.DeviceType,
                        Location = d.Location,
                        NominalWattageW = d.NominalWattageW,
                        ActualPowerW = d.ActualPowerW,
                        Status = d.Status,
                        Connectivity = d.Connectivity,
                        DurationOnSeconds = d.DurationOnSeconds,
                        CumulativeEnergyKwh = d.CumulativeEnergyKwh,
                        LastReadingReason = d.LastReadingReason,
                        Timestamp = d.Timestamp,
                        ConsumerId = d.ConsumerId
                    }).ToList()
                };

                _logger.LogInformation($"Successfully imported {devices.Count} devices from CSV file: {file.FileName}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing devices from CSV");
                return StatusCode(500, new { Error = "Error importing devices from CSV", Details = ex.Message });
            }
        }

        [HttpPost("import/csv/path")]
        public async Task<ActionResult<object>> ImportDevicesFromCsvPath([FromBody] CsvImportRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FilePath))
                {
                    return BadRequest("File path is required");
                }

                var devices = await _csvReaderService.ReadIoTDevicesFromCsvAsync(request.FilePath);

                var result = new
                {
                    Message = "CSV import completed successfully",
                    FilePath = request.FilePath,
                    TotalDevices = devices.Count,
                    Devices = devices.Select(d => new IoTDeviceDto
                    {
                        DeviceId = d.DeviceId,
                        DeviceType = d.DeviceType,
                        Location = d.Location,
                        NominalWattageW = d.NominalWattageW,
                        ActualPowerW = d.ActualPowerW,
                        Status = d.Status,
                        Connectivity = d.Connectivity,
                        DurationOnSeconds = d.DurationOnSeconds,
                        CumulativeEnergyKwh = d.CumulativeEnergyKwh,
                        LastReadingReason = d.LastReadingReason,
                        Timestamp = d.Timestamp,
                        ConsumerId = d.ConsumerId
                    }).ToList()
                };

                _logger.LogInformation($"Successfully imported {devices.Count} devices from CSV file: {request.FilePath}");

                return Ok(result);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "CSV file not found");
                return NotFound(new { Error = "CSV file not found", FilePath = request.FilePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing devices from CSV");
                return StatusCode(500, new { Error = "Error importing devices from CSV", Details = ex.Message });
            }
        }
    }

    public class CsvImportRequest
    {
        public string FilePath { get; set; } = string.Empty;
    }
} 