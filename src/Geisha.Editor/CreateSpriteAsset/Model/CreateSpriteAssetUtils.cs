using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Engine.Rendering.Assets;

namespace Geisha.Editor.CreateSpriteAsset.Model
{
    internal static class CreateSpriteAssetUtils
    {
        public static bool CanCreateSpriteAssetFromFile(string fileExtension)
        {
            return IsTextureAssetFile(fileExtension) || IsTextureFile(fileExtension);
        }

        public static bool IsTextureAssetFile(string fileExtension)
        {
            return RenderingFileExtensions.Texture == fileExtension;
        }

        public static bool IsTextureFile(string fileExtension)
        {
            return TextureFileFormat.IsSupported(fileExtension);
        }
    }
}