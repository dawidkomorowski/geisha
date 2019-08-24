using System.Collections.Generic;

namespace Geisha.Common.FileSystem
{
    /// <summary>
    ///     Represents a directory and provides methods for inspecting files and directories it contains.
    /// </summary>
    public interface IDirectory
    {
        /// <summary>
        ///     Name of the directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Absolute path to this directory.
        /// </summary>
        string Path { get; }

        /// <summary>
        ///     Directories in this directory.
        /// </summary>
        IEnumerable<IDirectory> Directories { get; }

        /// <summary>
        ///     Files in this directory.
        /// </summary>
        IEnumerable<IFile> Files { get; }
    }
}