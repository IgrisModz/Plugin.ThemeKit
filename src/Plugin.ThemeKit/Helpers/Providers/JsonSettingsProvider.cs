using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Plugin.ThemeKit.Helpers;

/// <summary>
/// A settings provider that uses a JSON file to store and retrieve settings.
/// </summary>
public partial class JsonSettingsProvider : ISettingsProvider
{
	readonly string filePath;
	readonly JsonSerializerOptions jsonOptions; // On stocke les options globales
	Dictionary<string, JsonElement> values = [];

	/// <summary>
	/// Initializes a new instance of the <see cref="JsonSettingsProvider"/> class.
	/// </summary>
	/// <param name="filePath">The path to the JSON file used to store settings.</param>
	/// <param name="contexts">An array of JSON serializer contexts to use for serialization and deserialization.</param>
	public JsonSettingsProvider(string filePath, params JsonSerializerContext[] contexts)
	{
		this.filePath = filePath;

		// On configure les options pour fusionner le contexte de la bibliothèque ET ceux du client
		jsonOptions = new JsonSerializerOptions { WriteIndented = true };

		var resolvers = new List<IJsonTypeInfoResolver> { SettingsJsonContext.Default };
		if (contexts != null)
		{
			resolvers.AddRange(contexts);
		}

		jsonOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine([.. resolvers]);

		Load();
	}

	/// <summary>
	/// Checks if the settings contain a value for the specified key.
	/// </summary>
	/// <param name="key">The key to check for existence.</param>
	/// <returns>Returns true if the key exists; otherwise, false.</returns>
	public bool Contains(string key) => values.ContainsKey(key);

	/// <summary>
	/// Gets the value associated with the specified key. If the key does not exist, returns null.
	/// </summary>
	/// <param name="key">The key whose value to get.</param>
	/// <returns>Returns the value associated with the specified key, or null if the key does not exist.</returns>
	public object? Get(string key) => values.TryGetValue(key, out var value) ? value : null;

	/// <summary>
	/// Sets the value for the specified key. If the key already exists, its value is updated; otherwise, a new key-value pair is added.
	/// </summary>
	/// <param name="key">The key whose value to set.</param>
	/// <param name="value">The value to set for the specified key.</param>
	public void Set(string key, object value)
	{
		// Utilisation de GetTypeInfo de manière dynamique via les options combinées
		var typeInfo = jsonOptions.GetTypeInfo(value.GetType());
		var json = JsonSerializer.SerializeToElement(value, typeInfo);
		values[key] = json;
		Save();
	}

	/// <summary>
	/// Gets the value associated with the specified key and deserializes it to the specified type. If the key does not exist or if deserialization fails, returns the default value for the type.
	/// </summary>
	/// <typeparam name="T">The type to which to convert the value.</typeparam>
	/// <param name="key">The key whose value to get.</param>
	/// <returns>Returns the value associated with the specified key, converted to the specified type, or null if the key does not exist.</returns>
	public T? Get<T>(string key)
	{
		if (!values.TryGetValue(key, out var jsonElement))
		{
			return default;
		}

		try
		{
			var typeInfo = (JsonTypeInfo<T>)jsonOptions.GetTypeInfo(typeof(T));
			return jsonElement.Deserialize(typeInfo);
		}
		catch (Exception)
		{
			HandleCorruptedKey(key);
			return default;
		}
	}

	/// <summary>
	/// Sets the value for the specified key and serializes it to JSON. If the key already exists, its value is updated; otherwise, a new key-value pair is added.
	/// </summary>
	/// <typeparam name="T">The type of the value to set.</typeparam>
	/// <param name="key">The key whose value to set.</param>
	/// <param name="value">The value to set for the specified key.</param>
	public void Set<T>(string key, T value)
	{
		var typeInfo = (JsonTypeInfo<T>)jsonOptions.GetTypeInfo(typeof(T));
		var json = JsonSerializer.SerializeToElement(value, typeInfo);
		values[key] = json;
		Save();
	}

	void Load()
	{
		if (!File.Exists(filePath))
		{
			return;
		}

		try
		{
			var json = File.ReadAllText(filePath);

			// On utilise GetTypeInfo pour résoudre le dictionnaire via notre résolveur combiné
			var typeInfo = (JsonTypeInfo<Dictionary<string, JsonElement>>)jsonOptions.GetTypeInfo(typeof(Dictionary<string, JsonElement>));

			values = JsonSerializer.Deserialize(json, typeInfo) ?? [];
		}
		catch (Exception)
		{
			HandleCorruptedFile();
		}
	}
	void HandleCorruptedFile()
	{
		try
		{
			File.Delete(filePath);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to delete corrupted settings file '{filePath}': {ex}");
		}

		values = [];
	}

	void HandleCorruptedKey(string key)
	{
		values.Remove(key);
		Save();
	}


	void Save()
	{
		// On utilise GetTypeInfo pour sérialiser le dictionnaire avec notre résolveur combiné
		var typeInfo = (JsonTypeInfo<Dictionary<string, JsonElement>>)jsonOptions.GetTypeInfo(typeof(Dictionary<string, JsonElement>));

		var json = JsonSerializer.Serialize(values, typeInfo);
		File.WriteAllText(filePath, json);
	}
}