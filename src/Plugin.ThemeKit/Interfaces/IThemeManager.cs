namespace Plugin.ThemeKit;

/// <summary>
/// Represents a manager for handling themes and backdrops in the application.
/// </summary>
public interface IThemeManager
{
	/// <summary>
	/// Gets the current theme of the application.
	/// </summary>
	Theme CurrentTheme { get; }

	/// <summary>
	/// Gets the current backdrop of the application.
	/// </summary>
	Backdrop CurrentBackdrop { get; }

	/// <summary>
	/// Sets the theme of the application asynchronously.
	/// </summary>
	/// <param name="theme">The theme to set.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	Task SetThemeAsync(Theme theme);
	
	/// <summary>
	/// Sets the backdrop of the application asynchronously.
	/// </summary>
	/// <param name="backdrop">The backdrop to set.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	Task SetBackdropAsync(Backdrop backdrop);

	/// <summary>
	/// Initializes the theme manager asynchronously, preparing it for use.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation.</returns>
	Task InitializeAsync();
}
