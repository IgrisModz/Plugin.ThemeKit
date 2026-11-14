using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plugin.ThemeKit.WinUI.Sample.Helpers;

[JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(NavigationOrientationHelper))]
partial class NavigationOrientationJsonContext : JsonSerializerContext
{
}
