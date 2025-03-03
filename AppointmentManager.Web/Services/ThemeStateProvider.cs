using MudBlazor;

namespace AppointmentManager.Web.Services;

public class ThemeStateProvider
{
    public bool IsDarkMode { get; set; }
    
    public event Action? StateHasChanged;
    private readonly MudTheme _customMudTheme;

    public ThemeStateProvider()
    {
        _customMudTheme = CreateCustomTheme();
    }
    
    public MudTheme GetCustomTheme()
    {
        return _customMudTheme;
    }
    
    public string GetCustomMarkedDateClass()
    {
        return IsDarkMode ? "customMarkedDateDark" : "customMarkedDateLight";
    }

    public virtual void OnStateHasChanged()
    {
        StateHasChanged?.Invoke();
    }
    
    private static MudTheme CreateCustomTheme()
    {
        return new MudTheme()
        {
            Palette = CreatePaletteLight(),
            PaletteDark = CreatePaletteDark(),
            LayoutProperties = CreateLayoutProperties()
        };
    }

    private static PaletteLight CreatePaletteLight()
    {
        return new PaletteLight
        {
            Background = "#e4e3e6d0",
            LinesDefault = "#010101ff",
            Surface = "#FFFFFFFF",
            AppbarBackground = "#e1e1e1"
        };
    }

    private static PaletteDark CreatePaletteDark()
    {
        var darkPalette = new PaletteDark
        {
            TableStriped = "#3a3a42",
            Primary = "#625c9cff",
            Secondary = "#f70457",
            BackgroundGrey = "#27272f1a"
        };

        return darkPalette;
    }

    private static LayoutProperties CreateLayoutProperties()
    {
        return new LayoutProperties
        {
            DefaultBorderRadius = "12px",
        };
    }
}