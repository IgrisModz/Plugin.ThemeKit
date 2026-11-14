namespace Plugin.ThemeKit;

public enum Backdrop
{
	/// <summary>
	/// No backdrop effect is applied.
	/// </summary>
	None = 0,

	/// <summary>
	/// Applies the Mica backdrop effect, which is a translucent material that incorporates the user's desktop wallpaper and theme colors.
	/// </summary>
	Mica = 1,

	/// <summary>
	/// Applies the Mica Alt backdrop effect, which is a variation of the Mica effect with a different level of translucency and color blending.
	/// </summary>
	MicaAlt = 2,

    /// <summary>
    /// Applies the Acrylic backdrop effect, which is a translucent material that blurs the background content.
    /// </summary>
    Acrylic = 3,

	/// <summary>
	/// Applies the Acrylic Thin backdrop effect, which is a variation of the Acrylic effect with a thinner blur and more transparency.
	/// </summary>
	AcrylicThin = 4
}
