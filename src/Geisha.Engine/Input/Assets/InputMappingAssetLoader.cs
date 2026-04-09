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

            var inputMapping = new InputMapping();

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
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(serializableHardwareAction.Key.Value)
                            });
                            break;
                        }
                        case { Key: null, MouseButton: not null }:
                        {
                            var mouseVariant = serializableHardwareAction.MouseButton switch
                            {
                                MouseButton.LeftButton => HardwareInputVariant.MouseVariant.LeftButton,
                                MouseButton.MiddleButton => HardwareInputVariant.MouseVariant.MiddleButton,
                                MouseButton.RightButton => HardwareInputVariant.MouseVariant.RightButton,
                                MouseButton.XButton1 => HardwareInputVariant.MouseVariant.XButton1,
                                MouseButton.XButton2 => HardwareInputVariant.MouseVariant.XButton2,
                                _ => throw new ArgumentOutOfRangeException(nameof(SerializableHardwareAction.MouseButton),
                                    serializableHardwareAction.MouseButton,
                                    "Unsupported mouse button found in input mapping.")
                            };

                            hardwareActions.Add(new HardwareAction
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(mouseVariant)
                            });
                            break;
                        }
                        default:
                            throw InvalidInputMappingAssetContentException.CreateForInvalidHardwareAction(inputMappingAssetContent);
                    }
                }

                var actionMapping = new ActionMapping
                {
                    ActionName = actionName,
                    HardwareActions = hardwareActions.ToImmutable()
                };
                inputMapping.ActionMappings.Add(actionMapping);
            }

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
                                HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(serializableHardwareAxis.Key.Value),
                                Scale = serializableHardwareAxis.Scale
                            });
                            break;
                        }
                        case { Key: null, MouseAxis: not null }:
                        {
                            var mouseVariant = serializableHardwareAxis.MouseAxis switch
                            {
                                MouseAxis.AxisX => HardwareInputVariant.MouseVariant.AxisX,
                                MouseAxis.AxisY => HardwareInputVariant.MouseVariant.AxisY,
                                _ => throw new ArgumentOutOfRangeException(nameof(SerializableHardwareAxis.MouseAxis), serializableHardwareAxis.MouseAxis,
                                    "Unsupported mouse axis found in input mapping.")
                            };

                            hardwareAxes.Add(new HardwareAxis
                            {
                                HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(mouseVariant),
                                Scale = serializableHardwareAxis.Scale
                            });
                            break;
                        }
                        default:
                            throw InvalidInputMappingAssetContentException.CreateForInvalidHardwareAxis(inputMappingAssetContent);
                    }
                }

                var axisMapping = new AxisMapping
                {
                    AxisName = axisName,
                    HardwareAxes = hardwareAxes.ToImmutable()
                };

                inputMapping.AxisMappings.Add(axisMapping);
            }

            return inputMapping;
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