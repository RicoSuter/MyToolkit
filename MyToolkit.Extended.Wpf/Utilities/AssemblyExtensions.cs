using System;
using System.Reflection;

namespace MyToolkit.Utilities
{
    public static class AssemblyExtensions
    {
        public static DateTime GetBuildTime(this Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);
        }

        public static string GetVersionWithBuildTime(this Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return version + " (" + GetBuildTime(assembly) + ")";
        }
    }
}
