using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;

namespace Beans.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif

        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(Key.A, KeyModifiers.Control | KeyModifiers.Shift),
            Command = new RelayCommand(() =>
            {
                if (Application.Current is App application)
                {
                    application.ToggleAcrylicBlur();
                }
            })
        });

    }
}
