using System;
using System.Collections.Generic;
using Microsoft.Win32;

public class GetDotNetVersion
{
    public static void Main()
    {
        Get45PlusFromRegistry();
    }

    delegate string VersionChecker(RegistryKey ndpKey);

    private static void Get45PlusFromRegistry()
    {
        Dictionary<string, VersionChecker> subkeys = new Dictionary<string, VersionChecker>
        {
            { @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\",       CheckFor45PlusVersion },
            { @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5\",          CheckFor35OrEarlierVersion },
            { @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\",          CheckFor35OrEarlierVersion },
            { @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727\",    CheckFor35OrEarlierVersion }
        };

        using (var hklmKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, string.Empty))
        {
            foreach (var subkey in subkeys.Keys)
            {
                using (var ndpKey = hklmKey.OpenSubKey(subkey))
                {
                    var versionChecker = subkeys[subkey];
                    var version = versionChecker(ndpKey);

                    Console.WriteLine($".NET Framework Version: {version}");
                }
            }
        }
    }
    // Checking the version using >= enables forward compatibility.
    static string CheckFor45PlusVersion(RegistryKey ndpKey)
    {
        if (ndpKey != null && ndpKey.GetValue("Release") != null)
        {
            var releaseKey = (int) ndpKey.GetValue("Release");

            if (releaseKey >= 528040)
                return "4.8 or later";
            if (releaseKey >= 461808)
                return "4.7.2";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            if (releaseKey >= 378389)
                return "4.5";
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
        }
        return null;
    }

    static string CheckFor35OrEarlierVersion(RegistryKey ndpKey)
    {
        return (string) ndpKey?.GetValue("Version");
    }
}
// This example displays output like the following:
//       .NET Framework Version: 4.6.1