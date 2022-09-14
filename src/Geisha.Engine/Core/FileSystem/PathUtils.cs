using System.IO;

namespace Geisha.Engine.Core.FileSystem
{
    // TODO Add xml documentation.
    // TODO Should it become a part of IFile/IDirectory?
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