using Geisha.Common.Math.Serialization;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Assets
{
    // TODO Introduce IAsset interface for all assets and pack Sprite into SpriteAsset?
    /// <summary>
    ///     Provides functionality to load <see cref="Sprite" /> from sprite file.
    /// </summary>
    internal sealed class SpriteLoader : AssetLoaderAdapter<Sprite>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IRenderer2D _renderer;

        public SpriteLoader(IFileSystem fileSystem, IJsonSerializer jsonSerializer, IRenderer2D renderer)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
            _renderer = renderer;
        }

        protected override Sprite LoadAsset(string filePath)
        {
            var spriteFileJson = _fileSystem.GetFile(filePath).ReadAllText();
            var spriteFileContent = _jsonSerializer.Deserialize<SpriteFileContent>(spriteFileJson);

            // TODO Same texture could be shared by many sprites so it should be loaded only if not already available
            var textureFilePath = PathUtils.GetSiblingPath(filePath, spriteFileContent.TextureAssetId.ToString());
            using (var stream = _fileSystem.GetFile(textureFilePath).OpenRead())
            {
                return new Sprite
                {
                    SourceTexture = _renderer.CreateTexture(stream),
                    SourceUV = SerializableVector2.ToVector2(spriteFileContent.SourceUV),
                    SourceDimension = SerializableVector2.ToVector2(spriteFileContent.SourceDimension),
                    SourceAnchor = SerializableVector2.ToVector2(spriteFileContent.SourceAnchor),
                    PixelsPerUnit = spriteFileContent.PixelsPerUnit
                };
            }
        }
    }
}