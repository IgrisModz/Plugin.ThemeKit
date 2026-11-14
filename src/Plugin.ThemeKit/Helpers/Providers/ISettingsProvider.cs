namespace Plugin.ThemeKit.Helpers;

/// <summary>
/// Represents a provider for application settings.
/// </summary>
public interface ISettingsProvider
{
	/// <summary>
	/// Determines whether the specified key exists in the settings.
	/// </summary>
	/// <param name="key">The key to check for existence.</param>
	/// <returns>Returns true if the key exists; otherwise, false.</returns>
	bool Contains(string key);

	/// <summary>
	/// Gets the value associated with the specified key.
	/// </summary>
	/// <param name="key">The key whose value to get.</param>
	/// <returns>Returns the value associated with the specified key, or null if the key does not exist.</returns>
	object? Get(string key);

	/// <summary>
	/// Sets the value for the specified key in the settings.
	/// </summary>
	/// <param name="key">The key whose value to set.</param>
	/// <param name="value">The value to set for the specified key.</param>
	void Set(string key, object value);

	/// <summary>
	/// Gets the value associated with the specified key and converts it to the specified type.
	/// </summary>
	/// <typeparam name="T">The type to which to convert the value.</typeparam>
	/// <param name="key">The key whose value to get.</param>
	/// <returns>Returns the value associated with the specified key, converted to the specified type, or null if the key does not exist.</returns>
	T? Get<T>(string key);

	/// <summary>
	/// Sets the value for the specified key in the settings, converting it to the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the value to set.</typeparam>
	/// <param name="key">The key whose value to set.</param>
	/// <param name="value">The value to set for the specified key.</param>
	void Set<T>(string key, T value);
}