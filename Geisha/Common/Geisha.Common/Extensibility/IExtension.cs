using System;
using System.ComponentModel.Composition;
using Autofac;

namespace Geisha.Common.Extensibility
{
    [InheritedExport]
    public interface IExtension
    {
        string Name { get; }
        string Description { get; }
        string Category { get; }
        string Author { get; }
        Version Version { get; }

        void Register(ContainerBuilder containerBuilder);
    }

    public static class ExtensionExtensions
    {
        public static string Format(this IExtension extension)
        {
            return $@"
{nameof(IExtension.Name)}: {extension.Name}
{nameof(IExtension.Description)}: {extension.Description}
{nameof(IExtension.Category)}: {extension.Category}
{nameof(IExtension.Author)}: {extension.Author}
{nameof(IExtension.Version)}: {extension.Version}
Assembly: {extension.GetType().Assembly.GetName().Name}
";
        }
    }
}