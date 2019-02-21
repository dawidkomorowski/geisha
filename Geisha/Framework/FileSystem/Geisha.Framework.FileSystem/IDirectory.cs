using System.Collections.Generic;

namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IDirectory
    {
        string Name { get; }
        string Path { get; }
        IEnumerable<IDirectory> Directories { get; }
        IEnumerable<IFile> Files { get; }
    }
}