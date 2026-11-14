using System.Text.Json.Serialization;
using Microsoft.Windows.Storage;
using Plugin.ThemeKit.Helpers;

namespace Plugin.ThemeKit.WinUI;

/// <summary>
/// Initializes the ThemeKit library for Windows applications, providing support for both packaged (MSIX / WinUI 3 Packaged) and unpackaged (Win32 classic) modes. This class allows you to register JSON serialization contexts and configure the settings provider factory based on the application's packaging state.
/// </summary>
public static class ThemeKitWindowsInitialization
{
	static readonly List<JsonSerializerContext> registeredContexts = [];
	static readonly Lock @lock = new();
	static bool isFactoryRegistered = false;

	/// <summary>
	/// Initializes the ThemeKit library for Windows applications, registering the provided JSON serialization contexts and configuring the settings provider factory based on the application's packaging state. This method should be called during application startup to ensure proper initialization of the ThemeKit library.
	/// </summary>
	/// <param name="contexts">The JSON serialization contexts to register.</param>
	public static void UseWindowsSettings(params JsonSerializerContext[] contexts)
	{
		lock (@lock)
		{
			if (contexts is not null)
			{
				foreach (var context in contexts)
				{
					if (!registeredContexts.Contains(context))
					{
						registeredContexts.Add(context);
					}
				}
			}

			if (!isFactoryRegistered)
			{
				ObservableSettings.RegisterProviderFactory(fileName =>
				{
					// 1. Détection du mode Packagé (MSIX / WinUI 3 Packaged)
					if (NativeMethods.IsAppPackaged)
					{
						// Ici, ApplicationData et ApplicationDataSettingsProvider sont parfaitement accessibles
						return new ApplicationDataSettingsProvider(ApplicationData.GetDefault().LocalSettings);
					}

					// 2. Fallback pour le mode non-packagé (Win32 classique / Unpackaged)
					string filePath = SettingsProviderFactory.GetDefaultFilePath(fileName);

					// On instancie le provider JSON avec les contextes passés à l'initialisation
					return new JsonSettingsProvider(filePath, [.. registeredContexts]);
				});
				
				isFactoryRegistered = true;
			}
		}
	}
}