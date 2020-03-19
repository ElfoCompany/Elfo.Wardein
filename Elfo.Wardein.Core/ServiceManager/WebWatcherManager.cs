using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Core.ExtensionMethods;
using Microsoft.Web.Administration;
using NLog;
using System;
using System.Linq;

namespace Elfo.Firmenich.Wardein.Core.ServiceManager
{
    public class WebWatcherManager : IAmServiceManager
    {
        #region Private variables
        private readonly ApplicationPool applicationPool;
        private readonly static Logger log = LogManager.GetCurrentClassLogger();
        #endregion

        public WebWatcherManager(string appPoolName) : base(appPoolName)
        {
            this.applicationPool = new ServerManager().ApplicationPools.FirstOrDefault(x => x.Name == appPoolName);

            if (applicationPool == null)
                throw new ArgumentNullException($"ApplicationPool {appPoolName} doesn't exist");
        }

        public override bool IsStillAlive => applicationPool.State == ObjectState.Started;

        public override void ForceKill()
        {
            try
            {
                log.Info($"Stopping pool {serviceName} @ {DateTime.UtcNow}");
                applicationPool.Stop();
                applicationPool.WaitForStatus(ObjectState.Stopped, TimeSpan.FromSeconds(30));
                log.Info($"{serviceName} pool stopped @ {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while killing {serviceName} IIS pool");
                throw;
            }
        }

        public override void Restart()
        {
            try
            {
                Stop();
                Start();
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while restarting {serviceName} IIS pool");
                throw;
            }
        }

        public override void Start()
        {
            try
            {
                log.Info($"Starting pool {base.serviceName} @ {DateTime.UtcNow}");
                if (!IsStillAlive)
                    this.applicationPool.Start();
                this.applicationPool.WaitForStatus(ObjectState.Started, TimeSpan.FromSeconds(30));
                log.Info($"{base.serviceName} pool started @ {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while starting {base.serviceName} IIS pool");
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
                log.Error(ex, $"Error while stopping {serviceName} IIS pool");
                throw;
            }
        }

        public override string GetStatus()
        {
            try
            {
                return applicationPool.State.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while getting status for pool {base.serviceName} IIS pool");
                throw;
            }
        }
    }
}
