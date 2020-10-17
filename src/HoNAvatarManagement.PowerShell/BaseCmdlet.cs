using System.Management.Automation;
using HoNAvatarManager.Core.Logging;
using HoNAvatarManager.PowerShell.Logging;

namespace HoNAvatarManager.PowerShell
{
    public abstract class BaseCmdlet : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            Logger.Configuration.WriteTo.PowerShellSink();

            if (MyInvocation.BoundParameters.ContainsKey("Verbose"))
            {
                Logger.Configuration.MinimumLevel.Verbose();
            }
        }
    }
}
