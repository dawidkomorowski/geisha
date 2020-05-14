using System;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Input.Mapping;

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

            if (inputMappingFileContent.ActionMappings == null)
                throw new InvalidOperationException($"{nameof(InputMappingFileContent)}.{nameof(InputMappingFileContent.ActionMappings)} cannot be null.");
            if (inputMappingFileContent.AxisMappings == null)
                throw new InvalidOperationException($"{nameof(InputMappingFileContent)}.{nameof(InputMappingFileContent.AxisMappings)} cannot be null.");

            var inputMapping = new InputMapping();

            foreach (var (actionName, serializableHardwareActions) in inputMappingFileContent.ActionMappings)
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
                        HardwareInputVariant.MouseVariant mouseVariant;
                        switch (serializableHardwareAction.MouseButton)
                        {
                            case MouseButton.LeftButton:
                                mouseVariant = HardwareInputVariant.MouseVariant.LeftButton;
                                break;
                            case MouseButton.MiddleButton:
                                mouseVariant = HardwareInputVariant.MouseVariant.MiddleButton;
                                break;
                            case MouseButton.RightButton:
                                mouseVariant = HardwareInputVariant.MouseVariant.RightButton;
                                break;
                            case MouseButton.XButton1:
                                mouseVariant = HardwareInputVariant.MouseVariant.XButton1;
                                break;
                            case MouseButton.XButton2:
                                mouseVariant = HardwareInputVariant.MouseVariant.XButton2;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(SerializableHardwareAction.MouseButton), serializableHardwareAction.MouseButton,
                                    "Unsupported mouse button found in input mapping.");
                        }

                        actionMapping.HardwareActions.Add(new HardwareAction
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(mouseVariant)
                        });
                    }
                    else
                    {
                        throw InvalidInputMappingFileContentException.CreateForInvalidHardwareAction(inputMappingFileContent);
                    }
                }

                inputMapping.ActionMappings.Add(actionMapping);
            }

            foreach (var (axisName, serializableHardwareAxes) in inputMappingFileContent.AxisMappings)
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
                        HardwareInputVariant.MouseVariant mouseVariant;
                        switch (serializableHardwareAxis.MouseAxis)
                        {
                            case MouseAxis.AxisX:
                                mouseVariant = HardwareInputVariant.MouseVariant.AxisX;
                                break;
                            case MouseAxis.AxisY:
                                mouseVariant = HardwareInputVariant.MouseVariant.AxisY;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(SerializableHardwareAxis.MouseAxis), serializableHardwareAxis.MouseAxis,
                                    "Unsupported mouse axis found in input mapping.");
                        }

                        axisMapping.HardwareAxes.Add(new HardwareAxis
                        {
                            HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(mouseVariant),
                            Scale = serializableHardwareAxis.Scale
                        });
                    }
                    else
                    {
                        throw InvalidInputMappingFileContentException.CreateForInvalidHardwareAxis(inputMappingFileContent);
                    }
                }

                inputMapping.AxisMappings.Add(axisMapping);
            }

            return inputMapping;
        }

        protected override void UnloadAsset(InputMapping asset)
        {
        }
    }

    /// <summary>
    ///     The exception that is thrown when loading input mapping asset from invalid input mapping file.
    /// </summary>
    public sealed class InvalidInputMappingFileContentException : Exception
    {
        private InvalidInputMappingFileContentException(InputMappingFileContent inputMappingFileContent, string message) : base(message)
        {
            InputMappingFileContent = inputMappingFileContent;
        }

        /// <summary>
        ///     Asset info of asset that registration has failed.
        /// </summary>
        public InputMappingFileContent InputMappingFileContent { get; }

        internal static InvalidInputMappingFileContentException CreateForInvalidHardwareAction(InputMappingFileContent inputMappingFileContent)
        {
            return new InvalidInputMappingFileContentException(inputMappingFileContent, "Hardware action does not specify single input device key or button.");
        }

        internal static InvalidInputMappingFileContentException CreateForInvalidHardwareAxis(InputMappingFileContent inputMappingFileContent)
        {
            return new InvalidInputMappingFileContentException(inputMappingFileContent, "Hardware axis does not specify single input device key or axis.");
        }
    }
}