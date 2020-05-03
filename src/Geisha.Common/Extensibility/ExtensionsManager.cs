using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Geisha.Common.Logging;

namespace Geisha.Common.Extensibility
{
    /// <summary>
    ///     Implements discovery and loading of extensions.
    /// </summary>
    public sealed class ExtensionsManager
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ExtensionsManager));
        private bool _extensionsLoaded;

        public ExtensionsManager()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainOnReflectionOnlyAssemblyResolve;
        }

        private static Assembly CurrentDomainOnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        /// <summary>
        ///     Discovers and loads extensions located in specified directory.
        /// </summary>
        /// <param name="directoryPath">Path to directory from which extensions are being loaded.</param>
        /// <returns>Extensions that were successfully discovered and loaded.</returns>
        public IReadOnlyCollection<IExtension> LoadExtensions(string directoryPath)
        {
            ThrowIfExtensionsAlreadyLoaded();

            Log.Info("Loading extensions.");
            _extensionsLoaded = true;

            var dlls = GetApplicationDlls(directoryPath);
            //var dllsWithExtensions = dlls.Where(DllContainsExtension); TODO Reflection only load is not supported by netcoreapp31 so assemblies are just loaded instead of investigated first but it is bad solution. It should be rewritten after migration to dotnet core is done. Also this code may be not relevant for engine (if deployment model will be changed) and only editor will dynamically load extensions.
            var extensions = dlls.SelectMany(LoadExtensionsFromDll).ToList();

            foreach (var extension in extensions)
            {
                Log.Info($"Extension loaded: {extension.Format()}");
            }

            Log.Info("Extensions loaded successfully.");

            return extensions.AsReadOnly();
        }

        private static IEnumerable<string> GetApplicationDlls(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath)
                .Where(path => Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase));
        }

        private static bool DllContainsExtension(string dllPath)
        {
            var assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
            return assembly.GetExportedTypes().Any(t => t.GetInterfaces().Any(i => i.AssemblyQualifiedName == typeof(IExtension).AssemblyQualifiedName));
        }

        private static IReadOnlyCollection<IExtension> LoadExtensionsFromDll(string dllPath)
        {
            var assembly = Assembly.LoadFrom(dllPath);
            var extensionsTypes = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(i => i == typeof(IExtension)));

            var extensions = new List<IExtension>();
            foreach (var extensionsType in extensionsTypes)
            {
                var extension = (IExtension) Activator.CreateInstance(extensionsType);
                extensions.Add(extension);
            }

            return extensions.AsReadOnly();
        }

        private void ThrowIfExtensionsAlreadyLoaded()
        {
            if (_extensionsLoaded) throw new InvalidOperationException($"Extensions were already loaded by this instance of {nameof(ExtensionsManager)}.");
        }
    }
}