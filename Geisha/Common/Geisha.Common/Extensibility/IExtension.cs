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
}