using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Plugin.ThemeKit.Helpers;

namespace Plugin.ThemeKit.Maui.Helpers.Providers;

/// <summary>
/// A settings provider that uses the application data storage to persist settings.
/// </summary>
public partial class ApplicationDataSettingsProvider : ISettingsProvider
{
	readonly IPreferences preferences;
	readonly JsonSerializerOptions jsonOptions;

	/// <summary>
	/// Initializes a new instance of the <see cref="ApplicationDataSettingsProvider"/> class.
	/// </summary>
	/// <param name="preferences">The preferences instance to use for storing settings.</param>
	/// <param name="contexts">Optional JSON serializer contexts for custom serialization.</param>
	public ApplicationDataSettingsProvider(IPreferences? preferences = null, params JsonSerializerContext[] contexts)
	{
		this.preferences = preferences ?? Preferences.Default;

		jsonOptions = new JsonSerializerOptions();
		var resolvers = new List<IJsonTypeInfoResolver> { SettingsJsonContext.Default };
		if (contexts != null)
		{
			resolvers.AddRange(contexts);
		}
		jsonOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine([.. resolvers]);
	}

	/// <summary>
	/// Checks if the specified key exists in the settings.
	/// </summary>
	/// <param name="key">The key to check for existence.</param>
	/// <returns>True if the key exists; otherwise, false.</returns>
	public bool Contains(string key) => preferences.ContainsKey(key);

	/// <summary>
	/// Removes the specified key and its associated value from the settings.
	/// </summary>
	/// <param name="key">The key to remove.</param>
	/// <returns>True if the key was removed; otherwise, false.</returns>
	public object? Get(string key) => preferences.Get<object?>(key, null);

	/// <summary>
	/// Sets the value for the specified key in the settings. If the value is a simple type, it is stored directly; otherwise, it is serialized to JSON.
	/// </summary>
	/// <param name="key">The key to set the value for.</param>
	/// <param name="value">The value to set.</param>
	public void Set(string key, object value)
	{
		if (IsSimpleType(value.GetType()))
		{
			SetSimpleValue(key, value);
		}
		else
		{
			var typeInfo = jsonOptions.GetTypeInfo(value.GetType());
			preferences.Set(key, JsonSerializer.Serialize(value, typeInfo));
		}
	}

	/// <summary>
	/// Gets the value associated with the specified key and attempts to deserialize it to the specified type. If the key does not exist or deserialization fails, the default value for the type is returned.
	/// </summary>
	/// <typeparam name="T">The type to deserialize the value to.</typeparam>
	/// <param name="key">The key of the value to retrieve.</param>
	/// <returns>The deserialized value if successful; otherwise, the default value for the type.</returns>
	public T? Get<T>(string key)
	{
		if (!preferences.ContainsKey(key))
		{
			return default;
		}

		if (IsSimpleType(typeof(T)))
		{
			return preferences.Get<T>(key, default!);
		}

		string? jsonStr = preferences.Get<string?>(key, null);
		if (jsonStr != null)
		{
			try
			{
				var typeInfo = (JsonTypeInfo<T>)jsonOptions.GetTypeInfo(typeof(T));
				return JsonSerializer.Deserialize(jsonStr, typeInfo);
			}
			catch (Exception)
			{
				HandleCorruptedKey(key);
				return default;
			}
		}

		return default;
	}

	/// <summary>
	/// Sets the value for the specified key in the settings, serializing it to JSON if it is not a simple type.
	/// </summary>
	/// <typeparam name="T">The type of the value to set.</typeparam>
	/// <param name="key">The key to set the value for.</param>
	/// <param name="value">The value to set.</param>
	public void Set<T>(string key, T value)
	{
		if (IsSimpleType(typeof(T)))
		{
			preferences.Set(key, value);
		}
		else
		{
			var typeInfo = (JsonTypeInfo<T>)jsonOptions.GetTypeInfo(typeof(T));
			string json = JsonSerializer.Serialize(value, typeInfo);
			preferences.Set(key, json);
		}
	}

	void HandleCorruptedKey(string key)
	{
		try
		{
			preferences.Remove(key);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to remove corrupted key '{key}': {ex}");
		}
	}

	void SetSimpleValue(string key, object value)
	{
		switch (value)
		{
			case string s: preferences.Set(key, s); break;
			case int i: preferences.Set(key, i); break;
			case bool b: preferences.Set(key, b); break;
			case long l: preferences.Set(key, l); break;
			case double d: preferences.Set(key, d); break;
			case float f: preferences.Set(key, f); break;
			case DateTime dt: preferences.Set(key, dt); break;
			default: preferences.Set(key, value.ToString()); break;
		}
	}

	static readonly HashSet<Type> extraSimpleTypes =
	[
		typeof(string),
		typeof(DateTime),
		typeof(DateTimeOffset),
		typeof(TimeSpan),
		typeof(Guid)
	];

	static bool IsSimpleType(Type type) => type.IsPrimitive || extraSimpleTypes.Contains(type);
}
