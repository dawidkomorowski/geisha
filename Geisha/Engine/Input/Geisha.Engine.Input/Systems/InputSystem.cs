using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Systems
{
    [Export(typeof(IFixedTimeStepSystem))]
    public class InputSystem : IFixedTimeStepSystem
    {
        private readonly IInputProvider _inputProvider;

        [ImportingConstructor]
        public InputSystem(IInputProvider inputProvider)
        {
            _inputProvider = inputProvider;
        }

        public int Priority { get; set; } = 0;

        public void FixedUpdate(Scene scene)
        {
            var hardwareInput = _inputProvider.Capture();

            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<InputComponent>())
                {
                    var input = entity.GetComponent<InputComponent>();
                    input.HardwareInput = hardwareInput;

                    HandleActionMappingGroups(input);
                    HandleAxisMappingGroups(input);
                }
            }
        }

        private void HandleActionMappingGroups(InputComponent inputComponent)
        {
            var previousActionStates = new Dictionary<string, bool>(inputComponent.ActionStates);

            ResetActionStates(inputComponent);
            if (inputComponent.InputMapping == null) return;

            var actionMappingGroups = inputComponent.InputMapping.ActionMappingGroups;

            foreach (var actionMappingGroup in actionMappingGroups)
            {
                var actionName = actionMappingGroup.ActionName;

                foreach (var actionMapping in actionMappingGroup.ActionMappings)
                {
                    bool state;

                    var hardwareInputVariant = actionMapping.HardwareInputVariant;
                    switch (hardwareInputVariant.CurrentVariant)
                    {
                        case HardwareInputVariant.Variant.Keyboard:
                            state = inputComponent.HardwareInput.KeyboardInput[hardwareInputVariant.Key];
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    inputComponent.ActionStates[actionName] = state;
                    if (state) break;
                }

                if (inputComponent.ActionBindings.ContainsKey(actionName))
                {
                    previousActionStates.TryGetValue(actionName, out var previousActionState);

                    if (previousActionState == false && inputComponent.GetActionState(actionName))
                        inputComponent.ActionBindings[actionName]();
                }
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

        private void HandleAxisMappingGroups(InputComponent inputComponent)
        {
            ResetAxisStates(inputComponent);
            if (inputComponent.InputMapping == null) return;

            var axisMappingGroups = inputComponent.InputMapping.AxisMappingGroups;

            foreach (var axisMappingGroup in axisMappingGroups)
            {
                var axisName = axisMappingGroup.AxisName;

                foreach (var axisMapping in axisMappingGroup.AxisMappings)
                {
                    double state;

                    var hardwareInputVariant = axisMapping.HardwareInputVariant;
                    switch (hardwareInputVariant.CurrentVariant)
                    {
                        case HardwareInputVariant.Variant.Keyboard:
                            state = BoolToDouble(inputComponent.HardwareInput.KeyboardInput[hardwareInputVariant.Key]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var scaledState = state * axisMapping.Scale;

                    if (inputComponent.AxisStates.ContainsKey(axisName))
                        inputComponent.AxisStates[axisName] += scaledState;
                    else
                        inputComponent.AxisStates[axisName] = scaledState;
                }

                if (inputComponent.AxisBindings.ContainsKey(axisName))
                    inputComponent.AxisBindings[axisName](inputComponent.GetAxisState(axisName));
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

        private static double BoolToDouble(bool b)
        {
            return b ? 1 : 0;
        }
    }
}