using Plugin.ThemeKit.Helpers;

#if WINDOWS
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#endif

namespace Plugin.ThemeKit.Maui;

/// <summary>
/// A theme manager implementation for .NET MAUI applications.
/// </summary>
public sealed class MauiThemeManager : ThemeManager
{
#if WINDOWS
	readonly Dictionary<Microsoft.UI.Xaml.Window, ThinAcrylicBackdrop> thinAcrylicBackdrops = [];
#endif

	/// <summary>
	/// Initializes a new instance of the <see cref="MauiThemeManager"/> class with the specified options.
	/// </summary>
	/// <param name="options">The theme options to use.</param>
	public MauiThemeManager(ThemeOptions? options = null) : base(options ?? new ThemeOptions())
	{
		ThemeKitInitialization.UseMauiSettings(SettingsJsonContext.Default);
	}

	protected override Task ApplyThemeAsync(Theme theme)
	{
		return MainThread.InvokeOnMainThreadAsync(() =>
		{
			Microsoft.Maui.Controls.Application.Current?.UserAppTheme = theme switch
			{
				Theme.Light => AppTheme.Light,
				Theme.Dark => AppTheme.Dark,
				_ => AppTheme.Unspecified
			};
		});
	}

	protected override Task ApplyBackdropAsync(Backdrop backdrop)
	{
		return MainThread.InvokeOnMainThreadAsync(() =>
		{
#if WINDOWS
			if (Microsoft.Maui.Controls.Application.Current is null)
			{
				return;
			}

			foreach (var mauiWindow in Microsoft.Maui.Controls.Application.Current.Windows)
			{
				var nativeWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

				if (nativeWindow is not null)
				{
					ApplyBackdropToNativeWindow(nativeWindow, backdrop);
				}
			}
#endif
		});
	}

#if WINDOWS
	void ApplyBackdropToNativeWindow(Microsoft.UI.Xaml.Window window, Backdrop backdrop)
	{
		if (thinAcrylicBackdrops.Remove(window, out var previousAcrylic))
		{
			window.Closed -= OnThinAcrylicWindowClosed;
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
			window.Closed += OnThinAcrylicWindowClosed;
		}
	}

	void OnThinAcrylicWindowClosed(object sender, WindowEventArgs args)
	{
		if (sender is Microsoft.UI.Xaml.Window window && thinAcrylicBackdrops.Remove(window, out var acrylic))
		{
			window.Closed -= OnThinAcrylicWindowClosed;
			acrylic.Dispose();
		}
	}
#endif
}
