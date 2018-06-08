using Elfo.Wardein.Core;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;

namespace Elfo.Wardein.Services
{
    public class WardeinService : MicroService, IMicroService
    {
        public void Start()
        {
            var wardeinInstance = new WardeinInstance();

            this.StartBase();
            Timers.Start("Poller", 60000, async () =>
            {
                Console.WriteLine("Polling at {0}\n", DateTime.Now.ToString("o"));
                await wardeinInstance.RunCheck();
            },
            (e) =>
            {
                Console.WriteLine("Exception while polling: {0}\n", e.ToString());
            });
            Console.WriteLine("I started");
        }

        public void Stop()
        {
            this.StopBase();
            Console.WriteLine("I stopped");
        }
    }
}
