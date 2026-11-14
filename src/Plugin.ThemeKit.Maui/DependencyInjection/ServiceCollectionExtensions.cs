namespace Plugin.ThemeKit.Maui;

/// <summary>
/// Extension methods for registering ThemeKit services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers ThemeKit services in the dependency injection container.
	/// </summary>
	/// <param name="services">The service collection to add the ThemeKit services to.</param>
	/// <param name="options">Optional ThemeOptions to configure the ThemeKit services.</param>
	/// <returns>The updated service collection.</returns>
	public static IServiceCollection AddMauiThemeKit(
		this IServiceCollection services,
		ThemeOptions? options = null)
	{
		services.AddSingleton(options ?? new ThemeOptions());
		services.AddSingleton<MauiThemeManager>();

		services.AddSingleton<IThemeManager>(sp => sp.GetRequiredService<MauiThemeManager>());

		return services;
	}
}
