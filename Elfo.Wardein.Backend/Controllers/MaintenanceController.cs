using Elfo.Wardein.Abstractions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elfo.Wardein.Backend.Controllers
{
    [ApiController]
    [Route("api/new/maintenance")]
    public class MaintenanceController : Controller
    {
        private readonly IAmWardeinConfigurationManager wardeinConfigurationManager;

        public MaintenanceController(IAmWardeinConfigurationManager wardeinConfigurationManager)
        {
            this.wardeinConfigurationManager = wardeinConfigurationManager;
        }

        [HttpGet("status")]
        public ActionResult<string> GetStatus()
        {
            var result = "Wardein is not in maintenance mode";
            if (wardeinConfigurationManager.IsInMaintenanceMode)
                result = "Wardein is in maintenance mode";
            return Ok(result);
        }
    }
}
