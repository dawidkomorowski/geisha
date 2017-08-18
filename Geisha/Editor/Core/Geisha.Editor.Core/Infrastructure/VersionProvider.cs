using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Reflection;

namespace Geisha.Editor.Core.Infrastructure
{
    public interface IVersionProvider
    {
        Version GetCurrentVersion();
    }

    [Export(typeof(IVersionProvider))]
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