using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Engine.Rendering.Assets;

namespace Geisha.Editor.CreateSprite.Model
{
    internal static class CreateSpriteUtils
    {
        public static bool CanCreateSpriteFromFile(string fileExtension)
        {
            return IsTextureMetadataFile(fileExtension) || IsTextureDataFile(fileExtension);
        }

        public static bool IsTextureMetadataFile(string fileExtension)
        {
            return RenderingFileExtensions.Texture == fileExtension;
        }

        public static bool IsTextureDataFile(string fileExtension)
        {
            return TextureFileFormat.IsSupported(fileExtension);
        }
    }
}