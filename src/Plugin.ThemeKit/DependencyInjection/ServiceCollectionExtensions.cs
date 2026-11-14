using Microsoft.Extensions.DependencyInjection;

namespace Plugin.ThemeKit;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddThemeKit<TThemeManager>(
		this IServiceCollection services,
		ThemeOptions? options = null)
		where TThemeManager : class, IThemeManager
	{
		services.AddSingleton(options ?? new ThemeOptions());
		services.AddSingleton<IThemeManager, TThemeManager>();
		return services;
	}
}

