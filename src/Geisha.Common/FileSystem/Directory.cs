using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Geisha.Common.FileSystem
{
    internal class Directory : IDirectory
    {
        public Directory(string path)
        {
            Path = System.IO.Path.GetFullPath(path);

            if (!System.IO.Directory.Exists(Path)) throw new DirectoryNotFoundException($"Directory not found. Directory path: {Path}");
        }

        public string Name => System.IO.Path.GetFileName(Path);
        public string Path { get; }
        public IEnumerable<IDirectory> Directories => System.IO.Directory.EnumerateDirectories(Path).Select(p => new Directory(p));
        public IEnumerable<IFile> Files => System.IO.Directory.EnumerateFiles(Path).Select(p => new File(p));
    }
}