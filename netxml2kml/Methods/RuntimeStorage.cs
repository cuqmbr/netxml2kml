using Serilog;
using Serilog.Events;

namespace netxml2kml.Methods;

public static class RuntimeStorage
{
    public static string AppFolder = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "netxml2kml");

    public static string LogsFolder = Path.Join(AppFolder, "logs");
    
    public static bool IsVerbose = false;

    public static void ConfigureLogger()
    {
        var logLevel =
            IsVerbose ? LogEventLevel.Verbose : LogEventLevel.Warning;
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(logLevel,
                outputTemplate: "{Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Join(LogsFolder, "log.txt"),
                restrictedToMinimumLevel: logLevel,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}