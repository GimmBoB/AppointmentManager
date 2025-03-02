namespace AppointmentManager.Shared;

public static class AppInfoExtensions
{
    public static string GetAssemblyFolderPath(this AppInfo appInfo)
    {
        var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var assemblyFolderPath = Path.GetDirectoryName(assemblyPath) ?? appInfo.ApplicationFolder;

        return assemblyFolderPath;
    }
}