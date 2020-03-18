using Elfo.Wardein.Services;
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

        public WardeinMicroService(ServiceBuilder serviceBuilder)
        {
            this.serviceBuilder = serviceBuilder;
        }

        public void Start()
        {
            serviceBuilder.ConfigureAndRunWarden().Wait();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
