using Geisha.Engine.Rendering.Assets;

namespace Geisha.Editor.CreateSprite.Model
{
    internal static class CanCreateSpriteFromFile
    {
        public static bool Check(string fileExtension)
        {
            return RenderingFileExtensions.Texture == fileExtension;
        }
    }
}