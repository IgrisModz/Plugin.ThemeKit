namespace Plugin.ThemeKit.Helpers;

public partial class SettingsHelper : ObservableSettings
{
	static readonly Lazy<SettingsHelper> lazyInstance = new(() => new SettingsHelper());
	public static SettingsHelper Current => lazyInstance.Value;

	SettingsHelper() : base(CreateProviderForFile("themeSettings.json"))
	{
	}

	/// <summary>
	/// Gets the selected application theme.
	/// </summary>
	public Theme SelectedAppTheme
	{
		get => GetOrCreateDefault(Theme.System);
		internal set => Set(value);
	}

	/// <summary>
	/// Gets the selected backdrop effect.
	/// </summary>
	public Backdrop SelectedBackdrop
	{
		get => GetOrCreateDefault(Backdrop.Mica);
		internal set => Set(value);
	}
}
