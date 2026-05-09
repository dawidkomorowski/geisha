using System;
using System.Collections.Immutable;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Assets
{
    internal sealed class InputMappingAssetLoader : IAssetLoader
    {
        private readonly IFileSystem _fileSystem;

        public InputMappingAssetLoader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public AssetType AssetType => InputAssetTypes.InputMapping;
        public Type AssetClassType { get; } = typeof(InputMapping);

        public object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore)
        {
            using var fileStream = _fileSystem.GetFile(assetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(fileStream);
            var inputMappingAssetContent = assetData.ReadJsonContent<InputMappingAssetContent>();

            if (inputMappingAssetContent.ActionMappings == null)
                throw new InvalidOperationException($"{nameof(InputMappingAssetContent)}.{nameof(InputMappingAssetContent.ActionMappings)} cannot be null.");
            if (inputMappingAssetContent.AxisMappings == null)
                throw new InvalidOperationException($"{nameof(InputMappingAssetContent)}.{nameof(InputMappingAssetContent.AxisMappings)} cannot be null.");

            var actionMappings = ImmutableArray.CreateBuilder<ActionMapping>();

            foreach (var (actionName, serializableHardwareActions) in inputMappingAssetContent.ActionMappings)
            {
                var hardwareActions = ImmutableArray.CreateBuilder<HardwareAction>();

                foreach (var serializableHardwareAction in serializableHardwareActions)
                {
                    switch (serializableHardwareAction)
                    {
                        case { Key: not null, MouseButton: null }:
                        {
                            hardwareActions.Add(new HardwareAction
                            {
                                InputElement = InputElement.Create(serializableHardwareAction.Key.Value)
                            });
                            break;
                        }
                        case { Key: null, MouseButton: not null }:
                        {
                            hardwareActions.Add(new HardwareAction
                            {
                                InputElement = InputElement.Create(serializableHardwareAction.MouseButton.Value)
                            });
                            break;
                        }
                        default:
                            throw InvalidInputMappingAssetContentException.CreateForInvalidHardwareAction(inputMappingAssetContent);
                    }
                }

                actionMappings.Add(new ActionMapping
                {
                    ActionName = actionName,
                    HardwareActions = hardwareActions.ToImmutable()
                });
            }

            var axisMappings = ImmutableArray.CreateBuilder<AxisMapping>();

            foreach (var (axisName, serializableHardwareAxes) in inputMappingAssetContent.AxisMappings)
            {
                var hardwareAxes = ImmutableArray.CreateBuilder<HardwareAxis>();

                foreach (var serializableHardwareAxis in serializableHardwareAxes)
                {
                    switch (serializableHardwareAxis)
                    {
                        case { Key: not null, MouseAxis: null }:
                        {
                            hardwareAxes.Add(new HardwareAxis
                            {
                                InputElement = InputElement.Create(serializableHardwareAxis.Key.Value),
                                Scale = serializableHardwareAxis.Scale
                            });
                            break;
                        }
                        case { Key: null, MouseAxis: not null }:
                        {
                            hardwareAxes.Add(new HardwareAxis
                            {
                                InputElement = InputElement.Create(serializableHardwareAxis.MouseAxis.Value),
                                Scale = serializableHardwareAxis.Scale
                            });
                            break;
                        }
                        default:
                            throw InvalidInputMappingAssetContentException.CreateForInvalidHardwareAxis(inputMappingAssetContent);
                    }
                }

                axisMappings.Add(new AxisMapping
                {
                    AxisName = axisName,
                    HardwareAxes = hardwareAxes.ToImmutable()
                });
            }

            return new InputMapping
            {
                ActionMappings = actionMappings.ToImmutable(),
                AxisMappings = axisMappings.ToImmutable()
            };
        }

        public void UnloadAsset(object asset)
        {
        }
    }

    /// <summary>
    ///     The exception that is thrown when loading input mapping asset from file with invalid content.
    /// </summary>
    public sealed class InvalidInputMappingAssetContentException : Exception
    {
        private InvalidInputMappingAssetContentException(InputMappingAssetContent inputMappingAssetContent, string message) : base(message)
        {
            InputMappingAssetContent = inputMappingAssetContent;
        }

        /// <summary>
        ///     Asset content with invalid data.
        /// </summary>
        public InputMappingAssetContent InputMappingAssetContent { get; }

        internal static InvalidInputMappingAssetContentException CreateForInvalidHardwareAction(InputMappingAssetContent inputMappingAssetContent)
        {
            return new InvalidInputMappingAssetContentException(inputMappingAssetContent,
                "Hardware action does not specify single input device key or button.");
        }

        internal static InvalidInputMappingAssetContentException CreateForInvalidHardwareAxis(InputMappingAssetContent inputMappingAssetContent)
        {
            return new InvalidInputMappingAssetContentException(inputMappingAssetContent, "Hardware axis does not specify single input device key or axis.");
        }
    }
}