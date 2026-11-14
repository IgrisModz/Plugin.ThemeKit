using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Plugin.ThemeKit.WinUI.Sample.Helpers;

namespace Plugin.ThemeKit.WinUI.Sample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    readonly IHost host;

	/// <summary>
	/// Gets the application's <see cref="WinUIThemeManager"/>.
	/// </summary>
	internal static WinUIThemeManager ThemeManager { get; private set; } = null!;

    /// <summary>
    /// Gets the application's <see cref="IServiceProvider"/>.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
	{
		// This is an instance for the current application and is not necessarily required for yours.
		// This instance adds new custom parameters for navigation (orientation, etc.) and uses them in the project.
		// You can use it as inspiration for your own custom parameters if you need them.
		// Use it before InitializeComponent() to ensure that the parameters are available before the application is initialized.
		// ThemeKitWindowsInitialization.UseWindowsSettings(NavigationOrientationJsonContext.Default);

		// Without ServiceProvider
		// InitializeComponent();
		// var options = new ThemeOptions { RestoreOnStartup = true, SaveTheme = true,  }; // Default value is true and for default theme that's System setting, and default backdrop to Mica.
		// ThemeManager = new WinUIThemeManager(options);

		// with ServiceProvider
		host = Host.CreateDefaultBuilder()
			.ConfigureServices(services =>
			{
				services.AddWinUIThemeKit();
				services.AddSingleton<MainWindow>();
				services.AddTransient<MainPage>();

				services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<MainWindow>());
			})
			.Build();

		Services = host.Services;

		ThemeKitWindowsInitialization.UseWindowsSettings(NavigationOrientationJsonContext.Default);
		
		InitializeComponent();
	}

	/// <summary>
	/// Invoked when the application is launched.
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected override async void OnLaunched(LaunchActivatedEventArgs args)
	{
		// Without ServiceProvider
		// await ThemeManager.InitializeAsync();
		// var window = new MainWindow();
		// ThemeManager.TrackWindow(window);
		// window.Closed += async (_, _) => Application.Current.Exit();
		// window.Activate();

		//With ServiceProvider
		await host.StartAsync();

		var window = host.Services.GetRequiredService<MainWindow>();
        
		ThemeManager = host.Services.GetRequiredService<WinUIThemeManager>();

		await ThemeManager.InitializeAsync();

		ThemeManager.TrackWindow(window);

		window.Closed += async (_, _) => await host.StopAsync();
        window.Activate();
    }
}
