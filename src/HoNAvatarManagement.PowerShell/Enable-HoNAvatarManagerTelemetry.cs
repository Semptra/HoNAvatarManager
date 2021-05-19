using System.Management.Automation;
using HoNAvatarManager.Core;

namespace HoNAvatarManager.PowerShell
{
    [Cmdlet(VerbsLifecycle.Enable, "HoNAvatarManagerTelemetry")]
    public class EnableHoNAvatarManagerTelemetry : BaseCmdlet
    {
        protected override void ProcessRecord()
        {
            var configuration = ConfigurationManager.GetAppConfiguration();

            configuration.TelemetryEnabled = true;

            ConfigurationManager.SetAppConfiguration(configuration);
        }
    }
}
