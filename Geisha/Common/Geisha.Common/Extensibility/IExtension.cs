using System;
using System.ComponentModel.Composition;
using Autofac;

namespace Geisha.Common.Extensibility
{
    /// <summary>
    ///     Specifies interface of extension for Geisha Engine. Implement this interface to provide custom functionality within
    ///     an extension.
    /// </summary>
    [InheritedExport]
    public interface IExtension
    {
        /// <summary>
        ///     Name of extension.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Description of extension. Provides brief information about functionality an extension implements.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Category of extension. Allows identifying and grouping similar extensions with common functionality.
        /// </summary>
        string Category { get; }

        /// <summary>
        ///     Author of extension.
        /// </summary>
        string Author { get; }

        /// <summary>
        ///     Version of extension.
        /// </summary>
        Version Version { get; }

        /// <summary>
        ///     Registers extension components in Autofac container.
        /// </summary>
        /// <param name="containerBuilder">Autofac container builder that provides components registration API.</param>
        /// <remarks>Implement this method to register components and services an extension provides in Autofac container.</remarks>
        void Register(ContainerBuilder containerBuilder);
    }

    /// <summary>
    ///     Provides extension methods for <see cref="IExtension" /> interface.
    /// </summary>
    public static class ExtensionExtensions
    {
        /// <summary>
        ///     Returns human-readable string representation of <see cref="IExtension" /> object.
        /// </summary>
        /// <param name="extension">An extension object to be used.</param>
        /// <returns>Human-readable string representation of <see cref="IExtension" /> object.</returns>
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