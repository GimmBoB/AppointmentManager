using AppointmentManager.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class Index
{
    private MudCarousel<string> _carousel = new();
    private string[] _source;
    
    [Inject] public IStringLocalizer<PageText> Localizer { get; set; }

    public Index()
    {
        var assemblyFolderPath = new AppInfo().GetAssemblyFolderPath();
        var imagesPath = Path.Combine(assemblyFolderPath, "wwwroot/images");
        var directoryInfo = new DirectoryInfo(imagesPath);
        var fileInfos = directoryInfo.GetFiles();
        _source = fileInfos.Select(info => $"images/{info.Name}").ToArray();
    }
}