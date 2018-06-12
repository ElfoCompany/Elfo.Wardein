using Elfo.Wardein.Core.Abstractions;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace Elfo.Wardein.Core.Helpers
{
    public class WindowsServiceHelper : IAmServiceManager
    {
        #region Private Variables
        private readonly ServiceController serviceController;
        #endregion

        #region Constructor
        public WindowsServiceHelper(string serviceName) : base(serviceName)
        {
            this.serviceController = new ServiceController(serviceName);
        }
        #endregion

        public override bool IsStillAlive => this.serviceController?.Status == ServiceControllerStatus.Running;

        public override void ForceKill()
        {
            try
            {
                ServiceController sc = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.Equals(base.serviceName));
                Console.WriteLine($"Stopping {sc.ServiceName}");
                if (sc != null)
                {
                    sc.Stop();
                    Process[] procs = Process.GetProcesses().Where(x => x.ProcessName.StartsWith(base.serviceName)).ToArray();
                    Console.WriteLine(string.Join(",", procs.Select(x => x.ProcessName)));
                    if (procs.Length > 0)
                    {
                        foreach (Process proc in procs)
                        {
                            Console.WriteLine($"Killing {proc.ProcessName} with PID: {proc.Id}");
                            //do other stuff if you need to find out if this is the correct proc instance if you have more than one
                            proc.Kill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while killing {base.serviceName}: {ex.Message}");
                throw;
            }
        }

        public override void Start()
        {
            try
            {
                if (!IsStillAlive)
                    this.serviceController.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while starting {base.serviceName}: {ex.Message}");
                throw;
            }
        }

        public override void Stop()
        {
            try
            {
                if (IsStillAlive)
                    ForceKill();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while stopping {base.serviceName}: {ex.Message}");
                throw;
            }
        }

        public override void Restart()
        {
            try
            {
                Stop();
                this.serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                Console.WriteLine($"Service {base.serviceName} stopped @ {DateTime.Now}.");
                Start();
                this.serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                Console.WriteLine($"Service {base.serviceName} started @ {DateTime.Now}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while restarting {base.serviceName}: {ex.Message}");
                throw;
            }
        }
    }
}
