using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Configurations;
using Elfo.Wardein.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
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
        private readonly IAmMailConfigurationManager mailConfigurationManager;

        public ConfigsController(IAmWardeinConfigurationManager wardeinConfigurationManager, IAmMailConfigurationManager mailConfigurationManager)
        {
            this.wardeinConfigurationManager = wardeinConfigurationManager;
            this.mailConfigurationManager = mailConfigurationManager;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return Ok(wardeinConfigurationManager.GetConfiguration());
        }

        [HttpGet("invalidate")]
        public async Task<ActionResult<string>> Invalidate([FromServices]IWarden warden)
        {
            mailConfigurationManager.InvalidateCache();
            wardeinConfigurationManager.InvalidateCache();
            /*
             * Warden was not working with this unfortunately, so had to implement a workaround
             * "Put file in temp folder from API and windows service with filesystemwatcher detect it
             * and throw exception to be restarted by Windows so that it reads new configs"
            */
            //await warden.StopAsync();
            //warden.Reconfigure(config =>
            //{
            //    config.ClearWatchers();
            //    config.ConfigureWardenConfigurationBuilder(wardeinConfigurationManager.GetConfiguration());
            //});
            //warden.StartAsync();
            string tempPath = Path.Combine(Path.GetTempPath(),"Wardein");
            System.IO.Directory.CreateDirectory(tempPath);
            using (System.IO.File.Create(Path.Combine(tempPath, "cache.invalidate"))) ;
            return Ok($"Cached configs invalidated");
        }

        [HttpGet("start")]
        public ActionResult<string> Start([FromServices] IWarden warden)
        {
            mailConfigurationManager.InvalidateCache();
            wardeinConfigurationManager.InvalidateCache();
            warden.StartAsync();
            return Ok($"Warden Started");
        }

        [HttpGet("stop")]
        public async Task<ActionResult<string>> Stop([FromServices] IWarden warden)
        {
            mailConfigurationManager.InvalidateCache();
            wardeinConfigurationManager.InvalidateCache();
            await warden.StopAsync();
            return Ok($"Warden stopped");
        }
    }
}
