using System;
using System.IO;

namespace Geisha.TestUtils
{
    public sealed class TemporaryDirectory : IDisposable
    {
        private bool _isDisposed;

        public TemporaryDirectory()
        {
            Path = Utils.GetPathUnderTestDirectory($"{nameof(TemporaryDirectory)}-{Utils.Random.GetString()}");
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public string GetRandomFilePath()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TemporaryDirectory));

            return GetPathUnderTemporaryDirectory(System.IO.Path.GetRandomFileName());
        }

        public string GetRandomDirectoryPath()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TemporaryDirectory));

            return GetPathUnderTemporaryDirectory(Utils.Random.GetString());
        }

        public string GetPathUnderTemporaryDirectory(string relativePath)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TemporaryDirectory));

            return System.IO.Path.Combine(Path, relativePath);
        }

        public void Dispose()
        {
            _isDisposed = true;

            if (new Uri(Path).IsBaseOf(new Uri(Directory.GetCurrentDirectory())))
            {
                Directory.SetCurrentDirectory(Utils.TestDirectory);
            }

            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, true);
            }
        }
    }
}