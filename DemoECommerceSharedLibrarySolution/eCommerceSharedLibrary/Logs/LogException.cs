using Microsoft.Extensions.Logging.Configuration;
using Serilog;

namespace eCommerce.SharedLibrary.Logs
{
    public static class LogException
    {
        public static void LogExceptions(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }

        private static void LogToFile(string message) => Log.Information($"Logging to file: {message}");
        private static void LogToConsole(string message) => Log.Warning($"Logging to console: {message}");
        private static void LogToDebugger(string message) => Log.Debug($"Logging to debugger: {message}");

    }
}
