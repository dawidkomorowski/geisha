using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Assets
{
    internal sealed class InputMappingAssetDiscoveryRule : IAssetDiscoveryRule
    {
        // TODO Consider changing Geisha Engine file extensions to prefixed version like .gsound, .gsprite, .gtexture, .ginput.
        private const string InputMappingFileExtension = ".input";
        private readonly IJsonSerializer _jsonSerializer;

        public InputMappingAssetDiscoveryRule(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<AssetInfo> Discover(IFile file)
        {
            if (file.Extension == InputMappingFileExtension)
            {
                var inputMappingFileContent = _jsonSerializer.Deserialize<InputMappingFileContent>(file.ReadAllText());
                var assetInfo = new AssetInfo(
                    new AssetId(inputMappingFileContent.AssetId),
                    typeof(InputMapping),
                    file.Path);
                return SingleOrEmpty.Single(assetInfo);
            }
            else
            {
                return SingleOrEmpty.Empty<AssetInfo>();
            }
        }
    }
}