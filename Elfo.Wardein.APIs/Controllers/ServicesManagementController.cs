using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elfo.Wardein.Core;
using Microsoft.AspNetCore.Mvc;

namespace Elfo.Wardein.APIs.Controllers
{
    [Route("api/[controller]/[action]/{servicename?}")]
    public class ServicesManagementController : Controller
    {
        [Route("")]
        [Route("api/")]
        [Route("api/ServicesManagement/")]
        public string CheckStatus() => "I'm your Wardein Service and I'm OK, please call an API, I'm here for you!";

        [HttpGet]
        public void RestartService(string servicename)
        {
            var wardeinInstance = new WardeinInstance(); //TODO: implement DI?
            wardeinInstance.RestartService(servicename);
        }

        [HttpGet]
        public void StopService(string servicename)
        {
            var wardeinInstance = new WardeinInstance(); //TODO: implement DI?
            wardeinInstance.StopService(servicename);
        }

        [HttpGet]
        public void StartService(string servicename)
        {
            var wardeinInstance = new WardeinInstance(); //TODO: implement DI?
            wardeinInstance.StartService(servicename);
        }

    }
}
