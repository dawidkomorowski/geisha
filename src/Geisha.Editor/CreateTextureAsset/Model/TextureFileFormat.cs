namespace Geisha.Editor.CreateTextureAsset.Model
{
    internal static class TextureFileFormat
    {
        public static bool IsSupported(string fileExtension)
        {
            return fileExtension switch
            {
                ".bmp" => true,
                ".png" => true,
                ".jpg" => true,
                _ => false
            };
        }
    }
}