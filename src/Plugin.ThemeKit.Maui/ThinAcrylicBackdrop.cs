#if WINDOWS
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using WinRT;
using WindowsThemeSettings = Microsoft.UI.System.ThemeSettings;

namespace Plugin.ThemeKit.Maui;

sealed partial class ThinAcrylicBackdrop : IDisposable
{
	readonly Microsoft.UI.Xaml.Window window;
	readonly Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
	readonly DesktopAcrylicController controller;
	readonly SystemBackdropConfiguration configuration;
	readonly WindowsThemeSettings themeSettings;
	readonly XamlRoot? xamlRoot;
	FrameworkElement? themeSource;
	bool disposed;

	public ThinAcrylicBackdrop(Microsoft.UI.Xaml.Window window)
	{
		this.window = window;
		dispatcherQueue = window.DispatcherQueue;
		configuration = new SystemBackdropConfiguration { IsInputActive = true };
		themeSettings = WindowsThemeSettings.CreateForWindowId(window.AppWindow.Id);
		configuration.IsHighContrast = themeSettings.HighContrast;
		xamlRoot = (window.Content as FrameworkElement)?.XamlRoot;
		controller = new DesktopAcrylicController { Kind = DesktopAcrylicKind.Thin };

		try
		{
			controller.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
			controller.SetSystemBackdropConfiguration(configuration);
			UpdateThemeSource();
			window.Activated += OnWindowActivated;
			themeSettings.Changed += OnSystemThemeChanged;
			if (xamlRoot is not null)
			{
				xamlRoot.Changed += OnXamlRootChanged;
			}
		}
		catch
		{
			Dispose();
			throw;
		}
	}

	void UpdateThemeSource()
	{
		var newSource = window.Content as FrameworkElement;
		if (themeSource != newSource)
		{
			if (themeSource is not null)
			{
				themeSource.ActualThemeChanged -= OnActualThemeChanged;
			}

			themeSource = newSource;
			if (themeSource is not null)
			{
				themeSource.ActualThemeChanged += OnActualThemeChanged;
			}
		}

		configuration.Theme = themeSource?.ActualTheme switch
		{
			ElementTheme.Light => SystemBackdropTheme.Light,
			ElementTheme.Dark => SystemBackdropTheme.Dark,
			_ => SystemBackdropTheme.Default
		};
	}

	void OnActualThemeChanged(FrameworkElement sender, object args) => UpdateThemeSource();

	void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args) => UpdateThemeSource();

	void OnWindowActivated(object sender, WindowActivatedEventArgs args) =>
		configuration.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;

	void OnSystemThemeChanged(WindowsThemeSettings sender, object args)
	{
		if (!dispatcherQueue.TryEnqueue(() =>
		{
			if (!disposed)
			{
				configuration.IsHighContrast = themeSettings.HighContrast;
			}
		}))
		{
			System.Diagnostics.Debug.WriteLine("Unable to update the AcrylicThin high contrast configuration: the dispatcher is unavailable.");
		}
	}

	public void Dispose()
	{
		if (disposed)
		{
			return;
		}

		disposed = true;
		window.Activated -= OnWindowActivated;
		themeSettings.Changed -= OnSystemThemeChanged;
		if (xamlRoot is not null)
		{
			xamlRoot.Changed -= OnXamlRootChanged;
		}
		if (themeSource is not null)
		{
			themeSource.ActualThemeChanged -= OnActualThemeChanged;
		}
		controller.Dispose();
	}
}
#endif