using System.Collections.Generic;

namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IDirectory
    {
        string Name { get; }
        IEnumerable<IFile> Files { get; }
        IEnumerable<IDirectory> Directories { get; }
    }
}