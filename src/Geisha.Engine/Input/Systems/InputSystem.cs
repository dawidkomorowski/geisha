using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Systems
{
    internal sealed class InputSystem : IFixedTimeStepSystem
    {
        private readonly IInputProvider _inputProvider;

        public InputSystem(IInputBackend inputBackend)
        {
            _inputProvider = inputBackend.CreateInputProvider();
        }

        public string Name => GetType().FullName;

        public void FixedUpdate(Scene scene)
        {
            var hardwareInput = _inputProvider.Capture();

            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<InputComponent>())
                {
                    var input = entity.GetComponent<InputComponent>();
                    input.HardwareInput = hardwareInput;

                    HandleActionMappings(input);
                    HandleAxisMappings(input);
                }
            }
        }

        private static double BoolToDouble(bool b)
        {
            return b ? 1 : 0;
        }

        #region Action mapping handling

        private static void HandleActionMappings(InputComponent inputComponent)
        {
            var previousActionStates = new Dictionary<string, bool>(inputComponent.ActionStates);

            ResetActionStates(inputComponent);
            if (inputComponent.InputMapping == null) return;

            var actionMappings = inputComponent.InputMapping.ActionMappings;

            foreach (var actionMapping in actionMappings)
            {
                var actionName = actionMapping.ActionName;

                foreach (var hardwareAction in actionMapping.HardwareActions)
                {
                    var state = ComputeState(inputComponent.HardwareInput, hardwareAction);
                    inputComponent.ActionStates[actionName] = state;
                    if (state) break;
                }

                if (inputComponent.ActionBindings.ContainsKey(actionName))
                {
                    previousActionStates.TryGetValue(actionName, out var previousActionState);

                    if (previousActionState == false && inputComponent.GetActionState(actionName))
                    {
                        inputComponent.ActionBindings[actionName]();
                    }
                }
            }
        }

        private static bool ComputeState(HardwareInput hardwareInput, HardwareAction hardwareAction)
        {
            var hardwareInputVariant = hardwareAction.HardwareInputVariant;
            switch (hardwareInputVariant.CurrentVariant)
            {
                case HardwareInputVariant.Variant.Keyboard:
                    return hardwareInput.KeyboardInput[hardwareInputVariant.AsKeyboard()];
                case HardwareInputVariant.Variant.Mouse:
                    switch (hardwareInputVariant.AsMouse())
                    {
                        case HardwareInputVariant.MouseVariant.LeftButton:
                            return hardwareInput.MouseInput.LeftButton;
                        case HardwareInputVariant.MouseVariant.MiddleButton:
                            return hardwareInput.MouseInput.MiddleButton;
                        case HardwareInputVariant.MouseVariant.RightButton:
                            return hardwareInput.MouseInput.RightButton;
                        case HardwareInputVariant.MouseVariant.XButton1:
                            return hardwareInput.MouseInput.XButton1;
                        case HardwareInputVariant.MouseVariant.XButton2:
                            return hardwareInput.MouseInput.XButton2;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ResetActionStates(InputComponent inputComponent)
        {
            var keys = inputComponent.ActionStates.Keys.ToList();
            foreach (var key in keys)
            {
                inputComponent.ActionStates[key] = false;
            }
        }

        #endregion

        #region Axis mapping handling

        private static void HandleAxisMappings(InputComponent inputComponent)
        {
            ResetAxisStates(inputComponent);
            if (inputComponent.InputMapping == null) return;

            var axisMappings = inputComponent.InputMapping.AxisMappings;

            foreach (var axisMapping in axisMappings)
            {
                var axisName = axisMapping.AxisName;

                foreach (var hardwareAxis in axisMapping.HardwareAxes)
                {
                    var state = ComputeState(inputComponent.HardwareInput, hardwareAxis);
                    var scaledState = state * hardwareAxis.Scale;

                    if (inputComponent.AxisStates.ContainsKey(axisName))
                    {
                        inputComponent.AxisStates[axisName] += scaledState;
                    }
                    else
                    {
                        inputComponent.AxisStates[axisName] = scaledState;
                    }
                }

                if (inputComponent.AxisBindings.ContainsKey(axisName))
                {
                    inputComponent.AxisBindings[axisName](inputComponent.GetAxisState(axisName));
                }
            }
        }

        private static double ComputeState(HardwareInput hardwareInput, HardwareAxis hardwareAxis)
        {
            var hardwareInputVariant = hardwareAxis.HardwareInputVariant;
            switch (hardwareInputVariant.CurrentVariant)
            {
                case HardwareInputVariant.Variant.Keyboard:
                    return BoolToDouble(hardwareInput.KeyboardInput[hardwareInputVariant.AsKeyboard()]);
                case HardwareInputVariant.Variant.Mouse:
                    switch (hardwareInputVariant.AsMouse())
                    {
                        case HardwareInputVariant.MouseVariant.AxisX:
                            return hardwareInput.MouseInput.PositionDelta.X;
                        case HardwareInputVariant.MouseVariant.AxisY:
                            return -hardwareInput.MouseInput.PositionDelta.Y;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ResetAxisStates(InputComponent inputComponent)
        {
            var keys = inputComponent.AxisStates.Keys.ToList();
            foreach (var key in keys)
            {
                inputComponent.AxisStates[key] = 0;
            }
        }

        #endregion
    }
}