using System.IO;

namespace Geisha.Engine.Core.FileSystem
{
    internal class File : IFile
    {
        public File(string path)
        {
            Path = System.IO.Path.GetFullPath(path);

            if (!System.IO.File.Exists(Path)) throw new FileNotFoundException("File not found.", Path);
        }

        public string Name => System.IO.Path.GetFileName(Path);
        public string Extension => System.IO.Path.GetExtension(Path);
        public string Path { get; }

        public string ReadAllText()
        {
            return System.IO.File.ReadAllText(Path);
        }

        public void WriteAllText(string contents)
        {
            System.IO.File.WriteAllText(Path, contents);
        }

        public Stream OpenRead()
        {
            return System.IO.File.OpenRead(Path);
        }
    }
}