using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plugin.ThemeKit.Helpers;

/// <summary>
/// Represents a source generation context for JSON serialization and deserialization of settings.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(SettingsHelper))]
public partial class SettingsJsonContext : JsonSerializerContext
{
}
