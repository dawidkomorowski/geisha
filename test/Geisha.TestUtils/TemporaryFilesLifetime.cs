using System;
using System.Collections.Generic;
using System.IO;

namespace Geisha.TestUtils
{
    public sealed class TemporaryFilesLifetime : IDisposable
    {
        private readonly List<string> _temporaryFiles = new List<string>();
        private bool _isDisposed;

        public string AcquireTemporaryFilePath()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(TemporaryFilesLifetime));

            var path = Utils.GetRandomFilePath();
            _temporaryFiles.Add(path);
            return path;
        }

        public void Dispose()
        {
            _isDisposed = true;

            foreach (var temporaryFile in _temporaryFiles)
            {
                if (File.Exists(temporaryFile))
                {
                    File.Delete(temporaryFile);
                }
            }

            _temporaryFiles.Clear();
        }
    }
}