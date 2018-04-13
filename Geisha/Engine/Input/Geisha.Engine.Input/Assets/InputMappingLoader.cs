using System.ComponentModel.Composition;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Input.Assets
{
    /// <summary>
    ///     Provides functionality to load <see cref="InputMapping" /> from <see cref="InputMappingFile" />.
    /// </summary>
    [Export(typeof(IAssetLoader))]
    internal class InputMappingLoader : AssetLoaderAdapter<InputMapping>
    {
        private readonly IFileSystem _fileSystem;

        [ImportingConstructor]
        public InputMappingLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public override object Load(string filePath)
        {
            var inputMappingFileJson = _fileSystem.ReadAllTextFromFile(filePath);
            var inputMappingFile = Serializer.DeserializeJson<InputMappingFile>(inputMappingFileJson);

            var inputMapping = new InputMapping();

            foreach (var actionMappingDefinition in inputMappingFile.ActionMappings)
            {
                var actionMapping = new ActionMapping
                {
                    ActionName = actionMappingDefinition.Key
                };

                foreach (var hardwareActionDefinition in actionMappingDefinition.Value)
                {
                    actionMapping.HardwareActions.Add(new HardwareAction
                    {
                        HardwareInputVariant = new HardwareInputVariant(hardwareActionDefinition.Key)
                    });
                }

                inputMapping.ActionMappings.Add(actionMapping);
            }

            foreach (var axisMappingDefinition in inputMappingFile.AxisMappings)
            {
                var axisMappingGroup = new AxisMapping
                {
                    AxisName = axisMappingDefinition.Key
                };

                foreach (var hardwareAxisDefinition in axisMappingDefinition.Value)
                {
                    axisMappingGroup.HardwareAxes.Add(new HardwareAxis
                    {
                        HardwareInputVariant = new HardwareInputVariant(hardwareAxisDefinition.Key),
                        Scale = hardwareAxisDefinition.Scale
                    });
                }

                inputMapping.AxisMappings.Add(axisMappingGroup);
            }

            return inputMapping;
        }
    }
}