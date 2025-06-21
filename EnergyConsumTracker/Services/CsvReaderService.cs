using EnergyConsumTracker.App.Models;
using System.Globalization;

namespace EnergyConsumTracker.Services
{
    public interface ICsvReaderService
    {
        Task<List<IoTDevice>> ReadIoTDevicesFromCsvAsync(string filePath);
        Task<List<IoTDevice>> ReadIoTDevicesFromCsvAsync(Stream csvStream);
        List<IoTDevice> ReadIoTDevicesFromCsv(string filePath);
        List<IoTDevice> ReadIoTDevicesFromCsv(Stream csvStream);
    }

    public class CsvReaderService : ICsvReaderService
    {
        private readonly ILogger<CsvReaderService> _logger;

        public CsvReaderService(ILogger<CsvReaderService> logger)
        {
            _logger = logger;
        }

        public async Task<List<IoTDevice>> ReadIoTDevicesFromCsvAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"CSV file not found: {filePath}");
                    throw new FileNotFoundException($"CSV file not found: {filePath}");
                }

                using var stream = File.OpenRead(filePath);
                return await ReadIoTDevicesFromCsvAsync(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading CSV file from path: {filePath}");
                throw;
            }
        }

        public async Task<List<IoTDevice>> ReadIoTDevicesFromCsvAsync(Stream csvStream)
        {
            try
            {
                var devices = new List<IoTDevice>();
                using var reader = new StreamReader(csvStream);
                
                // Skip header line
                var headerLine = await reader.ReadLineAsync();
                if (headerLine == null)
                {
                    _logger.LogWarning("CSV file is empty or contains no data");
                    return devices;
                }

                var lineNumber = 1; // Start from 1 since we skipped header
                string? line;
                
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    try
                    {
                        var device = ParseCsvLine(line, lineNumber);
                        if (device != null)
                        {
                            devices.Add(device);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error parsing line {lineNumber}: {line}");
                        // Continue processing other lines
                    }
                }

                _logger.LogInformation($"Successfully read {devices.Count} IoT devices from CSV");
                return devices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading CSV stream");
                throw;
            }
        }

        public List<IoTDevice> ReadIoTDevicesFromCsv(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"CSV file not found: {filePath}");
                    throw new FileNotFoundException($"CSV file not found: {filePath}");
                }

                using var stream = File.OpenRead(filePath);
                return ReadIoTDevicesFromCsv(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading CSV file from path: {filePath}");
                throw;
            }
        }

        public List<IoTDevice> ReadIoTDevicesFromCsv(Stream csvStream)
        {
            try
            {
                var devices = new List<IoTDevice>();
                using var reader = new StreamReader(csvStream);
                
                // Skip header line
                var headerLine = reader.ReadLine();
                if (headerLine == null)
                {
                    _logger.LogWarning("CSV file is empty or contains no data");
                    return devices;
                }

                var lineNumber = 1; // Start from 1 since we skipped header
                string? line;
                
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    try
                    {
                        var device = ParseCsvLine(line, lineNumber);
                        if (device != null)
                        {
                            devices.Add(device);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error parsing line {lineNumber}: {line}");
                        // Continue processing other lines
                    }
                }

                _logger.LogInformation($"Successfully read {devices.Count} IoT devices from CSV");
                return devices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading CSV stream");
                throw;
            }
        }

        private IoTDevice? ParseCsvLine(string line, int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var columns = ParseCsvColumns(line);
            
            if (columns.Length < 11)
            {
                _logger.LogWarning($"Line {lineNumber} has insufficient columns. Expected 11, got {columns.Length}");
                return null;
            }

            try
            {
                var device = new IoTDevice
                {
                    Id = lineNumber, // Use line number as temporary ID   test
                    Name = columns[0]?.Trim() ?? "Unknown Device",
                    DeviceId = columns[1]?.Trim() ?? "",
                    DeviceType = columns[2]?.Trim() ?? "",
                    Location = columns[3]?.Trim() ?? "",
                    NominalWattageW = ParseDecimal(columns[4]),
                    ActualPowerW = ParseDecimal(columns[5]),
                    Status = columns[6]?.Trim() ?? "OFF",
                    Connectivity = columns[7]?.Trim() ?? "Offline",
                    DurationOnSeconds = ParseInt(columns[8]),
                    CumulativeEnergyKwh = ParseDecimal(columns[9]),
                    LastReadingReason = columns[10]?.Trim() ?? "CSV Import",
                    Timestamp = ParseDateTime(columns[0]),
                    ConsumerId = 1, // Default consumer ID
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                return device;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing device data from line {lineNumber}: {line}");
                return null;
            }
        }

        private string[] ParseCsvColumns(string line)
        {
            var columns = new List<string>();
            var currentColumn = "";
            var inQuotes = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    columns.Add(currentColumn);
                    currentColumn = "";
                }
                else
                {
                    currentColumn += c;
                }
            }
            
            // Add the last column
            columns.Add(currentColumn);
            
            return columns.ToArray();
        }

        private DateTime ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.UtcNow;

            // Try multiple date formats
            var formats = new[]
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-dd HH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "MM/dd/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm:ss"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }

            // Fallback to standard parsing
            if (DateTime.TryParse(value, out var fallbackResult))
            {
                return fallbackResult;
            }

            _logger.LogWarning($"Could not parse datetime: {value}, using current time");
            return DateTime.UtcNow;
        }

        private decimal ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            _logger.LogWarning($"Could not parse decimal: {value}, using 0");
            return 0;
        }

        private int ParseInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            _logger.LogWarning($"Could not parse integer: {value}, using 0");
            return 0;
        }
    }
} 