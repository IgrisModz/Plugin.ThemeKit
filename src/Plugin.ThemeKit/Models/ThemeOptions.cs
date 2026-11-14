namespace Plugin.ThemeKit;

/// <summary>
/// Represents the options for configuring the theme behavior in the application.
/// </summary>
public sealed class ThemeOptions
{
	/// <summary>
	/// Gets or sets a value indicating whether the theme should be saved when the application is closed.
	/// </summary>
	public bool SaveTheme { get; init; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the backdrop should be saved when the application is closed.
	/// </summary>
	public bool SaveBackdrop { get; init; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the theme should be restored when the application starts.
	/// </summary>
	public bool RestoreOnStartup { get; init; } = true;

	/// <summary>
	/// Gets or sets the default theme to be used when the application starts.
	/// </summary>
	public Theme DefaultTheme { get; init; } = Theme.System;

	/// <summary>
	/// Gets or sets the default backdrop to be used when the application starts.
	/// </summary>
	public Backdrop DefaultBackdrop { get; init; } = Backdrop.Mica;
}