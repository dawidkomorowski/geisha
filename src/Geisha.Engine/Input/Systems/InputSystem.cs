using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Systems;

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
            if (!inputComponent.Enabled) continue;

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
        if (inputComponent.InputMapping == null) return;

        if (!inputComponent.HasActionStatesInitialized)
        {
            InitializeActionStates(inputComponent);
            inputComponent.HasActionStatesInitialized = true;
            return;
        }

        var actionMappings = inputComponent.InputMapping.ActionMappings;

        foreach (var actionMapping in actionMappings)
        {
            var actionName = actionMapping.ActionName;
            var previousState = inputComponent.ActionStates[actionName];
            var currentState = false;

            foreach (var hardwareAction in actionMapping.HardwareActions)
            {
                currentState = ComputeState(inputComponent.HardwareInput, hardwareAction);
                inputComponent.ActionStates[actionName] = currentState;
                if (currentState) break;
            }

            if (!previousState && currentState)
            {
                if (inputComponent.ActionBindings.TryGetValue(actionName, out var binding))
                {
                    binding();
                }
            }
        }
    }

    private static void InitializeActionStates(InputComponent inputComponent)
    {
        Debug.Assert(inputComponent.InputMapping != null, "inputComponent.InputMapping != null");
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
        }
    }

    private static bool ComputeState(HardwareInput hardwareInput, HardwareAction hardwareAction)
    {
        var hardwareInputVariant = hardwareAction.HardwareInputVariant;
        return hardwareInputVariant.CurrentVariant switch
        {
            HardwareInputVariant.Variant.Keyboard => hardwareInput.KeyboardInput[hardwareInputVariant.AsKeyboard()],
            HardwareInputVariant.Variant.Mouse => hardwareInputVariant.AsMouse() switch
            {
                HardwareInputVariant.MouseVariant.LeftButton => hardwareInput.MouseInput.LeftButton,
                HardwareInputVariant.MouseVariant.MiddleButton => hardwareInput.MouseInput.MiddleButton,
                HardwareInputVariant.MouseVariant.RightButton => hardwareInput.MouseInput.RightButton,
                HardwareInputVariant.MouseVariant.XButton1 => hardwareInput.MouseInput.XButton1,
                HardwareInputVariant.MouseVariant.XButton2 => hardwareInput.MouseInput.XButton2,
                _ => throw new InvalidOperationException($"Unexpected {nameof(HardwareInputVariant.MouseVariant)}: {hardwareInputVariant.AsMouse()}")
            },
            _ => throw new InvalidOperationException($"Unexpected {nameof(HardwareInputVariant.Variant)}: {hardwareInputVariant.CurrentVariant}")
        };
    }

    #endregion

    #region Axis mapping handling

    private static void HandleAxisMappings(InputComponent inputComponent)
    {
        if (inputComponent.InputMapping == null) return;

        var axisMappings = inputComponent.InputMapping.AxisMappings;

        foreach (var axisMapping in axisMappings)
        {
            var axisName = axisMapping.AxisName;
            var currentState = 0d;

            foreach (var hardwareAxis in axisMapping.HardwareAxes)
            {
                var state = ComputeState(inputComponent.HardwareInput, hardwareAxis);
                currentState += state * hardwareAxis.Scale;
            }

            inputComponent.AxisStates[axisName] = currentState;

            if (inputComponent.AxisBindings.TryGetValue(axisName, out var binding))
            {
                binding(currentState);
            }
        }
    }

    private static double ComputeState(HardwareInput hardwareInput, HardwareAxis hardwareAxis)
    {
        var hardwareInputVariant = hardwareAxis.HardwareInputVariant;
        return hardwareInputVariant.CurrentVariant switch
        {
            HardwareInputVariant.Variant.Keyboard => BoolToDouble(hardwareInput.KeyboardInput[hardwareInputVariant.AsKeyboard()]),
            HardwareInputVariant.Variant.Mouse => hardwareInputVariant.AsMouse() switch
            {
                HardwareInputVariant.MouseVariant.AxisX => hardwareInput.MouseInput.PositionDelta.X,
                HardwareInputVariant.MouseVariant.AxisY => -hardwareInput.MouseInput.PositionDelta.Y,
                _ => throw new InvalidOperationException($"Unexpected {nameof(HardwareInputVariant.MouseVariant)}: {hardwareInputVariant.AsMouse()}")
            },
            _ => throw new InvalidOperationException($"Unexpected {nameof(HardwareInputVariant.Variant)}: {hardwareInputVariant.CurrentVariant}")
        };
    }

    #endregion
}