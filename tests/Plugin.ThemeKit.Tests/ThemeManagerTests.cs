namespace Plugin.ThemeKit.Tests;

[TestClass]
public sealed class ThemeManagerTests
{
	[TestMethod]
	public void Constructor_UsesConfiguredDefaultThemeAndBackdrop()
	{
		var manager = new RecordingThemeManager(new ThemeOptions
		{
			DefaultTheme = Theme.Light,
			DefaultBackdrop = Backdrop.None
		});

		Assert.AreEqual(Theme.Light, manager.CurrentTheme);
		Assert.AreEqual(Backdrop.None, manager.CurrentBackdrop);
	}

	[TestMethod]
	public async Task InitializeAsync_AppliesConfiguredDefaults_WhenRestoreIsDisabled()
	{
		var manager = new RecordingThemeManager(new ThemeOptions
		{
			DefaultTheme = Theme.Dark,
			DefaultBackdrop = Backdrop.Acrylic,
			RestoreOnStartup = false,
			SaveTheme = false,
			SaveBackdrop = false
		});

		await manager.InitializeAsync();

		CollectionAssert.AreEqual(new[] { Theme.Dark }, manager.AppliedThemes);
		CollectionAssert.AreEqual(new[] { Backdrop.Acrylic }, manager.AppliedBackdrops);
	}

	[TestMethod]
	public async Task SetThemeAsync_UpdatesCurrentThemeAndAppliesTheme()
	{
		var manager = new RecordingThemeManager();

		await manager.SetThemeAsync(Theme.Dark);

		Assert.AreEqual(Theme.Dark, manager.CurrentTheme);
		CollectionAssert.AreEqual(new[] { Theme.Dark }, manager.AppliedThemes);
	}

	[TestMethod]
	public async Task SetBackdropAsync_UpdatesCurrentBackdropAndAppliesBackdrop()
	{
		var manager = new RecordingThemeManager();

		await manager.SetBackdropAsync(Backdrop.MicaAlt);

		Assert.AreEqual(Backdrop.MicaAlt, manager.CurrentBackdrop);
		CollectionAssert.AreEqual(new[] { Backdrop.MicaAlt }, manager.AppliedBackdrops);
	}

	sealed class RecordingThemeManager(ThemeOptions? options = null) : ThemeManager(options ?? new ThemeOptions { SaveTheme = false, SaveBackdrop = false })
	{
		public List<Theme> AppliedThemes { get; } = [];

		public List<Backdrop> AppliedBackdrops { get; } = [];

		protected override Task ApplyThemeAsync(Theme theme)
		{
			AppliedThemes.Add(theme);
			return Task.CompletedTask;
		}

		protected override Task ApplyBackdropAsync(Backdrop backdrop)
		{
			AppliedBackdrops.Add(backdrop);
			return Task.CompletedTask;
		}
	}
}
