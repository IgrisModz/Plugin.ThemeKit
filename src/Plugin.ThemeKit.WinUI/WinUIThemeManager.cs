using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Plugin.ThemeKit.Helpers;

namespace Plugin.ThemeKit.WinUI;

/// <summary>
/// A theme manager for WinUI applications that automatically applies the current theme and backdrop to registered windows.
/// </summary>
public sealed class WinUIThemeManager : ThemeManager
{
	readonly HashSet<Window> trackedWindows = [];
	readonly Dictionary<Window, ThinAcrylicBackdrop> thinAcrylicBackdrops = [];

	public WinUIThemeManager(ThemeOptions? options = null) : base(options ?? new ThemeOptions())
	{
		ThemeKitWindowsInitialization.UseWindowsSettings(SettingsJsonContext.Default);
	}

	/// <summary>
	/// Register a window to automatically apply the current theme and backdrop.
	/// </summary>
	public void TrackWindow(Window window)
	{
		if (window == null || !trackedWindows.Add(window))
		{
			return;
		}

		window.Closed += OnWindowClosed;

		// Apply the current theme and backdrop immediately to the new window
		ApplyThemeToWindow(window, CurrentTheme);
		ApplyBackdropToWindow(window, CurrentBackdrop);
	}

	/// <summary>
	/// Unregister a window and clean up its resources.
	/// </summary>
	public void UntrackWindow(Window window)
	{
		if (window == null || !trackedWindows.Remove(window))
		{
			return;
		}

		window.Closed -= OnWindowClosed;

		if (thinAcrylicBackdrops.Remove(window, out var acrylic))
		{
			acrylic.Dispose();
		}

		var previousBackdrop = window.SystemBackdrop;
		window.SystemBackdrop = null;

		if (previousBackdrop is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}

	protected override Task ApplyThemeAsync(Theme theme)
	{
		foreach (var window in trackedWindows)
		{
			ApplyThemeToWindow(window, theme);
		}
		return Task.CompletedTask;
	}

	protected override Task ApplyBackdropAsync(Backdrop backdrop)
	{
		foreach (var window in trackedWindows)
		{
			ApplyBackdropToWindow(window, backdrop);
		}
		return Task.CompletedTask;
	}

	static void ApplyThemeToWindow(Window window, Theme theme)
	{
		if (window.Content is FrameworkElement element)
		{
			element.RequestedTheme = theme switch
			{
				Theme.Light => ElementTheme.Light,
				Theme.Dark => ElementTheme.Dark,
				_ => ElementTheme.Default
			};
		}
	}

	void ApplyBackdropToWindow(Window window, Backdrop backdrop)
	{
		if (thinAcrylicBackdrops.Remove(window, out var previousAcrylic))
		{
			previousAcrylic.Dispose();
		}

		var previousBackdrop = window.SystemBackdrop;
		window.SystemBackdrop = backdrop switch
		{
			Backdrop.Mica => new MicaBackdrop(),
			Backdrop.MicaAlt => new MicaBackdrop { Kind = MicaKind.BaseAlt },
			Backdrop.Acrylic => new DesktopAcrylicBackdrop(),
			_ => null
		};

		if (previousBackdrop is IDisposable disposable)
		{
			disposable.Dispose();
		}

		if (backdrop == Backdrop.AcrylicThin && DesktopAcrylicController.IsSupported())
		{
			thinAcrylicBackdrops.Add(window, new ThinAcrylicBackdrop(window));
		}
	}

	void OnWindowClosed(object sender, WindowEventArgs e)
    {
		if (sender is Window window)
		{
			UntrackWindow(window);
		}
	}
}
