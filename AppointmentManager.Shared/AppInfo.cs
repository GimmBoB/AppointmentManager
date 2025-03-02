using System.Reflection;

namespace AppointmentManager.Shared;

public class AppInfo
{
    public string ApplicationFolder { get; }

    public AppInfo()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null) throw new InvalidOperationException("entry assembly must not be null");
        ApplicationFolder = GetFolderPath(entryAssembly);
    }
    
    private static string GetFolderPath(Assembly assembly)
    {
        var assemblyFilePath  = assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyFilePath);
        return assemblyDirectory ?? "";
    }
}