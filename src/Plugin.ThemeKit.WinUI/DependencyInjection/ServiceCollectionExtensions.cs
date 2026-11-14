using Microsoft.Extensions.DependencyInjection;

namespace Plugin.ThemeKit.WinUI;

/// <summary>
/// Extension methods for registering WinUI Theme Kit services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds the WinUI Theme Kit services to the dependency injection container.
	/// </summary>
	/// <param name="services">The service collection to add the services to.</param>
	/// <param name="options">The theme options to configure the theme behavior.</param>
	/// <returns>The updated service collection.</returns>
	public static IServiceCollection AddWinUIThemeKit(
		this IServiceCollection services,
		ThemeOptions? options = null)
	{
		services.AddSingleton(options ?? new ThemeOptions());
		services.AddSingleton<WinUIThemeManager>();

		services.AddSingleton<IThemeManager>(sp => sp.GetRequiredService<WinUIThemeManager>());

		return services;
	}
}
