using Microsoft.Windows.Storage;
using Plugin.ThemeKit.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Plugin.ThemeKit.WinUI;

/// <summary>
/// A settings provider that uses the Windows.Storage.ApplicationDataContainer to store and retrieve settings.
/// </summary>
public partial class ApplicationDataSettingsProvider : ISettingsProvider
{
	readonly ApplicationDataContainer container;
	readonly JsonSerializerOptions jsonOptions;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ApplicationDataSettingsProvider"/> class.
	/// </summary>
	/// <param name="container">The application data container to use for storing settings.</param>
	/// <param name="contexts">Optional JSON serializer contexts for custom serialization.</param>
	public ApplicationDataSettingsProvider(ApplicationDataContainer container, params JsonSerializerContext[] contexts)
	{
		ArgumentNullException.ThrowIfNull(container, nameof(container));
		this.container = container;

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
	public bool Contains(string key) => container.Values.ContainsKey(key);

	/// <summary>
	/// Gets the value associated with the specified key from the settings.
	/// </summary>
	/// <param name="key">The key whose value to get.</param>
	/// <returns>The value associated with the specified key, or null if the key does not exist.</returns>
	public object? Get(string key) => container.Values.TryGetValue(key, out var value) ? value : null;

	/// <summary>
	/// Sets the value for the specified key in the settings. If the value is a simple type, it is stored directly; otherwise, it is serialized to JSON.
	/// </summary>
	/// <param name="key">The key whose value to set.</param>
	/// <param name="value">The value to set for the specified key.</param>
	public void Set(string key, object value)
	{
		if (IsSimpleType(value.GetType()))
		{
			container.Values[key] = value;
		}
		else
		{
			var typeInfo = jsonOptions.GetTypeInfo(value.GetType());
			container.Values[key] = JsonSerializer.Serialize(value, typeInfo);
		}
	}

	/// <summary>
	/// Gets the value associated with the specified key from the settings, deserialized to the specified type. If the key does not exist or the value cannot be deserialized, returns the default value for the type.
	/// </summary>
	/// <typeparam name="T">The type to which the value should be deserialized.</typeparam>
	/// <param name="key">The key whose value to get.</param>
	/// <returns>The value associated with the specified key, deserialized to the specified type, or the default value for the type if the key does not exist or the value cannot be deserialized.</returns>
	public T? Get<T>(string key)
	{
		if (!container.Values.TryGetValue(key, out var value))
		{
			return default;
		}

		if (value is T t)
		{
			return t;
		}

		if (value is string str && !IsSimpleType(typeof(T)))
		{
			try
			{
				var typeInfo = (JsonTypeInfo<T>)jsonOptions.GetTypeInfo(typeof(T));
				return JsonSerializer.Deserialize(str, typeInfo);
			}
			catch (Exception)
			{
				HandleCorruptedKey(key);
				return default;
			}
		}

		return (T)Convert.ChangeType(value, typeof(T));
	}

	/// <summary>
	/// Sets the value for the specified key in the settings, serialized to JSON if it is not a simple type.
	/// </summary>
	/// <typeparam name="T">The type of the value to set.</typeparam>
	/// <param name="key">The key whose value to set.</param>
	/// <param name="value">The value to set for the specified key.</param>
	public void Set<T>(string key, T value)
	{
		if (IsSimpleType(typeof(T)))
		{
			container.Values[key] = value!;
		}
		else
		{
			var typeInfo = (JsonTypeInfo<T>)jsonOptions.GetTypeInfo(typeof(T));
			container.Values[key] = JsonSerializer.Serialize(value, typeInfo);
		}
	}

	void HandleCorruptedKey(string key)
	{
		try
		{
			container.Values.Remove(key);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to remove corrupted key '{key}': {ex}");
		}
	}

	static readonly HashSet<Type> extraSimpleTypes =
	[
		typeof(string),
		typeof(DateTimeOffset),
		typeof(TimeSpan),
		typeof(Guid),
		typeof(Windows.Foundation.Point),
		typeof(Windows.Foundation.Size),
		typeof(Windows.Foundation.Rect)
	];

	static bool IsSimpleType(Type type) => type.IsPrimitive || extraSimpleTypes.Contains(type);
}
