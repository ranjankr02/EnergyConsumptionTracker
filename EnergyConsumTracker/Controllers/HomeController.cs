using Microsoft.AspNetCore.Mvc;
using EnergyConsumTracker.Services;
using EnergyConsumTracker.App.Models;

namespace EnergyConsumTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IoTDataSimulationService _simulationService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IoTDataSimulationService simulationService,
            ILogger<HomeController> logger)
        {
            _simulationService = simulationService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EnergyConsumption()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRealTimeData()
        {
            try
            {
                var devices = _simulationService.GetDevices();
                var realTimeData = devices.Select(d => new
                {
                    DeviceId = d.DeviceId,
                    DeviceType = d.DeviceType,
                    Location = d.Location,
                    ActualPowerW = d.ActualPowerW,
                    Status = d.Status,
                    Connectivity = d.Connectivity,
                    CumulativeEnergyKwh = d.CumulativeEnergyKwh,
                    Timestamp = d.Timestamp,
                    ConsumerId = d.ConsumerId
                }).ToList();

                return Json(new { success = true, data = realTimeData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching real-time data");
                return Json(new { success = false, message = "Error fetching data" });
            }
        }

      
    }
} 