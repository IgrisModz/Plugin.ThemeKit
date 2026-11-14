using Microsoft.UI.Xaml.Controls;

namespace Plugin.ThemeKit.WinUI.Sample;

public interface INavigationService
{
	NavigationView? NavigationView { get; }
	void SetPaneDisplayMode(NavigationViewPaneDisplayMode mode);
}
