using System;
using Serilog;
using Serilog.Configuration;

namespace HoNAvatarManager.PowerShell.Logging
{
    public static class PowerShellSinkExtensions
    {
        public static LoggerConfiguration PowerShellSink(this LoggerSinkConfiguration loggerConfiguration, IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new PowerShellSink(formatProvider));
        }
    }
}
