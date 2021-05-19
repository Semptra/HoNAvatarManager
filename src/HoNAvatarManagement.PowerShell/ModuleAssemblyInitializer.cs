using System.Management.Automation;
using HoNAvatarManager.PowerShell.Telemetry;

namespace HoNAvatarManager.PowerShell
{
    public class ModuleAssemblyInitializer : IModuleAssemblyInitializer
    {
        public void OnImport()
        {
            TelemetryClient.InitializeTelemetry();
        }
    }
}
