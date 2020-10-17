using System;
using System.Management.Automation;
using Serilog.Core;
using Serilog.Events;
using PS = System.Management.Automation.PowerShell;

namespace HoNAvatarManager.PowerShell.Logging
{
    public class PowerShellSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public PowerShellSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);

            PS.Create(RunspaceMode.CurrentRunspace)
                .AddScript("$VerbosePreference = 'Continue'")
                .Invoke();

            PS.Create(RunspaceMode.CurrentRunspace)
                .AddCommand("Write-Verbose")
                .AddParameter("Message", message)
                .Invoke();
        }
    }
}
