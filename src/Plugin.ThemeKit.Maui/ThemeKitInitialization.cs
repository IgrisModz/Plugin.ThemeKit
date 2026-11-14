using System.Text.Json.Serialization;
using Plugin.ThemeKit.Helpers;
using Plugin.ThemeKit.Maui.Helpers.Providers;

namespace Plugin.ThemeKit.Maui;

/// <summary>
/// Provides initialization methods for the ThemeKit library in Windows applications, allowing for the registration of JSON serialization contexts and configuration of settings provider factories based on the application's packaging state.
/// </summary>
public static class ThemeKitInitialization
{
	static readonly List<JsonSerializerContext> registeredContexts = [];
	static readonly Lock @lock = new();
	static bool isFactoryRegistered = false;

	/// <summary>
	/// Registers the specified JSON serialization contexts for use with the ThemeKit library and configures the settings provider factory based on the application's packaging state. This method ensures that the contexts are registered only once and that the appropriate settings provider is used depending on whether the application is packaged or not.
	/// </summary>
	/// <param name="contexts">The JSON serialization contexts to register.</param>
	public static void UseMauiSettings(params JsonSerializerContext[] contexts)
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
					return new ApplicationDataSettingsProvider(null, [.. registeredContexts]);
				});

				isFactoryRegistered = true;
			}
		}
	}
}