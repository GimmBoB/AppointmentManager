using AppointmentManager.Shared;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class Index
{
    private MudCarousel<string> _carousel = new();
    private string[] _source;
    private int _selectedIndex;
    
    public Index()
    {
        var assemblyFolderPath = new AppInfo().GetAssemblyFolderPath();
        var imagesPath = Path.Combine(assemblyFolderPath, "wwwroot/images");
        var directoryInfo = new DirectoryInfo(imagesPath);
        var fileInfos = directoryInfo.GetFiles();
        _source = fileInfos.Select(info => $"images/{info.Name}").ToArray();
    }

    protected override void OnInitialized()
    {
        
        base.OnInitialized();
    }
}