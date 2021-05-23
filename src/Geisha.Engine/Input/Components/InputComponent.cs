using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Components
{
    /// <summary>
    ///     <see cref="InputComponent" /> gives an <see cref="Entity" /> capability to access user input both direct hardware
    ///     input and mapped logical input.
    /// </summary>
    [ComponentId("Geisha.Engine.Input.InputComponent")]
    public sealed class InputComponent : Component
    {
        private InputMapping? _inputMapping;

        internal IDictionary<string, Action> ActionBindings { get; } = new Dictionary<string, Action>();
        internal IDictionary<string, Action<double>> AxisBindings { get; } = new Dictionary<string, Action<double>>();
        internal IDictionary<string, bool> ActionStates { get; } = new Dictionary<string, bool>();
        internal IDictionary<string, double> AxisStates { get; } = new Dictionary<string, double>();

        /// <summary>
        ///     Input mapping attached to input component.
        /// </summary>
        public InputMapping? InputMapping
        {
            get => _inputMapping;
            set
            {
                _inputMapping = value;

                ActionBindings.Clear();
                AxisBindings.Clear();
                ActionStates.Clear();
                AxisStates.Clear();

                if (_inputMapping == null) return;

                foreach (var actionMapping in _inputMapping.ActionMappings)
                {
                    ActionStates[actionMapping.ActionName] = false;
                }

                foreach (var axisMapping in _inputMapping.AxisMappings)
                {
                    AxisStates[axisMapping.AxisName] = 0;
                }
            }
        }

        /// <summary>
        ///     Hardware input captured directly from input devices.
        /// </summary>
        public HardwareInput HardwareInput { get; internal set; } = HardwareInput.Empty;

        /// <summary>
        ///     Binds an <see cref="Action" /> to given action name that will be executed whenever action becomes active.
        /// </summary>
        /// <param name="actionName">Name of action to be bound.</param>
        /// <param name="action">Action to be executed.</param>
        /// <remarks>
        ///     Bound <see cref="Action" /> is executed once when action becomes active. Action needs to become inactive and
        ///     active again to execute bound <see cref="Action" /> again.
        /// </remarks>
        public void BindAction(string actionName, Action action)
        {
            ActionBindings[actionName] = action;
        }

        /// <summary>
        ///     Binds an <see cref="Action" /> to given axis name that will be executed every fixed update.
        /// </summary>
        /// <param name="axisName">Name of axis to be bound.</param>
        /// <param name="action">Action to be executed. Double parameter accepted by action is state of axis.</param>
        public void BindAxis(string axisName, Action<double> action)
        {
            AxisBindings[axisName] = action;
        }

        /// <summary>
        ///     Gets state of given action.
        /// </summary>
        /// <param name="actionName">
        ///     Action name. If action with given name was not defined in <see cref="InputMapping" /> this
        ///     method throws an exception.
        /// </param>
        /// <returns>True if action is active (associated key is pressed); otherwise false.</returns>
        public bool GetActionState(string actionName) => ActionStates[actionName];

        /// <summary>
        ///     Gets state of given axis.
        /// </summary>
        /// <param name="axisName">
        ///     Axis name. If axis with given name was not defined in <see cref="InputMapping" /> this method
        ///     throws an exception.
        /// </param>
        /// <returns>
        ///     Non zero if axis is triggered; otherwise zero. Can be positive and negative depending on axis tilt direction.
        ///     Amount depends on how much axis is triggered.
        /// </returns>
        public double GetAxisState(string axisName) => AxisStates[axisName];

        /// <summary>
        ///     Removes existing action bound to specified action name.
        /// </summary>
        /// <param name="actionName">Name of bound action.</param>
        public void RemoveActionBinding(string actionName)
        {
            ActionBindings.Remove(actionName);
        }

        /// <summary>
        ///     Removes existing action bound to specified axis name.
        /// </summary>
        /// <param name="axisBinding">Name of bound axis.</param>
        public void RemoveAxisBinding(string axisBinding)
        {
            AxisBindings.Remove(axisBinding);
        }
    }

    internal sealed class InputComponentFactory : ComponentFactory<InputComponent>
    {
        protected override InputComponent CreateComponent() => new InputComponent();
    }
}