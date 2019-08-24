using System.IO;

namespace Geisha.Common.FileSystem
{
    /// <summary>
    ///     Represents a file and provides methods for reading and writing to a file.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        ///     Name of the file including its extension.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Extension of the file including dot character at the beginning.
        /// </summary>
        string Extension { get; }

        /// <summary>
        ///     Absolute path to this file.
        /// </summary>
        string Path { get; }

        /// <summary>
        ///     Opens this text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <returns>A string containing all lines of the file.</returns>
        string ReadAllText();

        /// <summary>
        ///     Opens this text file, writes the specified string to the file, and then closes the file.
        /// </summary>
        /// <param name="contents">The string to write to the file.  </param>
        void WriteAllText(string contents);

        /// <summary>
        ///     Opens this file for reading.
        /// </summary>
        /// <returns>A read-only <see cref="Stream" /> for reading this file.</returns>
        Stream OpenRead();
    }
}