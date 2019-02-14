﻿using System.IO;

namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IFileSystem
    {
        string ReadAllTextFromFile(string path);
        void WriteAllTextToFile(string path, string contents);
        Stream OpenFileStreamForReading(string path);
        IDirectory GetDirectory(string path);
    }

    internal sealed class FileSystem : IFileSystem
    {
        public string ReadAllTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllTextToFile(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public Stream OpenFileStreamForReading(string path)
        {
            return new FileStream(path, FileMode.Open);
        }

        public IDirectory GetDirectory(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}