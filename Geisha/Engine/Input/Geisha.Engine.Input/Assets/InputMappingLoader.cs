using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Input.Assets
{
    /// <summary>
    ///     Provides functionality to load <see cref="InputMapping" /> from input mapping file.
    /// </summary>
    internal sealed class InputMappingLoader : AssetLoaderAdapter<InputMapping>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public InputMappingLoader(IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        protected override InputMapping LoadAsset(string filePath)
        {
            var inputMappingFileJson = _fileSystem.GetFile(filePath).ReadAllText();
            var inputMappingFileContent = _jsonSerializer.Deserialize<InputMappingFileContent>(inputMappingFileJson);

            var inputMapping = new InputMapping();

            foreach (var serializableActionMapping in inputMappingFileContent.ActionMappings)
            {
                var actionMapping = new ActionMapping
                {
                    ActionName = serializableActionMapping.Key
                };

                foreach (var serializableHardwareAction in serializableActionMapping.Value)
                {
                    actionMapping.HardwareActions.Add(new HardwareAction
                    {
                        HardwareInputVariant = new HardwareInputVariant(serializableHardwareAction.Key)
                    });
                }

                inputMapping.ActionMappings.Add(actionMapping);
            }

            foreach (var serializableAxisMapping in inputMappingFileContent.AxisMappings)
            {
                var axisMapping = new AxisMapping
                {
                    AxisName = serializableAxisMapping.Key
                };

                foreach (var serializableHardwareAxis in serializableAxisMapping.Value)
                {
                    axisMapping.HardwareAxes.Add(new HardwareAxis
                    {
                        HardwareInputVariant = new HardwareInputVariant(serializableHardwareAxis.Key),
                        Scale = serializableHardwareAxis.Scale
                    });
                }

                inputMapping.AxisMappings.Add(axisMapping);
            }

            return inputMapping;
        }
    }
}