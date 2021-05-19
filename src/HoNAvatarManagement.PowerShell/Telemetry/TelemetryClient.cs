using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using HoNAvatarManager.Core;
using PS = System.Management.Automation.PowerShell;
using Logger = HoNAvatarManager.Core.Logging.Logger;

namespace HoNAvatarManager.PowerShell.Telemetry
{
    public static class TelemetryClient
    {
        // Populate before the module release
        private const string TELEMETRY_INSTRUMENTATION_KEY = "";

        private static readonly PS _ps = PS.Create(RunspaceMode.CurrentRunspace);

        public static void InitializeTelemetry()
        {
            var configuration = ConfigurationManager.GetAppConfiguration();

            var telemetryModulePath = Path.Combine(Directory.GetCurrentDirectory(), "Modules", "HoNAvatarManager.Telemetry.psm1");

            var importTelemetryModuleCommand = _ps.AddCommand("Import-Module")
                .AddParameter("FullyQualifiedName", telemetryModulePath)
                .AddParameter("PassThru", true)
                .AddParameter("DisableNameChecking", true);

            var telemetryModule = importTelemetryModuleCommand.Invoke<PSModuleInfo>().FirstOrDefault();

            if (importTelemetryModuleCommand.HadErrors)
            {
                Logger.Log.Warning("Cannot load telemetry module. Telemetry is disabled.");

                configuration.TelemetryEnabled = false;
                ConfigurationManager.SetAppConfiguration(configuration);

                return;
            }

            var microsoftApplicationInsightsPath = Path.Combine(Directory.GetCurrentDirectory(), "Microsoft.ApplicationInsights.dll");

            var initializeTelemetryScript = _ps.AddScript($"Initialize-Telemetry " +
                $"-MicrosoftApplicationInsightsPath {microsoftApplicationInsightsPath} " +
                $"-InstrumentationKey {TELEMETRY_INSTRUMENTATION_KEY}");

            var _ = initializeTelemetryScript.Invoke();

            if (initializeTelemetryScript.HadErrors)
            {
                Logger.Log.Warning("Cannot initialize telemetry client. Telemetry is disabled.");

                configuration.TelemetryEnabled = false;
                ConfigurationManager.SetAppConfiguration(configuration);

                return;
            }

            Logger.Log.Information("Telemetry client initialized successfully. You could disable telemetry by running \"Disable-HoNAvatarManagerTelemetry\".");

            configuration.TelemetryEnabled = true;
            ConfigurationManager.SetAppConfiguration(configuration);
        }
    
        public static void TrackSetHeroAvatarEvent(string hero, string avatar)
        {
            ExecuteWithSuppresedException(() =>
            {
                var properties = new Dictionary<string, string>
            {
                { "Hero", hero },
                { "Avatar", avatar }
            };

                var trackTelemetryEventCommand = _ps.AddCommand("Track-TelemetryEvent")
                    .AddParameter("EventName", "Set-HeroAvatar-Execution")
                    .AddParameter("Properties", properties);

                var _ = trackTelemetryEventCommand.Invoke();
            });
        }

        public static void TrackException(Exception ex, Dictionary<string, string> properties)
        {
            ExecuteWithSuppresedException(() =>
            {
                var trackExceptionCommand = _ps.AddCommand("Track-Exception")
                .AddParameter("Exception", ex)
                .AddParameter("Properties", properties);

                var _ = trackExceptionCommand.Invoke();
            });
        }

        public static void ExecuteWithSuppresedException(Action action)
        {
            try
            {
                action();
            }
            catch
            {

            }
        }
    }
}
