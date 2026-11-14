using System.Text.Json.Serialization;

namespace Plugin.ThemeKit.Maui;

public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseMauiThemeKit(
		this MauiAppBuilder builder,
		ThemeOptions? options = null,
		params JsonSerializerContext[] contexts)
	{
		// Enregistrement des services
		builder.Services.AddMauiThemeKit(options);

		// Pré-enregistrement des contextes JSON pour les préférences
		ThemeKitInitialization.UseMauiSettings(contexts);

		return builder;
	}
}
