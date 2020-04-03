using Elfo.Wardein.Services;
using NLog;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein
{
    internal class WardeinMicroService : MicroService, IMicroService
    {
        ServiceBuilder serviceBuilder;
        static Logger log = LogManager.GetCurrentClassLogger();

        public WardeinMicroService(ServiceBuilder serviceBuilder)
        {
            log.Debug($"WardeinMicroService initialized, serviceBuilder initialized: {serviceBuilder is null == false}");
            this.serviceBuilder = serviceBuilder;
        }

        public void Start()
        {
            serviceBuilder.ConfigureAndRunWarden();
        }

        public void Stop()
        {
            serviceBuilder.Stop();
        }
    }
}
