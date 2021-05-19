using System.Management.Automation;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell
{
    [Cmdlet(VerbsLifecycle.Disable, "HoNAvatarManagerTelemetry")]
    public class DisableHoNAvatarManagerTelemetry : BaseCmdlet
    {
        protected override void ProcessRecord()
        {
            var configuration = ConfigurationManager.GetAppConfiguration();

            configuration.TelemetryEnabled = false;

            ConfigurationManager.SetAppConfiguration(configuration);
        }
    }
}
