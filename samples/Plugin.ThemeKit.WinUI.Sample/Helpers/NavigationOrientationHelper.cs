using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Plugin.ThemeKit.Helpers;

namespace Plugin.ThemeKit.WinUI.Sample.Helpers;

public partial class NavigationOrientationHelper : ObservableSettings
{
	static readonly Lazy<NavigationOrientationHelper> instance = new(() => new NavigationOrientationHelper());
	public static NavigationOrientationHelper Current => instance.Value;

	NavigationOrientationHelper() : base(CreateProviderForFile("navbar.json"))
	{
	}

	public bool IsLeftMode
	{
		get => GetOrCreateDefault(true);
		private set => Set(value);
	}

	public static void IsLeftModeForElement(bool isLeftMode)
	{
		UpdateNavigationViewForElement(isLeftMode);
		Current.IsLeftMode = isLeftMode;
	}

	public static void UpdateNavigationViewForElement(bool isLeftMode)
	{
		var navService = App.Services.GetService<INavigationService>();
		if (navService?.NavigationView is null)
		{
			return;
		}

		var mode = isLeftMode ? NavigationViewPaneDisplayMode.Auto : NavigationViewPaneDisplayMode.Top;
		navService.SetPaneDisplayMode(mode);
	}
}
