﻿using System;
using Avalonia;

namespace Beans;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .AfterSetup(
                _ =>
                {
                    App.ConfigureDesktopServices();
                }
            )
            .WithInterFont()
            .UsePlatformDetect()
            .LogToTrace();
}
