using System;
using Autofac;
using Geisha.Common.Extensibility;

namespace Geisha.Framework.FileSystem
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Framework File System";
        public string Description => "Provides file system access.";
        public string Category => "File system";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
        }
    }
}