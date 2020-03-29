using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Core.ExtensionMethods;
using Microsoft.Web.Administration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.ServiceManager
{
    public class IISPoolManager : IAmServiceManager
    {
        #region Private variables
        private readonly ApplicationPool applicationPool;
        private readonly static Logger log = LogManager.GetCurrentClassLogger();
        #endregion

        public IISPoolManager(string appPoolName) : base(appPoolName)
        {
            this.applicationPool = new ServerManager().ApplicationPools.FirstOrDefault(x => x.Name == appPoolName);

            if (this.applicationPool == null)
                throw new ArgumentNullException($"ApplicationPool {appPoolName} doesn't exist");
        }

        public override async Task<bool> IsStillAlive() => await Task.FromResult(this.applicationPool.State == ObjectState.Started);

        public override async Task ForceKill()
        {
            try
            {
                log.Debug($"Stopping pool {base.serviceName}");
                this.applicationPool.Stop();
                this.applicationPool.WaitForStatus(ObjectState.Stopped, TimeSpan.FromSeconds(30));
                log.Debug($"{base.serviceName} pool stopped @ {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while killing {base.serviceName} IIS pool");
                throw;
            }
        }

        public override async Task Restart()
        {
            try
            {
                await Stop();
                await Start();
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while restarting {base.serviceName} IIS pool");
                throw;
            }
        }

        public override async Task Start()
        {
            try
            {
                log.Debug($"Starting pool {base.serviceName}");
                if (!await IsStillAlive())
                    this.applicationPool.Start();
                this.applicationPool.WaitForStatus(ObjectState.Started, TimeSpan.FromSeconds(30));
                log.Debug($"{base.serviceName} pool started");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while starting {base.serviceName} IIS pool");
                throw;
            }
        }

        public override async Task Stop()
        {
            try
            {
                if (await IsStillAlive())
                    await this.ForceKill();
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while stopping {base.serviceName} IIS pool");
                throw;
            }
        }

        public override async Task<string> GetStatus()
        {
            try
            {
                return await Task.FromResult(this.applicationPool.State.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while getting status for pool {base.serviceName} IIS pool");
                throw;
            }
        }
    }
}
