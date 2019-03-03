namespace Geisha.Framework.FileSystem
{
    /// <summary>
    ///     Provides access to file system.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        ///     Creates or overwrites a file in the specified path.
        /// </summary>
        /// <param name="filePath">The path and name of the file to create.</param>
        /// <returns>
        ///     An <see cref="IFile" /> instance that represents the file specified in <paramref name="filePath" />.
        /// </returns>
        IFile CreateFile(string filePath);

        /// <summary>
        ///     Gets an <see cref="IFile" /> instance that represents the file specified in <paramref name="filePath" />.
        /// </summary>
        /// <param name="filePath">The path and name of the existing file.</param>
        /// <returns>
        ///     An <see cref="IFile" /> instance that represents the file specified in <paramref name="filePath" />.
        /// </returns>
        IFile GetFile(string filePath);

        /// <summary>
        ///     Gets an <see cref="IDirectory" /> instance that represents the directory specified in
        ///     <paramref name="directoryPath" />.
        /// </summary>
        /// <param name="directoryPath">The path to existing directory.</param>
        /// <returns>
        ///     An <see cref="IDirectory" /> instance that represents the directory specified in
        ///     <paramref name="directoryPath" />.
        /// </returns>
        IDirectory GetDirectory(string directoryPath);
    }
}