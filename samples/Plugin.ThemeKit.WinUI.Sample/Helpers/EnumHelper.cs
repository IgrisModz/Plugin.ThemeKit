using System.Reflection;

namespace Plugin.ThemeKit.WinUI.Sample.Helpers;

static partial class EnumHelper
{
	/// <summary>
	/// Converts a string into an enum.
	/// </summary>
	/// <typeparam name="TEnum">The output enum type.</typeparam>
	/// <param name="text">The input text.</param>
	/// <returns>The parsed enum.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the TEnum type is not a enum.</exception>
	public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
	{
		if (!typeof(TEnum).GetTypeInfo().IsEnum)
		{
			throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
		}
		return Enum.Parse<TEnum>(text);
	}
}
