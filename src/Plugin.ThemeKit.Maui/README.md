# Plugin.ThemeKit.Maui

Plugin.ThemeKit.Maui helps a .NET MAUI app apply a light, dark, or system theme. On Windows, it can also apply a window backdrop such as Mica or Acrylic.

## Requirements

- .NET 10 and a .NET MAUI application.
- The backdrop effects are available only when the MAUI app runs on Windows. Theme selection works on the other supported MAUI platforms too.

## Install

```sh
dotnet add package Plugin.ThemeKit.Maui
```

This package brings in the shared `Plugin.ThemeKit` library as a dependency. You do not need to install that package separately.

## Quick start

Register ThemeKit in `MauiProgram.cs`:

```csharp
using Plugin.ThemeKit;
using Plugin.ThemeKit.Maui;

// Inside CreateMauiApp, add this to the existing builder chain:
builder
    .UseMauiApp<App>()
    .UseMauiThemeKit(new ThemeOptions
    {
        DefaultTheme = Theme.System,
        DefaultBackdrop = Backdrop.None
    });
```

`UseMauiThemeKit` registers the theme manager with MAUI's dependency injection. You can receive it in a page constructor, as in the sample app:

```csharp
using Plugin.ThemeKit;
using Plugin.ThemeKit.Maui;

public partial class SettingsPage : ContentPage
{
    private readonly MauiThemeManager themeManager;

    public SettingsPage(MauiThemeManager themeManager)
    {
        InitializeComponent();
        this.themeManager = themeManager;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await themeManager.InitializeAsync();
    }

    private async void UseDarkThemeClicked(object? sender, EventArgs e)
    {
        await themeManager.SetThemeAsync(Theme.Dark);
    }
}
```

Connect `UseDarkThemeClicked` to a button in your page, or call `SetThemeAsync` from your own event handler. You can also use `Theme.Light` or `Theme.System`.

On Windows, change the window backdrop in the same way:

```csharp
await themeManager.SetBackdropAsync(Backdrop.Mica);
```

Available backdrops are `None`, `Mica`, `MicaAlt`, `Acrylic`, and `AcrylicThin`. MAUI does not apply these backdrops on Android, iOS, or Mac Catalyst.

## Saved settings

By default, ThemeKit saves the selected theme and backdrop and restores them when the app starts. Call `InitializeAsync` once when the app starts to apply the saved choices. You can change this behavior with `ThemeOptions`, for example:

```csharp
new ThemeOptions
{
    RestoreOnStartup = true,
    SaveTheme = true,
    SaveBackdrop = true
}
```

## Sample

See `samples/Plugin.ThemeKit.Maui.Sample` for a working MAUI example, including theme and backdrop pickers.
