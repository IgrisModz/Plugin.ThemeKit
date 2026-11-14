using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Plugin.ThemeKit.WinUI.Sample.Helpers;

namespace Plugin.ThemeKit.WinUI.Sample;

/// <summary>
/// The application window. This hosts a Frame that displays pages. Add your
/// UI and logic to MainPage.xaml / MainPage.xaml.cs instead of here so you
/// can use Page features such as navigation events and the Loaded lifecycle.
/// </summary>
public sealed partial class MainWindow : Window, INavigationService
{
	public NavigationView? NavigationView => navBar;

	public void SetPaneDisplayMode(NavigationViewPaneDisplayMode mode)
	{
		navBar?.PaneDisplayMode = mode;
	}

	public MainWindow()
	{
		InitializeComponent();

		DispatcherQueue.EnsureSystemDispatcherQueue();

		UpdatePaneToggleButtonVisibility(navBar.DisplayMode);

		navBar.SelectedItem = navBar.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();

		// Hide the default system title bar.
		ExtendsContentIntoTitleBar = true;
		// Replace system title bar with the WinUI TitleBar.
		SetTitleBar(AppTitleBar);
	}

	void RootGrid_Loaded(object sender, RoutedEventArgs e)
	{
		var left = NavigationOrientationHelper.Current.IsLeftMode;
		NavigationOrientationHelper.UpdateNavigationViewForElement(left);
	}

	void NavBar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		Home?.Icon = new FontIcon { Glyph = "\uE80F" };

		if (args.IsSettingsSelected)
		{
			if (rootFrame.CurrentSourcePageType != typeof(SettingsPage))
			{
				Navigate(typeof(SettingsPage));
			}
		}
		else if (args.SelectedItemContainer == Home)
		{
			Home?.Icon = new FontIcon { Glyph = "\uEA8A" };

			if (rootFrame.CurrentSourcePageType != typeof(MainPage))
			{
				Navigate(typeof(MainPage));
			}
		}
	}

	void OnPaneDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
	{
		UpdatePaneToggleButtonVisibility(args.DisplayMode);
	}

	void UpdatePaneToggleButtonVisibility(NavigationViewDisplayMode displayMode)
	{
		if (navBar.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
		{
			AppTitleBar.IsPaneToggleButtonVisible = false;
			return;
		}

		var isNotExpanded = displayMode != NavigationViewDisplayMode.Expanded;

		AppTitleBar.IsPaneToggleButtonVisible = isNotExpanded;
	}

	void TitleBar_PaneToggleRequested(TitleBar sender, object args)
	{
		navBar.IsPaneOpen = !navBar.IsPaneOpen;
	}

	void TitleBar_BackRequested(TitleBar sender, object args)
	{
		if (rootFrame.CanGoBack)
		{
			rootFrame.GoBack();
		}
	}

	public void Navigate(Type pageType, object? targetPageArguments = null, NavigationTransitionInfo? navigationTransitionInfo = null)
	{
		rootFrame.Navigate(pageType, targetPageArguments, navigationTransitionInfo);
	}
}