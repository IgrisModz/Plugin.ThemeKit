using System.Diagnostics;

namespace Plugin.ThemeKit.Helpers;

public static partial class ProcessInfoHelper
{
	static readonly FileVersionInfo? fileVersionInfo;
	static readonly Process process;
	static ProcessInfoHelper()
	{
		process = Process.GetCurrentProcess();
		fileVersionInfo = process.MainModule?.FileVersionInfo;
	}

	/// <summary>
	/// Returns the version string.
	/// </summary>
	public static string Version => GetVersion()?.ToString() ?? string.Empty;

	/// <summary>
	/// Returns the version string prefixed with 'v'.
	/// </summary>
	public static string VersionWithPrefix => $"v{Version}";

	/// <summary>
	/// Retrieves the product name. If not available, it returns 'Unknown Product'.
	/// </summary>
	public static string ProductName => fileVersionInfo?.ProductName ?? "Unknown Product";

	/// <summary>
	/// Combines the product name and version into a single string. The version includes a prefix.
	/// </summary>
	public static string ProductNameAndVersion => $"{ProductName} {VersionWithPrefix}";

	/// <summary>
	/// Returns the company name of the publisher. If not available, it defaults to 'Unknown Publisher'.
	/// </summary>
	public static string Publisher => fileVersionInfo?.CompanyName ?? "Unknown Publisher";

	/// <summary>
	/// Retrieves the version of the current assembly as a Version object. If the version information is not available, it returns null.
	/// </summary>
	/// <returns>Returns a Version object representing the assembly version, or null if not available.</returns>
	public static Version? GetVersion()
	{
		return fileVersionInfo is null
			? null
			: new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
	}

	/// <summary>
	/// Retrieves the file version information for the current assembly.
	/// </summary>
	/// <returns>Returns a FileVersionInfo object containing version details.</returns>
	public static FileVersionInfo? GetFileVersionInfo()
	{
		return fileVersionInfo;
	}

	/// <summary>
	/// Retrieves the current process instance.
	/// </summary>
	/// <returns>Returns the current Process object.</returns>
	public static Process GetProcess()
	{
		return process;
	}
}