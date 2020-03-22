using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.ConfigurationManagers
{
    public class WardeinConfigurationManagerFromJSON : IAmWardeinConfigurationManager
    {
        private readonly string wardeinConfigurationPath;
        private readonly IOHelper ioHelper;
        private WardeinConfig cachedWardeinConfig;

        public WardeinConfigurationManagerFromJSON(string wardeinConfigurationPath)
        {
            this.wardeinConfigurationPath = wardeinConfigurationPath;
            this.ioHelper = new IOHelper(this.wardeinConfigurationPath);
        }

        public bool IsInMaintenanceMode
        {
            get
            {
                if (!GetMaintenanceModeValue())
                    return false;

                if (IsMaintenanceModeTimeoutExpired())
                    StopMaintenaceMode();

                return GetMaintenanceModeValue();

                #region Local Functions

                bool GetMaintenanceModeValue() => GetConfiguration().MaintenanceModeStatus?.IsInMaintenanceMode ?? false;

                bool IsMaintenanceModeTimeoutExpired()
                {
                    return GetExpirationDate() <= DateTime.UtcNow;

                    #region Local Functions

                    DateTime GetExpirationDate() =>
                        GetConfiguration().MaintenanceModeStatus.MaintenanceModeStartDateInUTC.AddSeconds(GetConfiguration().MaintenanceModeStatus.DurationInSeconds);

                    #endregion
                }

                #endregion
            }
        }

        public WardeinConfig GetConfiguration()
        {
            if (this.cachedWardeinConfig == null)
                this.cachedWardeinConfig = JsonConvert.DeserializeObject<WardeinConfig>(this.ioHelper.GetFileContent());

            return this.cachedWardeinConfig;
        }

        public void InvalidateCache() => this.cachedWardeinConfig = null;

        public void StartMaintenanceMode(double durationInSeconds = 300)
        {
            ToggleAndPersistMaintenanceModeStatus(startmaintenanceMode: true, durationInSeconds: durationInSeconds);
        }

        public void StopMaintenaceMode()
        {
            ToggleAndPersistMaintenanceModeStatus(startmaintenanceMode: false);
        }

        private void ToggleAndPersistMaintenanceModeStatus(bool startmaintenanceMode, double? durationInSeconds = null)
        {
            if (GetConfiguration().MaintenanceModeStatus == null)
                GetConfiguration().MaintenanceModeStatus = new MaintenanceModeStatus();

            SetStatus();
            SetDurationInSecondIfNeccessary();
            SetStartDateIfNeccessary();

            this.ioHelper.PersistFileOnDisk(JsonConvert.SerializeObject(GetConfiguration()));

            #region Local Functions

            void SetStatus()
            {
                GetConfiguration().MaintenanceModeStatus.IsInMaintenanceMode = startmaintenanceMode;
            }

            void SetDurationInSecondIfNeccessary()
            {
                if (durationInSeconds.HasValue)
                    GetConfiguration().MaintenanceModeStatus.DurationInSeconds = durationInSeconds.Value;
            }

            void SetStartDateIfNeccessary()
            {
                if (startmaintenanceMode)
                    GetConfiguration().MaintenanceModeStatus.MaintenanceModeStartDateInUTC = DateTime.UtcNow;
            }

            #endregion
        }
    }
}
