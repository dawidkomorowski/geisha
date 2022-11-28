using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Systems
{
    // TODO Should this system be Fixed or Variable time step? How it impacts determinism of simulation?
    internal sealed class InputSystem : IInputGameLoopStep, ISceneObserver
    {
        private readonly IInputProvider _inputProvider;
        private readonly List<InputComponent> _inputComponents = new();

        public InputSystem(IInputBackend inputBackend)
        {
            _inputProvider = inputBackend.CreateInputProvider();
        }

        #region Implementation of IInputGameLoopStep

        public void ProcessInput()
        {
            var hardwareInput = _inputProvider.Capture();

            foreach (var inputComponent in _inputComponents)
            {
                inputComponent.HardwareInput = hardwareInput;

                HandleActionMappings(inputComponent);
                HandleAxisMappings(inputComponent);
            }
        }

        #endregion

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
            if (component is InputComponent inputComponent)
            {
                _inputComponents.Add(inputComponent);
            }
        }

        public void OnComponentRemoved(Component component)
        {
            if (component is InputComponent inputComponent)
            {
                _inputComponents.Remove(inputComponent);
            }
        }

        #endregion

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

                if (inputComponent.ActionBindings.TryGetValue(actionName, out var binding))
                {
                    previousActionStates.TryGetValue(actionName, out var previousActionState);

                    if (!previousActionState && inputComponent.GetActionState(actionName))
                    {
                        binding();
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
                    return hardwareInputVariant.AsMouse() switch
                    {
                        HardwareInputVariant.MouseVariant.LeftButton => hardwareInput.MouseInput.LeftButton,
                        HardwareInputVariant.MouseVariant.MiddleButton => hardwareInput.MouseInput.MiddleButton,
                        HardwareInputVariant.MouseVariant.RightButton => hardwareInput.MouseInput.RightButton,
                        HardwareInputVariant.MouseVariant.XButton1 => hardwareInput.MouseInput.XButton1,
                        HardwareInputVariant.MouseVariant.XButton2 => hardwareInput.MouseInput.XButton2,
                        _ => throw new ArgumentOutOfRangeException()
                    };

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
                    inputComponent.AxisStates[axisName] += scaledState;
                }

                if (inputComponent.AxisBindings.TryGetValue(axisName, out var binding))
                {
                    binding(inputComponent.GetAxisState(axisName));
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
                    return hardwareInputVariant.AsMouse() switch
                    {
                        HardwareInputVariant.MouseVariant.AxisX => hardwareInput.MouseInput.PositionDelta.X,
                        HardwareInputVariant.MouseVariant.AxisY => -hardwareInput.MouseInput.PositionDelta.Y,
                        _ => throw new ArgumentOutOfRangeException()
                    };

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