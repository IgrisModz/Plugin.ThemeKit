# Plugin.ThemeKit.WinUI

Plugin.ThemeKit.WinUI helps a WinUI 3 app apply a light, dark, or system theme and a Windows window backdrop such as Mica or Acrylic.

## Requirements

- .NET 10 and a WinUI 3 desktop application.
- This package is for Windows apps. Backdrop effects also depend on Windows support for the selected effect.

## Install

```sh
dotnet add package Plugin.ThemeKit.WinUI
```

This package brings in the shared `Plugin.ThemeKit` library as a dependency. You do not need to install that package separately.

## Quick start

Create a theme manager, initialize it when the app starts, and track each window. For example, in `App.xaml.cs`:

```csharp
using Microsoft.UI.Xaml;
using Plugin.ThemeKit;
using Plugin.ThemeKit.WinUI;

public partial class App : Application
{
    private readonly WinUIThemeManager themeManager = new();

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await themeManager.InitializeAsync();

        var window = new MainWindow();
        themeManager.TrackWindow(window);
        window.Activate();
    }
}
```

Tracking a window lets ThemeKit apply theme and backdrop changes to it. Track every window that should use ThemeKit; a window is automatically untracked when it closes.

To change the theme from a button or another event handler, call:

```csharp
await themeManager.SetThemeAsync(Theme.Dark);
```

Use `Theme.Light` or `Theme.System` for the other choices. To change the backdrop, call:

```csharp
await themeManager.SetBackdropAsync(Backdrop.Mica);
```

Available backdrops are `None`, `Mica`, `MicaAlt`, `Acrylic`, and `AcrylicThin`.

## Saved settings

By default, ThemeKit saves the selected theme and backdrop and restores them when the app starts. Call `InitializeAsync` once during startup before tracking the window. You can change this behavior with `ThemeOptions`, for example:

```csharp
var themeManager = new WinUIThemeManager(new ThemeOptions
{
    RestoreOnStartup = true,
    SaveTheme = true,
    SaveBackdrop = true,
    DefaultTheme = Theme.System,
    DefaultBackdrop = Backdrop.Mica
});
```

## Sample

See `samples/Plugin.ThemeKit.WinUI.Sample` for a working WinUI example. It also shows how to register the manager with dependency injection.
