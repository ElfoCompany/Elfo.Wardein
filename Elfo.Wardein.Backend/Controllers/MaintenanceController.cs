using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

namespace Elfo.Wardein.Backend.Controllers
{
    [ApiController]
    [Route("api/new/maintenance")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IAmWardeinConfigurationManager wardeinConfigurationManager;
        private readonly IServiceProvider serviceProvider;

        public MaintenanceController(IAmWardeinConfigurationManager wardeinConfigurationManager)
        {
            this.wardeinConfigurationManager = wardeinConfigurationManager;
            //this.serviceProvider = serviceProvider;
        }


        [HttpGet]
        public ActionResult<bool> Get()
        {
            return Ok(wardeinConfigurationManager.IsInMaintenanceMode);
        }

        [HttpGet("status")]
        public ActionResult<string> GetStatus()
        {
            var result = "Wardein is not in maintenance mode";
            if (wardeinConfigurationManager.IsInMaintenanceMode)
                result = "Wardein is in maintenance mode";
            return Ok(result);
        }

        [HttpGet("start/{durationInSecond}")]
        public ActionResult StartMaintenanceMode([FromRoute]double? durationInSecond = null)
        {
            if (!durationInSecond.HasValue)
                durationInSecond = TimeSpan.FromMinutes(5).TotalSeconds; // Default value

            wardeinConfigurationManager.StartMaintenanceMode(durationInSecond.Value);

            string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Wardein");
            System.IO.Directory.CreateDirectory(tempPath);
            using (System.IO.File.Create(System.IO.Path.Combine(tempPath, "cache.invalidate"))) ;

            return Ok($"Maintenance Mode Started");
        }

        [HttpGet("stop")]
        public ActionResult StopMaintenanceMode()
        {
            wardeinConfigurationManager.StopMaintenaceMode();

            string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Wardein");
            System.IO.Directory.CreateDirectory(tempPath);
            using (System.IO.File.Create(System.IO.Path.Combine(tempPath, "cache.invalidate"))) ;

            return Ok($"Maintenance Mode Stopped");
        }
    }
}
