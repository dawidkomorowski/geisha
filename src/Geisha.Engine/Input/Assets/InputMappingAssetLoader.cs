using System;
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
                var actionMapping = new ActionMapping
                {
                    ActionName = actionName
                };

                foreach (var serializableHardwareAction in serializableHardwareActions)
                {
                    if (serializableHardwareAction.Key != null && serializableHardwareAction.MouseButton == null)
                    {
                        actionMapping.HardwareActions.Add(new HardwareAction
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(serializableHardwareAction.Key.Value)
                        });
                    }
                    else if (serializableHardwareAction.Key == null && serializableHardwareAction.MouseButton != null)
                    {
                        var mouseVariant = serializableHardwareAction.MouseButton switch
                        {
                            MouseButton.LeftButton => HardwareInputVariant.MouseVariant.LeftButton,
                            MouseButton.MiddleButton => HardwareInputVariant.MouseVariant.MiddleButton,
                            MouseButton.RightButton => HardwareInputVariant.MouseVariant.RightButton,
                            MouseButton.XButton1 => HardwareInputVariant.MouseVariant.XButton1,
                            MouseButton.XButton2 => HardwareInputVariant.MouseVariant.XButton2,
                            _ => throw new ArgumentOutOfRangeException(nameof(SerializableHardwareAction.MouseButton), serializableHardwareAction.MouseButton,
                                "Unsupported mouse button found in input mapping.")
                        };

                        actionMapping.HardwareActions.Add(new HardwareAction
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(mouseVariant)
                        });
                    }
                    else
                    {
                        throw InvalidInputMappingAssetContentException.CreateForInvalidHardwareAction(inputMappingAssetContent);
                    }
                }

                inputMapping.ActionMappings.Add(actionMapping);
            }

            foreach (var (axisName, serializableHardwareAxes) in inputMappingAssetContent.AxisMappings)
            {
                var axisMapping = new AxisMapping
                {
                    AxisName = axisName
                };

                foreach (var serializableHardwareAxis in serializableHardwareAxes)
                {
                    if (serializableHardwareAxis.Key != null && serializableHardwareAxis.MouseAxis == null)
                    {
                        axisMapping.HardwareAxes.Add(new HardwareAxis
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(serializableHardwareAxis.Key.Value),
                            Scale = serializableHardwareAxis.Scale
                        });
                    }
                    else if (serializableHardwareAxis.Key == null && serializableHardwareAxis.MouseAxis != null)
                    {
                        var mouseVariant = serializableHardwareAxis.MouseAxis switch
                        {
                            MouseAxis.AxisX => HardwareInputVariant.MouseVariant.AxisX,
                            MouseAxis.AxisY => HardwareInputVariant.MouseVariant.AxisY,
                            _ => throw new ArgumentOutOfRangeException(nameof(SerializableHardwareAxis.MouseAxis), serializableHardwareAxis.MouseAxis,
                                "Unsupported mouse axis found in input mapping.")
                        };

                        axisMapping.HardwareAxes.Add(new HardwareAxis
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(mouseVariant),
                            Scale = serializableHardwareAxis.Scale
                        });
                    }
                    else
                    {
                        throw InvalidInputMappingAssetContentException.CreateForInvalidHardwareAxis(inputMappingAssetContent);
                    }
                }

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