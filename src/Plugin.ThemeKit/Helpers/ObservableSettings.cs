using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plugin.ThemeKit.Helpers;

public partial class ObservableSettings(ISettingsProvider provider) : INotifyPropertyChanged
{
	readonly ISettingsProvider provider = provider;

	static Func<string, ISettingsProvider>? providerFactory;

	/// <summary>
	/// Enregistre la méthode de création des providers. 
	/// Appelé une seule fois au démarrage par la bibliothèque dépendante (ex: JSON ou SQLite).
	/// </summary>
	public static void RegisterProviderFactory(Func<string, ISettingsProvider> factory)
	{
		providerFactory = factory;
	}

	/// <summary>
	/// Permet aux classes enfants d'obtenir facilement leur provider via le système centralisé.
	/// </summary>
	protected static ISettingsProvider CreateProviderForFile(string fileName)
	{
		if (providerFactory is null)
		{
			throw new InvalidOperationException(
				"Aucun fournisseur de paramètres n'a été enregistré. " +
				"Veuillez appeler ObservableSettings.RegisterProviderFactory(...) au démarrage."
			);
		}
		return providerFactory(fileName);
	}

	/// <summary>
	/// Notify the UI that a property has changed. This event is raised when a property value is set using the Set method.
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	protected bool Set<T>(T value, [CallerMemberName] string? propertyName = null)
	{
		ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

		if (provider.Contains(propertyName))
		{
			var currentValue = provider.Get<T>(propertyName);
			if (Equals(currentValue, value))
			{
				return false;
			}
		}

		provider.Set(propertyName, value);
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		return true;
	}

	protected T? Get<T>([CallerMemberName] string? propertyName = null)
	{
		ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
		return provider.Get<T>(propertyName);
	}

	protected T GetOrCreateDefault<T>(T defaultValue, [CallerMemberName] string? propertyName = null)
	{
		ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

		if (!provider.Contains(propertyName))
		{
			Set(defaultValue, propertyName);
			return defaultValue;
		}

		return Get<T>(propertyName)!;
	}
}