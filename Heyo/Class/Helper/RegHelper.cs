using System;
using Microsoft.Win32;

namespace Heyo.Class.Helper
{
    public class RegHelper
    {
        public static RegistryKey GetRegistryKey()
        {
            return GetRegistryKey(null);
        }

        public static RegistryKey GetRegistryKey(string keyPath)
        {
            var localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                    Environment.Is64BitOperatingSystem
                        ? RegistryView.Registry64
                        : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath);
        }

        public static object GetRegistryValue(string keyPath, string keyName)
        {
            var registry = GetRegistryKey(keyPath);
            return registry.GetValue(keyName);
        }
    }
}