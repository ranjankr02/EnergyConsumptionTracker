using EnergyConsumTracker.App.Models;

namespace EnergyConsumTracker.Services
{
    public interface IIoTDataSimulationService
    {
        List<IoTDevice> GetDevices();
        IoTDevice? GetDevice(string deviceId);
        void UpdateDevice(string deviceId, IoTDeviceUpdateDto updateDto);
        void StartSimulation();
        void StopSimulation();
        bool IsSimulationRunning { get; }
    }
} 