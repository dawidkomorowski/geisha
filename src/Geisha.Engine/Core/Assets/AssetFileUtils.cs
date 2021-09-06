using System.IO;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Provides set of utilities for working with asset file.
    /// </summary>
    public static class AssetFileUtils
    {
        /// <summary>
        ///     Asset file extension.
        /// </summary>
        public const string Extension = ".geisha-asset";

        /// <summary>
        ///     Returns file path with asset file extension appended to specified <paramref name="filePath" />.
        /// </summary>
        /// <param name="filePath">File path to append asset file extension to.</param>
        /// <returns>File path with asset file extension appended.</returns>
        public static string AppendExtension(string filePath) => $"{filePath}{Extension}";

        /// <summary>
        ///     Checks if specified <paramref name="filePath" /> has asset file extension.
        /// </summary>
        /// <param name="filePath">File path to check.</param>
        /// <returns>True if <paramref name="filePath" /> has asset file extension; false otherwise.</returns>
        public static bool IsAssetFile(string filePath) => Path.GetExtension(filePath) == Extension;
    }
}