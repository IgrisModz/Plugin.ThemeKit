using System.Runtime.Versioning;
using Windows.Win32.Foundation;

namespace Plugin.ThemeKit;

[SupportedOSPlatform("windows8.0")]
public partial class NativeMethods
{
	public static bool IsAppPackaged { get; } = CheckIsPackaged();

	static bool CheckIsPackaged()
	{
		unsafe
		{
			uint packageFullNameLength = 0;

			var result = Windows.Win32.PInvoke.GetCurrentPackageFullName(&packageFullNameLength, null);

			return result == WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER;
		}
	}

	static string? GetCurrentPackageFullName()
	{
		unsafe
		{
			uint packageFullNameLength = 0;
			var result = Windows.Win32.PInvoke.GetCurrentPackageFullName(&packageFullNameLength, null);

			if (result == WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
			{
				char* packageFullName = stackalloc char[(int)packageFullNameLength];
				result = Windows.Win32.PInvoke.GetCurrentPackageFullName(&packageFullNameLength, packageFullName);


				if (result != WIN32_ERROR.ERROR_SUCCESS)
				{
					// -1 to ignore the null terminator '\0' specific to C++
					return new string(packageFullName, 0, (int)packageFullNameLength - 1);
				}
			}
		}

		return null;
	}
}
