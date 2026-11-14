# Plugin.ThemeKit

Plugin.ThemeKit helps .NET MAUI and WinUI 3 applications manage their appearance. Let users choose a light or dark theme, or follow the system setting. On Windows, you can also choose a window backdrop such as Mica or Acrylic.

## Features

- Choose between system, light, and dark themes.
- Choose a Windows backdrop: None, Mica, Mica Alt, Acrylic, or Acrylic Thin.
- Save the user's choices and restore them when the application starts.
- Use dependency injection to access the theme manager.

Backdrop effects depend on Windows support. In a MAUI application, backdrops are applied only on Windows; theme selection also works on Android, iOS, and Mac Catalyst.

## Choose your package

| Application | Package | Guide |
| --- | --- | --- |
| .NET MAUI | `IgrisModz.Plugin.ThemeKit.Maui` | [MAUI guide](src/Plugin.ThemeKit.Maui/README.md) |
| WinUI 3 | `IgrisModz.Plugin.ThemeKit.WinUI` | [WinUI guide](src/Plugin.ThemeKit.WinUI/README.md) |

Both packages depend on the shared `IgrisModz.Plugin.ThemeKit` package. You normally only need to install the package for your application type. The projects target .NET 10.

When the packages are available from your NuGet source, install the one you need:

```sh
dotnet add package IgrisModz.Plugin.ThemeKit.Maui
```

or:

```sh
dotnet add package IgrisModz.Plugin.ThemeKit.WinUI
```

## Basic usage

Choose either dependency injection or direct creation for your application. Call `InitializeAsync` once at startup, after the application is ready. By default, it restores the saved theme and backdrop.

### .NET MAUI

For dependency injection, add ThemeKit in `MauiProgram.cs`:

```csharp
using Plugin.ThemeKit.Maui;

// Inside CreateMauiApp, use the existing builder:
builder.UseMauiApp<App>()
       .UseMauiThemeKit();
```

Alternatively, register the manager directly with the service collection using `builder.Services.AddMauiThemeKit()`. Use one registration method, not both. You can then receive the registered manager in a page constructor:

```csharp
using Plugin.ThemeKit.Maui;

public partial class MainPage : ContentPage
{
    private readonly MauiThemeManager themeManager;

    public MainPage(MauiThemeManager themeManager)
    {
        InitializeComponent();
        this.themeManager = themeManager;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnLoaded;
        await themeManager.InitializeAsync();
    }
}
```

Make sure the page is created through MAUI's dependency injection, so it can receive the manager. For an example using Shell, see the [MAUI sample](samples/Plugin.ThemeKit.Maui.Sample).

To create the manager directly instead, without registering ThemeKit as a service, replace the constructor and field above with:

```csharp
private readonly MauiThemeManager themeManager = new();

public MainPage()
{
    InitializeComponent();
    Loaded += OnLoaded;
}
```

Keep the same `OnLoaded` method to initialize it. In either approach, keep one manager instance and initialize it once for the application.

### WinUI 3

In `App.xaml.cs`, you can create the manager directly:

```csharp
using Plugin.ThemeKit.WinUI;

// Inside the App class:
private readonly WinUIThemeManager themeManager = new();
```

Alternatively, use a service collection. This example requires the `Microsoft.Extensions.DependencyInjection` package. Replace the field above and add the registration to your existing `App` constructor:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Plugin.ThemeKit.WinUI;

// Inside the App class:
private readonly ServiceProvider services;
private readonly WinUIThemeManager themeManager;

public App()
{
    InitializeComponent();

    var serviceCollection = new ServiceCollection();
    serviceCollection.AddWinUIThemeKit();
    services = serviceCollection.BuildServiceProvider();
    themeManager = services.GetRequiredService<WinUIThemeManager>();
}
```

Keep the service provider for the application's lifetime, and dispose it when it is no longer needed. If your app already has a service provider, register ThemeKit with its service collection instead of creating another one.

For either approach, initialize the manager and track your window in `OnLaunched`:

```csharp
// Inside the App class:
private Window? window;

protected override async void OnLaunched(LaunchActivatedEventArgs args)
{
    await themeManager.InitializeAsync();
    window = new MainWindow();
    themeManager.TrackWindow(window);
    window.Activate();
}
```

Add `using Microsoft.UI.Xaml;` to `App.xaml.cs` if it is not already present. Track each window that should use ThemeKit; it is automatically untracked when it closes.

### Change the theme or backdrop

Once the manager is initialized, use the same instance from your page or event handler:

```csharp
using Plugin.ThemeKit;

// Inside an async method:
await themeManager.SetThemeAsync(Theme.Dark);
```

Use `Theme.Light` or `Theme.System` for the other choices. On Windows, you can also change the backdrop:

```csharp
await themeManager.SetBackdropAsync(Backdrop.Mica);
```

See the [MAUI guide](src/Plugin.ThemeKit.Maui/README.md) and [WinUI guide](src/Plugin.ThemeKit.WinUI/README.md) for more examples and options.

## Samples

- [MAUI sample](samples/Plugin.ThemeKit.Maui.Sample): choose a theme and backdrop using pickers.
- [WinUI sample](samples/Plugin.ThemeKit.WinUI.Sample): choose a theme and backdrop from the settings page.

Open `Plugin.ThemeKit.slnx` in Visual Studio and run the sample for your platform. Building MAUI and WinUI projects requires the corresponding Visual Studio workloads and platform tools.
