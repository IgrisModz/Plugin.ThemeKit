namespace Plugin.ThemeKit;

/// <summary>
/// Represents the theme settings for the application.
/// </summary>
public sealed class ThemeSettings
{
	/// <summary>
	/// Gets or sets the theme for the application.
	/// </summary>
	public Theme Theme { get; set; } = Theme.System;

	/// <summary>
	/// Gets or sets the backdrop effect for the application.
	/// </summary>
	public Backdrop Backdrop { get; set; } = Backdrop.Mica;
}
