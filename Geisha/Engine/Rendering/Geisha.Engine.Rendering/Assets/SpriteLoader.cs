using Geisha.Common.Math.Serialization;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Assets
{
    // TODO Introduce IAsset interface for all assets and pack Sprite into SpriteAsset?
    /// <summary>
    ///     Provides functionality to load <see cref="Sprite" /> from <see cref="SpriteFile" />.
    /// </summary>
    internal sealed class SpriteLoader : AssetLoaderAdapter<Sprite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRenderer2D _renderer;

        public SpriteLoader(IFileSystem fileSystem, IRenderer2D renderer)
        {
            _fileSystem = fileSystem;
            _renderer = renderer;
        }

        protected override Sprite LoadAsset(string filePath)
        {
            var spriteFileJson = _fileSystem.ReadAllTextFromFile(filePath);
            var spriteFile = Serializer.DeserializeJson<SpriteFile>(spriteFileJson);

            // TODO Same texture could be shared by many sprites so it should be loaded only if not already available
            var textureFilePath = PathUtils.GetSiblingPath(filePath, spriteFile.SourceTextureFilePath);
            using (var stream = _fileSystem.OpenFileStreamForReading(textureFilePath))
            {
                return new Sprite
                {
                    SourceTexture = _renderer.CreateTexture(stream),
                    SourceUV = SerializableVector2.ToVector2(spriteFile.SourceUV),
                    SourceDimension = SerializableVector2.ToVector2(spriteFile.SourceDimension),
                    SourceAnchor = SerializableVector2.ToVector2(spriteFile.SourceAnchor),
                    PixelsPerUnit = spriteFile.PixelsPerUnit
                };
            }
        }
    }
}