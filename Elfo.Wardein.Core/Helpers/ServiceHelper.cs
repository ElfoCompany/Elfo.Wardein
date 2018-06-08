using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Elfo.Wardein.Core.Helpers
{
    public class WindowsServiceHelper
    {
        private readonly string serviceName;
        private readonly ServiceController serviceController;

        public WindowsServiceHelper(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException("Service Name cannot be null");

            this.serviceName = serviceName;
            this.serviceController = new ServiceController(serviceName);
        }

        public bool IsStillAlive()
        {
            var result = false;
            try
            {
                switch (this.serviceController.Status)
                {
                    case ServiceControllerStatus.Running:
                        result = true;
                        break;
                    case ServiceControllerStatus.StartPending:
                        result = false;
                        break;
                    //case ServiceControllerStatus.Stopped:
                    //    return false;
                    //case ServiceControllerStatus.Paused:
                    //    return false;
                    //case ServiceControllerStatus.StopPending:
                    //    return false;
                    default:
                        result = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Check service if still alive error: {ex.ToString()}");
            }

            return result;
        }

        private void ForceKill()
        {
            ServiceController sc = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.Equals(serviceName));
            Console.WriteLine($"Stopping {sc.ServiceName}");
            if (sc != null)
            {
                sc.Stop();
                Process[] procs = Process.GetProcesses().Where(x => x.ProcessName.StartsWith(serviceName)).ToArray();
                Console.WriteLine(string.Join(",", procs.Select(x => x.ProcessName)));
                if (procs.Length > 0)
                {
                    foreach (Process proc in procs)
                    {
                        Console.WriteLine($"Killing {proc.ProcessName} with PID:{proc.Id}");
                        //do other stuff if you need to find out if this is the correct proc instance if you have more than one
                        proc.Kill();
                    }
                }
            }
        }

        public void Start()
        {
            this.serviceController.Start();
        }
    }
}
