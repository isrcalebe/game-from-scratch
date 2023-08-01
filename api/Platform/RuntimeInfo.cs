/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Diagnostics;
using System.Reflection;

namespace miniblocks.API.Platform;

public static class RuntimeInfo
{
    public static string StartupDirectory { get; }
        = AppContext.BaseDirectory;

    public static string GetAPIAssemblyPath()
    {
        var assembly = Assembly.GetAssembly(typeof(RuntimeInfo));
        Debug.Assert(assembly != null);

        return assembly.Location;
    }

    public static Platform OS { get; }

    public static bool IsUnix => OS != Platform.Windows;
    public static bool IsDesktop => OS == Platform.Windows || OS == Platform.Linux || OS == Platform.MacOS;
    public static bool IsApple => OS == Platform.MacOS;

    static RuntimeInfo()
    {
        if (OperatingSystem.IsWindows())
            OS = Platform.Windows;
        if (OperatingSystem.IsLinux())
            OS = OS == 0 ? Platform.Linux : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.Linux)}, but is already {Enum.GetName(OS)}");
        if (OperatingSystem.IsMacOS())
            OS = OS == 0 ? Platform.MacOS : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.MacOS)}, but is already {Enum.GetName(OS)}");

        if (OS == 0)
            throw new PlatformNotSupportedException("Your operating system is not supported.");
    }

    public enum Platform
    {
        Windows = 1,
        Linux = 2,
        MacOS = 3,
        // ? Should we add Android and iOS?
    }
}
