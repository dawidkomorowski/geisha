using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Assets
{
    /// <summary>
    ///     Provides functionality to load <see cref="ITexture" /> from sprite file.
    /// </summary>
    internal sealed class TextureLoader : AssetLoaderAdapter<ITexture>
    {
        private readonly IRenderer2D _renderer2D;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public TextureLoader(IRenderer2D renderer2D, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _renderer2D = renderer2D;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        protected override ITexture LoadAsset(string filePath)
        {
            var textureFileJson = _fileSystem.GetFile(filePath).ReadAllText();
            var textureFileContent = _jsonSerializer.Deserialize<TextureFileContent>(textureFileJson);

            var textureFilePath = PathUtils.GetSiblingPath(filePath, textureFileContent.TextureFilePath);
            using (var stream = _fileSystem.GetFile(textureFilePath).OpenRead())
            {
                return _renderer2D.CreateTexture(stream);
            }
        }
    }
}