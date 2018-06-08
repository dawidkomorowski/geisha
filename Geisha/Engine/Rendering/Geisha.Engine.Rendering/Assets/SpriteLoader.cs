using System.ComponentModel.Composition;
using Geisha.Common.Math.Definition;
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
    [Export(typeof(IAssetLoader))]
    internal sealed class SpriteLoader : AssetLoaderAdapter<Sprite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRenderer2D _renderer;

        [ImportingConstructor]
        public SpriteLoader(IFileSystem fileSystem, IRenderer2D renderer)
        {
            _fileSystem = fileSystem;
            _renderer = renderer;
        }

        protected override Sprite LoadAsset(string filePath)
        {
            var spriteFileJson = _fileSystem.ReadAllTextFromFile(filePath);
            var spriteFile = Serializer.DeserializeJson<SpriteFile>(spriteFileJson);

            var textureFilePath = PathUtils.GetSiblingPath(filePath, spriteFile.SourceTextureFilePath);
            using (var stream = _fileSystem.OpenFileStreamForReading(textureFilePath))
            {
                return new Sprite
                {
                    SourceTexture = _renderer.CreateTexture(stream),
                    SourceUV = Vector2Definition.ToVector2(spriteFile.SourceUV),
                    SourceDimension = Vector2Definition.ToVector2(spriteFile.SourceDimension),
                    SourceAnchor = Vector2Definition.ToVector2(spriteFile.SourceAnchor),
                    PixelsPerUnit = spriteFile.PixelsPerUnit
                };
            }
        }
    }
}