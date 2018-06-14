using Elfo.Wardein.Core.Abstractions;
using NLog;
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
        private readonly static Logger log = LogManager.GetCurrentClassLogger();
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
                log.Info($"Stopping {sc.ServiceName}");
                if (sc != null)
                {
                    sc.Stop();
                    Process[] procs = Process.GetProcesses().Where(x => x.ProcessName.StartsWith(base.serviceName)).ToArray();
                    log.Info(string.Join(",", procs.Select(x => x.ProcessName)));
                    if (procs.Length > 0)
                    {
                        foreach (Process proc in procs)
                        {
                            log.Info($"Killing {proc.ProcessName} with PID: {proc.Id}");
                            //do other stuff if you need to find out if this is the correct proc instance if you have more than one
                            proc.Kill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while killing {base.serviceName}");
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
                log.Error(ex, $"Error while starting {base.serviceName}");
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
                log.Error(ex, $"Error while stopping {base.serviceName}");
                throw;
            }
        }

        public override void Restart()
        {
            try
            {
                Stop();
                this.serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)); // TODO: get this value from config
                log.Info($"Service {base.serviceName} stopped @ {DateTime.Now}.");
                Start();
                this.serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)); // TODO: get this value from config
                log.Info($"Service {base.serviceName} started @ {DateTime.Now}.");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while restarting {base.serviceName}");
                throw;
            }
        }

        public override string GetStatus()
        {
            try
            {
                return this.serviceController.Status.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while getting status for service {base.serviceName}");
                throw;
            }
        }
    }
}
