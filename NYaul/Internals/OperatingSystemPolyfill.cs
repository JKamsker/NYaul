using System;

namespace NYaul.Internals;

// OperatingSystem: IsWindows, IsLinux
internal class OperatingSystemPolyfill
{
#if NET6_0_OR_GREATER

    public static bool IsWindows() => OperatingSystem.IsWindows();

    public static bool IsLinux() => OperatingSystem.IsLinux();
#else

    public static bool IsWindows()
    {
        var platform = System.Environment.OSVersion.Platform;
        return platform == PlatformID.Win32NT ||
            platform == PlatformID.Win32S ||
            platform == PlatformID.Win32Windows ||
            platform == PlatformID.WinCE;
    }

    public static bool IsLinux() => System.Environment.OSVersion.Platform == PlatformID.Unix;

#endif
}