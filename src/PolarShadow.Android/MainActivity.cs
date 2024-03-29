﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.NativeControls.Android;
using Android.Views;
using Avalonia.Controls.Android;
using Avalonia.Dialogs;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace PolarShadow.Android;

[Activity(
    Label = "PolarShadow",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .AfterSetup(builder =>
            {
                var services = (builder.Instance as App).ServiceCollection;
                builder.UseNativeControls(services);
                builder.UseEssentials(services, this);
            });
    }

    protected override void OnStop()
    {
        base.OnStop();
        App.FileCacheFlush();
    }

    protected override void OnPause()
    {
        base.OnPause();
        App.FileCacheFlush();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        App.FileCacheFlush();
    }
}
