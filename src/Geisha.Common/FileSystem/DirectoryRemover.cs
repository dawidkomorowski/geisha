namespace Geisha.Common.FileSystem
{
    public static class DirectoryRemover
    {
        public static void RemoveDirectoryRecursively(string path)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(path))
            {
                System.IO.File.Delete(file);
            }

            foreach (var directory in System.IO.Directory.EnumerateDirectories(path))
            {
                RemoveDirectoryRecursively(directory);
            }

            System.IO.Directory.Delete(path);
        }
    }
}