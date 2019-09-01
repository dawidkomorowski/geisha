using System;
using System.Diagnostics;
using System.Reflection;

namespace Geisha.Editor.Core
{
    public interface IVersionProvider
    {
        Version GetCurrentVersion();
    }

    internal class VersionProvider : IVersionProvider
    {
        public Version GetCurrentVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
            return Version.Parse(fileVersionInfo.ProductVersion);
        }
    }
}