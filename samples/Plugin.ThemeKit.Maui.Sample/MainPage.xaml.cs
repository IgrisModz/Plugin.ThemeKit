namespace Plugin.ThemeKit.Maui.Sample;

public partial class MainPage : ContentPage
{
	readonly MauiThemeManager themeManager;

	public MainPage(MauiThemeManager themeManager)
	{
		InitializeComponent();
		this.themeManager = themeManager;

		Loaded += OnLoaded;
	}

	async void OnLoaded(object? sender, EventArgs e)
	{
		await themeManager.InitializeAsync();

		var currentTheme = themeManager.CurrentTheme;
		ThemePicker.SelectedIndex = currentTheme switch
		{
			Theme.Light => 1,
			Theme.Dark => 2,
			_ => 0
		};

		var currentBackdrop = themeManager.CurrentBackdrop;
		BackdropPicker.SelectedIndex = currentBackdrop switch
		{
			Backdrop.None => 0,
			Backdrop.Mica => 1,
			Backdrop.MicaAlt => 2,
			Backdrop.Acrylic => 3,
			Backdrop.AcrylicThin => 4,
			_ => 1
		};
	}

	async void ThemePicker_SelectedIndexChanged(object? sender, EventArgs e)
	{
		if (sender is not Picker picker || picker.SelectedItem is not string selectedTheme)
		{
			return;
		}

		switch (selectedTheme)
		{
			case "System":
				await themeManager.SetThemeAsync(Theme.System);
				break;
			case "Light":
				await themeManager.SetThemeAsync(Theme.Light);
				break;
			case "Dark":
				await themeManager.SetThemeAsync(Theme.Dark);
				break;
		}
	}

	async void BackdropPicker_SelectedIndexChanged(object? sender, EventArgs e)
	{
		if (sender is not Picker picker || picker.SelectedItem is not string selectedTheme)
		{
			return;
		}

		switch (selectedTheme)
		{
			case "None":
				await themeManager.SetBackdropAsync(Backdrop.None);
				break;
			case "Mica":
				await themeManager.SetBackdropAsync(Backdrop.Mica);
				break;
			case "Mica Alt":
				await themeManager.SetBackdropAsync(Backdrop.MicaAlt);
				break;
			case "Acrylic":
				await themeManager.SetBackdropAsync(Backdrop.Acrylic);
				break;
			case "Acrylic Thin":
				await themeManager.SetBackdropAsync(Backdrop.AcrylicThin);
				break;
		}
	}
}
