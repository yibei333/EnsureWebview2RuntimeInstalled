using Microsoft.Win32;
using SharpDevLib;

namespace EnsureWebview2RuntimeInstalled;

public static class RegistryUtil
{
    public static bool GetRuntimeIsInstalled()
    {
        var value = ReadRegistry(true, @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}", "pv");
        if (value.NotEmpty() && value != "0.0.0.0") return true;
        value = ReadRegistry(false, @"Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}", "pv");
        return value.NotEmpty() && value != "0.0.0.0";
    }

    static string? ReadRegistry(bool isLocal, string keyPath, string valueName)
    {
        var key = isLocal ? Registry.LocalMachine.OpenSubKey(keyPath) : Registry.CurrentUser.OpenSubKey(keyPath);
        if (key is null) return null;
        return key.GetValue(valueName)?.ToString();
    }
}
