using System;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Input.Assets
{
    internal sealed class InputMappingManagedAsset : ManagedAsset<InputMapping>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public InputMappingManagedAsset(AssetInfo assetInfo, IFileSystem fileSystem, IJsonSerializer jsonSerializer) : base(assetInfo)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        protected override InputMapping LoadAsset()
        {
            var inputMappingFileJson = _fileSystem.GetFile(AssetInfo.AssetFilePath).ReadAllText();
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
                        HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(serializableHardwareAction.Key)
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
                        HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(serializableHardwareAxis.Key),
                        Scale = serializableHardwareAxis.Scale
                    });
                }

                inputMapping.AxisMappings.Add(axisMapping);
            }

            return inputMapping;
        }

        protected override void UnloadAsset(InputMapping asset)
        {
        }
    }
}