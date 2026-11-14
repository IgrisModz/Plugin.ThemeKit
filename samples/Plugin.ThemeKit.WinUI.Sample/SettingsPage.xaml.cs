using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Plugin.ThemeKit.WinUI.Sample.Helpers;

namespace Plugin.ThemeKit.WinUI.Sample;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
	int lastNavigationSelectionMode = 0;

	public SettingsPage()
	{
		InitializeComponent();
		Loaded += OnLoaded;
	}

	void OnLoaded(object sender, RoutedEventArgs e)
	{
		var currentTheme = App.ThemeManager?.CurrentTheme ?? Theme.System;
		themeMode.SelectedIndex = currentTheme switch
		{
			Theme.Light => 1,
			Theme.Dark => 2,
			_ => 0
		};

		var currentBackdrop = App.ThemeManager?.CurrentBackdrop ?? Backdrop.None;
		backdropMode.SelectedIndex = currentBackdrop switch
		{
			Backdrop.None => 0,
			Backdrop.Mica => 1,
			Backdrop.MicaAlt => 2,
			Backdrop.Acrylic => 3,
			Backdrop.AcrylicThin => 4,
			_ => 1
		};

		var navService = App.Services.GetRequiredService<INavigationService>();

		if (navService.NavigationView != null)
		{
			navigationLocation.SelectedIndex =
				navService.NavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto ? 0 : 1;

			lastNavigationSelectionMode = navigationLocation.SelectedIndex;
		}
	}

	async void OnThemeModeSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is not UIElement ||
			(themeMode.SelectedItem as ComboBoxItem)?.Tag.ToString() is not string selectedTheme ||
			App.ThemeManager is null)
		{
			return;
		}

		await App.ThemeManager.SetThemeAsync(EnumHelper.GetEnum<Theme>(selectedTheme));
	}

	async void OnBackdropModeSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is not UIElement ||
			(backdropMode.SelectedItem as ComboBoxItem)?.Tag.ToString() is not string selectedBackdrop ||
			App.ThemeManager is null)
		{
			return;
		}

		await App.ThemeManager.SetBackdropAsync(EnumHelper.GetEnum<Backdrop>(selectedBackdrop));
	}

	void OnNavigationLocationSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		// Since setting the left mode does not look at the old setting we 
		// need to check if this is an actual update
		if (navigationLocation.SelectedIndex != lastNavigationSelectionMode)
		{
			NavigationOrientationHelper.IsLeftModeForElement(navigationLocation.SelectedIndex == 0);
			lastNavigationSelectionMode = navigationLocation.SelectedIndex;
		}
	}
}
