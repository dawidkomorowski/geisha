namespace Geisha.Editor.CreateTexture.Model
{
    internal static class TextureFileFormat
    {
        public static bool IsSupported(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".bmp":
                    return true;
                case ".png":
                    return true;
                case ".jpg":
                    return true;
                default:
                    return false;
            }
        }
    }
}