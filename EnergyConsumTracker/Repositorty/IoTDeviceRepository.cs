using EnergyConsumTracker.App.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnergyConsumTracker.Repositorty
{
    public interface IIoTDeviceRepository
    {
        Task<IEnumerable<IoTDevice>> GetAllAsync();
        Task<IoTDevice?> GetByIdAsync(string deviceId);
        Task<IEnumerable<IoTDevice>> GetByConsumerIdAsync(int consumerId);
        Task<IoTDevice> AddAsync(IoTDevice device);
        Task<IoTDevice> UpdateAsync(IoTDevice device);
        Task<bool> DeleteAsync(string deviceId);
        Task<IEnumerable<IoTDevice>> GetOnlineDevicesAsync();
        Task<IEnumerable<IoTDevice>> GetActiveDevicesAsync();
    }

    public class IoTDeviceRepository : IIoTDeviceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IoTDeviceRepository> _logger;

        public IoTDeviceRepository(ApplicationDbContext context, ILogger<IoTDeviceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<IoTDevice>> GetAllAsync()
        {
            return await _context.IoTDevices
                .Include(d => d.Consumer)
                .ToListAsync();
        }

        public async Task<IoTDevice?> GetByIdAsync(string deviceId)
        {
            return await _context.IoTDevices
                .Include(d => d.Consumer)
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        public async Task<IEnumerable<IoTDevice>> GetByConsumerIdAsync(int consumerId)
        {
            return await _context.IoTDevices
                .Include(d => d.Consumer)
                .Where(d => d.ConsumerId == consumerId)
                .ToListAsync();
        }

        public async Task<IoTDevice> AddAsync(IoTDevice device)
        {
            _context.IoTDevices.Add(device);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Added new IoT device: {device.DeviceId}");
            return device;
        }

        public async Task<IoTDevice> UpdateAsync(IoTDevice device)
        {
            device.UpdatedAt = DateTime.UtcNow;
            _context.IoTDevices.Update(device);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated IoT device: {device.DeviceId}");
            return device;
        }

        public async Task<bool> DeleteAsync(string deviceId)
        {
            var device = await _context.IoTDevices.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
            if (device == null)
                return false;

            _context.IoTDevices.Remove(device);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted IoT device: {deviceId}");
            return true;
        }

        public async Task<IEnumerable<IoTDevice>> GetOnlineDevicesAsync()
        {
            return await _context.IoTDevices
                .Include(d => d.Consumer)
                .Where(d => d.Connectivity == "Online")
                .ToListAsync();
        }

        public async Task<IEnumerable<IoTDevice>> GetActiveDevicesAsync()
        {
            return await _context.IoTDevices
                .Include(d => d.Consumer)
                .Where(d => d.Status == "ON" && d.Connectivity == "Online")
                .ToListAsync();
        }
    }
} 