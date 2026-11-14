namespace Plugin.ThemeKit.Helpers;

/// <summary>
/// Provides methods to create and manage settings providers for the application.
/// </summary>
public static partial class SettingsProviderFactory
{
	/// <summary>
	/// Gets the default file path for a given file name in the application's data folder.
	/// </summary>
	/// <param name="fileName">The name of the file.</param>
	/// <returns>Returns the full path to the file in the application's data folder.</returns>
	public static string GetDefaultFilePath(string fileName)
	{
		var folder = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), // Mis à jour selon ton code
			ProcessInfoHelper.ProductName
		);

		if (!Directory.Exists(folder))
		{
			Directory.CreateDirectory(folder);
		}

		return Path.Combine(folder, fileName);
	}
}