using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elfo.Wardein.Core.ConfigurationManagers
{
    public class OracleWardeinConfigurationManager : IAmWardeinConfigurationManager
    {
        private readonly OracleConnectionConfiguration configuration;
        private readonly IOracleHelper oracleHelper;
        private readonly string hostname;
        private WardeinConfig cachedWardeinConfig;
        static Logger log = LogManager.GetCurrentClassLogger();

        public OracleWardeinConfigurationManager(IOracleHelper oracleHelper, string hostname)
        {
            this.oracleHelper = oracleHelper;
            this.hostname = hostname;
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
                    var expirationDate = GetExpirationDate();
                    log.Debug($"MaintenanceMode ExpirationDate: {GetExpirationDate()}");
                    return expirationDate <= DateTime.UtcNow;

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
            if (cachedWardeinConfig == null)
            {
                log.Debug($"Searching config for {hostname}");
                var parameters = new Dictionary<string, object>
                {
                    ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = hostname
                };
                var query = @"SELECT * FROM V_WRD_WATCHERS WHERE ""ApplicationHostname"" = :APPL_HOSTNAME";
                var waredinWatcherConfigs = this.oracleHelper.Query<WardeinConfigurationModel>(query, parameters);

                query = @"SELECT DT_MNTNC_END FROM WRD_CNFG WHERE APPL_HOSTNAME = :APPL_HOSTNAME";
                var maintenanceDate = this.oracleHelper.Query<DateTime>(query, parameters).FirstOrDefault();

                log.Debug($"Config found {waredinWatcherConfigs is null == false}");
                log.Debug($"{waredinWatcherConfigs?.Count()} watchers to be registered");
                var wardeinConfig = JObject.Parse(waredinWatcherConfigs.FirstOrDefault()?.WardeinConfig);
                foreach (var wardeinWatcherConfig in waredinWatcherConfigs)
                {
                    var watcherTypeConfig = JObject.Parse((string)wardeinWatcherConfig.WatcherTypeJsonConfig);
                    var watcherConfig = JObject.Parse((string)wardeinWatcherConfig.WatcherJsonConfig);
                    watcherConfig.AddDefaultProps(wardeinWatcherConfig.WatcherType, wardeinWatcherConfig.WatcherConfigurationId, wardeinWatcherConfig.ApplicationId, wardeinWatcherConfig.ApplicationHostname);
                    watcherTypeConfig.Merge(watcherConfig);
                    wardeinConfig.Merge(watcherTypeConfig);
                }

                cachedWardeinConfig = wardeinConfig.ToObject<WardeinConfig>();
                cachedWardeinConfig.MaintenanceModeStatus.MaintenanceModeStartDateInUTC = maintenanceDate;
                cachedWardeinConfig.MaintenanceModeStatus.DurationInSeconds = 0;
                cachedWardeinConfig.MaintenanceModeStatus.IsInMaintenanceMode = maintenanceDate >= DateTime.UtcNow;
            }

            return cachedWardeinConfig;
        }

        public void InvalidateCache()
        {
            cachedWardeinConfig = null;
        }

        public void StartMaintenanceMode(double durationInSeconds)
        {
            ToggleAndPersistMaintenanceModeStatus(startmaintenanceMode: true, durationInSeconds: durationInSeconds);
        }

        public void StopMaintenaceMode()
        {
            log.Debug($"Stopping MaintenanceMode before: {JsonConvert.SerializeObject(GetConfiguration().MaintenanceModeStatus)}");
            var parameters = new Dictionary<string, object>
            {
                ["DT_MNTNC_END"] = new OracleParameter("DT_MNTNC_END", OracleDbType.Date).Value = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1)),
                ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = hostname
            };
            var query = @"UPDATE WRD_CNFG SET DT_MNTNC_END  = :DT_MNTNC_END WHERE APPL_HOSTNAME =:APPL_HOSTNAME";
            oracleHelper.ExecuteAsync(query, parameters);
            ToggleAndPersistMaintenanceModeStatus(startmaintenanceMode: false);
            log.Debug($"Stopping MaintenanceMode after: {JsonConvert.SerializeObject(GetConfiguration().MaintenanceModeStatus)}");
        }

        private void ToggleAndPersistMaintenanceModeStatus(bool startmaintenanceMode, double? durationInSeconds = null)
        {
            if (GetConfiguration().MaintenanceModeStatus == null)
                GetConfiguration().MaintenanceModeStatus = new MaintenanceModeStatus();

            SetStatus();
            SetDurationInSecondIfNeccessary();
            SetStartDateIfNeccessary();

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
                {
                    var startDate = DateTime.UtcNow;
                    var endDate = startDate.AddSeconds((double)durationInSeconds);
                    var parameters = new Dictionary<string, object>
                    {
                        ["DT_MNTNC_END"] = new OracleParameter("DT_MNTNC_END", OracleDbType.Date).Value = endDate,
                        ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = hostname
                    };
                    var query = @"UPDATE WRD_CNFG SET DT_MNTNC_END  = :DT_MNTNC_END WHERE APPL_HOSTNAME =:APPL_HOSTNAME";
                    GetConfiguration().MaintenanceModeStatus.MaintenanceModeStartDateInUTC = startDate;
                    oracleHelper.ExecuteAsync(query, parameters);
                }
            }
            #endregion
        }
    }

    public class WardeinConfigurationModel
    {
        public int WatcherConfigurationId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationHostname { get; set; }
        public WardeinWatcherType WatcherType { get; set; }
        public string WatcherJsonConfig { get; set; }
        public string WatcherTypeJsonConfig { get; set; }
        public string WardeinConfig { get; set; }
    }

    public enum WardeinWatcherType
    {
        Unknown,
        WindowsService,
        CleanUp,
        IISPool,
        Web,
        WardeinHeartbeat,
        HealthAPI,
        Performance
    }

    internal static class JObjectExtensions
    {
        internal static void AddDefaultProps(this JObject config, WardeinWatcherType watcherType, int watcherConfigurationId, int applicationId, string applicationHostname)
        {
            JToken tokens;
            switch (watcherType)
            {
                case WardeinWatcherType.WindowsService:
                    if (config.TryGetValue("services", out tokens))
                        foreach (var token in tokens.AsJEnumerable())
                            token.AddDefaultProps(watcherConfigurationId, applicationId, applicationHostname);
                    break;
                case WardeinWatcherType.IISPool:
                    if (config.TryGetValue("iisPools", out tokens))
                        foreach (var token in tokens.AsJEnumerable())
                            token.AddDefaultProps(watcherConfigurationId, applicationId, applicationHostname);
                    break;
                case WardeinWatcherType.Web:
                    if (config.TryGetValue("urls", out tokens))
                        foreach (var token in tokens.AsJEnumerable())
                            token.AddDefaultProps(watcherConfigurationId, applicationId, applicationHostname);
                    break;
                case WardeinWatcherType.Performance:
                    if (config.TryGetValue("performances", out tokens))
                        foreach (var token in tokens.AsJEnumerable())
                            token.AddDefaultProps(watcherConfigurationId, applicationId, applicationHostname);
                    break;
                case WardeinWatcherType.WardeinHeartbeat:
                    if (config.TryGetValue("heartbeat", out tokens))
                        tokens.AddDefaultProps(watcherConfigurationId, applicationId, applicationHostname);
                    break;
                default:
                    return;
            }
        }

        internal static void AddDefaultProps(this JToken token, int watcherConfigurationId, int applicationId, string applicationHostname)
        {
            token["applicationId"] = applicationId;
            token["watcherConfigurationId"] = watcherConfigurationId;
            token["applicationHostname"] = applicationHostname;
        }
    }
}
