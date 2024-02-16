using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Beans.Model.Services;

namespace Beans.Services;

public class ApplicationService : IApplicationService
{
    public void ToggleTheme()
    {
        if (Application.Current is { })
        {
            Application.Current.RequestedThemeVariant =
                Application.Current.RequestedThemeVariant == ThemeVariant.Light
                    ? ThemeVariant.Dark
                    : ThemeVariant.Light;
        }
    }

    public void ToggleTopmost()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime
            {
                MainWindow: { } window
            })
        {
            window.Topmost = !window.Topmost;
        }
    }


    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}
