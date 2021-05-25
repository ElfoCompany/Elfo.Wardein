using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Warden;

namespace Elfo.Wardein.Backend.Controllers
{
    [ApiController]
    [Route("api/new/configs")]
    public class ConfigsController : ControllerBase
    {
        private readonly IAmWardeinConfigurationManager wardeinConfigurationManager;

        public ConfigsController(IAmWardeinConfigurationManager wardeinConfigurationManager)
        {
            this.wardeinConfigurationManager = wardeinConfigurationManager;
        }

        [HttpGet("invalidate")]
        public async Task<ActionResult<string>> Invalidate([FromServices]IWarden warden)
        {
            ServicesContainer.MailConfigurationManager().InvalidateCache();
            ServicesContainer.WardeinConfigurationManager().InvalidateCache();
            await warden.StopAsync();
            warden.StartAsync();
            return Ok($"Cached configs invalidated");
        }
    }
}
