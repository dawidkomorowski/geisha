using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Input.Assets
{
    public class InputMappingFileLoader : AssetLoaderAdapter<InputMappingFile>
    {
        private readonly IFileSystem _fileSystem;

        public InputMappingFileLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public override object Load(string filePath)
        {
            var inputMappingFileJson = _fileSystem.ReadAllTextFromFile(filePath);
            var inputMappingFile = Serializer.DeserializeJson<InputMappingFile>(inputMappingFileJson);

            var inputMapping = new InputMapping();

            foreach (var actionMappingDefinitions in inputMappingFile.ActionMappings)
            {
                var actionMappingGroup = new ActionMappingGroup
                {
                    ActionName = actionMappingDefinitions.Key
                };

                foreach (var actionMappingDefinition in actionMappingDefinitions.Value)
                {
                    actionMappingGroup.ActionMappings.Add(new ActionMapping
                    {
                        HardwareInputVariant = new HardwareInputVariant(actionMappingDefinition.Key)
                    });
                }

                inputMapping.ActionMappingGroups.Add(actionMappingGroup);
            }

            foreach (var axisMappingDefinitions in inputMappingFile.AxisMappings)
            {
                var axisMappingGroup = new AxisMappingGroup
                {
                    AxisName = axisMappingDefinitions.Key
                };

                foreach (var axisMappingDefinition in axisMappingDefinitions.Value)
                {
                    axisMappingGroup.AxisMappings.Add(new AxisMapping
                    {
                        HardwareInputVariant = new HardwareInputVariant(axisMappingDefinition.Key),
                        Scale = axisMappingDefinition.Scale
                    });
                }

                inputMapping.AxisMappingGroups.Add(axisMappingGroup);
            }

            return inputMapping;
        }
    }
}