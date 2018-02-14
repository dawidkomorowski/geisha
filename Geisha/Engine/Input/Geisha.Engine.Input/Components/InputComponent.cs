using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Components
{
    public class InputComponent : IComponent
    {
        private InputMapping _inputMapping;

        internal IDictionary<string, Action> ActionBindings { get; } = new Dictionary<string, Action>();
        internal IDictionary<string, Action<double>> AxisBindings { get; } = new Dictionary<string, Action<double>>();
        internal IDictionary<string, bool> ActionStates { get; } = new Dictionary<string, bool>();
        internal IDictionary<string, double> AxisStates { get; } = new Dictionary<string, double>();

        public InputMapping InputMapping
        {
            get => _inputMapping;
            set
            {
                _inputMapping = value;

                ActionBindings.Clear();
                AxisBindings.Clear();
                ActionStates.Clear();
                AxisStates.Clear();

                foreach (var actionMappingGroup in _inputMapping.ActionMappingGroups)
                {
                    ActionStates[actionMappingGroup.ActionName] = false;
                }
                foreach (var axisMappingGroup in _inputMapping.AxisMappingGroups)
                {
                    AxisStates[axisMappingGroup.AxisName] = 0;
                }
            }
        }

        public HardwareInput HardwareInput { get; internal set; }

        public void BindAction(string actionName, Action action)
        {
            ActionBindings[actionName] = action;
        }

        public void BindAxis(string axisName, Action<double> action)
        {
            AxisBindings[axisName] = action;
        }

        public bool GetActionState(string actionName)
        {
            return ActionStates[actionName];
        }

        public double GetAxisState(string axisName)
        {
            return AxisStates[axisName];
        }
    }
}