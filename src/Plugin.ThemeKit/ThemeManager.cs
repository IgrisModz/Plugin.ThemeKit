using Plugin.ThemeKit.Helpers;

namespace Plugin.ThemeKit;

public abstract class ThemeManager : IThemeManager
{
	ThemeSettings settings;

	protected ThemeOptions Options { get; }

	protected ThemeManager(ThemeOptions options)
	{
		Options = options;
		settings = new ThemeSettings
		{
			Theme = options.DefaultTheme,
			Backdrop = options.DefaultBackdrop
		};
	}

	public Theme CurrentTheme => settings.Theme;

	public Backdrop CurrentBackdrop => settings.Backdrop;

	public async Task InitializeAsync()
	{
		if (Options.RestoreOnStartup)
		{
			settings = new ThemeSettings
			{
				Theme = SettingsHelper.Current.SelectedAppTheme,
				Backdrop = SettingsHelper.Current.SelectedBackdrop
			};
		}

		await ApplyThemeAsync(settings.Theme);
		await ApplyBackdropAsync(settings.Backdrop);
	}

	public async Task SetThemeAsync(Theme theme)
	{
		settings.Theme = theme;
		await ApplyThemeAsync(theme);
		if (Options.SaveTheme)
		{
			SettingsHelper.Current.SelectedAppTheme = theme;
		}
	}

	public async Task SetBackdropAsync(Backdrop backdrop)
	{
		settings.Backdrop = backdrop;
		await ApplyBackdropAsync(backdrop);
		if (Options.SaveBackdrop)
		{
			SettingsHelper.Current.SelectedBackdrop = backdrop;
		}
	}

	protected abstract Task ApplyThemeAsync(Theme theme);

	protected abstract Task ApplyBackdropAsync(Backdrop backdrop);
}

