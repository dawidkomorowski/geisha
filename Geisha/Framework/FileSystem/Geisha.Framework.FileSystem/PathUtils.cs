using System.IO;

namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public static class PathUtils
    {
        public static string GetSiblingPath(string path, string relativeSiblingPath)
        {
            var parentPath = Path.GetDirectoryName(path);
            var siblingPath = Path.Combine(parentPath, relativeSiblingPath);
            return siblingPath;
        }
    }
}