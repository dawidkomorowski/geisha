using System;
using System.Diagnostics;

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
            var editorAssembly = typeof(VersionProvider).Assembly;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(editorAssembly.Location);
            return Version.Parse(fileVersionInfo.ProductVersion ?? throw new InvalidOperationException("Could not retrieve product version."));
        }
    }
}