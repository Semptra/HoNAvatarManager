using System;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace HoNAvatarManager.Core.Logging
{
    public static class Logger
    {
        private static readonly Lazy<LoggerConfiguration> _configuration = new Lazy<LoggerConfiguration>(
            () => new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Code));

        private static readonly Lazy<ILogger> _log = new Lazy<ILogger>(() => _configuration.Value.CreateLogger());

        public static LoggerConfiguration Configuration => _configuration.Value;

        public static ILogger Log => _log.Value;
    }
}
