using System;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Components
{
    [TestFixture]
    public class InputComponentTests
    {
        [Test]
        public void Constructor_ShouldSetHardwareInputToEmpty()
        {
            // Arrange
            // Act
            var inputComponent = new InputComponent();

            // Assert
            Assert.That(inputComponent.HardwareInput, Is.EqualTo(HardwareInput.Empty));
        }

        [Test]
        public void ActionBindings_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = new InputComponent();

            // Assert
            Assert.That(inputComponent.ActionBindings, Is.Not.Null);
            Assert.That(inputComponent.ActionBindings, Is.Empty);
        }

        [Test]
        public void AxisBindings_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = new InputComponent();

            // Assert
            Assert.That(inputComponent.AxisBindings, Is.Not.Null);
            Assert.That(inputComponent.AxisBindings, Is.Empty);
        }

        [Test]
        public void ActionStates_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = new InputComponent();

            // Assert
            Assert.That(inputComponent.ActionStates, Is.Not.Null);
            Assert.That(inputComponent.ActionStates, Is.Empty);
        }

        [Test]
        public void AxisStates_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = new InputComponent();

            // Assert
            Assert.That(inputComponent.AxisStates, Is.Not.Null);
            Assert.That(inputComponent.AxisStates, Is.Empty);
        }

        #region InputMapping

        [TestCase(false)]
        [TestCase(true)]
        public void InputMapping_ShouldClearActionBindings_WhenSet(bool inputMappingIsNull)
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.ActionBindings["ActionName"] = () => { };

            // Act
            inputComponent.InputMapping = GetInputMapping(inputMappingIsNull);

            // Assert
            Assert.That(inputComponent.ActionBindings, Is.Empty);
        }


        [TestCase(false)]
        [TestCase(true)]
        public void InputMapping_ShouldClearAxisBindings_WhenSet(bool inputMappingIsNull)
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.AxisBindings["AxisName"] = value => { };

            // Act
            inputComponent.InputMapping = GetInputMapping(inputMappingIsNull);

            // Assert
            Assert.That(inputComponent.AxisBindings, Is.Empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void InputMapping_ShouldClearActionStates_WhenSetWithNoActionMappingGroups(bool inputMappingIsNull)
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.ActionBindings["ActionName"] = () => { };

            // Act
            inputComponent.InputMapping = GetInputMapping(inputMappingIsNull);

            // Assert
            Assert.That(inputComponent.ActionStates, Is.Empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void InputMapping_ShouldClearAxisStates_WhenSetWithNoAxisMappingGroups(bool inputMappingIsNull)
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.AxisBindings["AxisName"] = value => { };

            // Act
            inputComponent.InputMapping = GetInputMapping(inputMappingIsNull);

            // Assert
            Assert.That(inputComponent.AxisStates, Is.Empty);
        }

        [Test]
        public void InputMapping_ShouldSetDefinedActionStatesToFalse_WhenSetWithActionMappingGroups()
        {
            // Arrange
            var inputComponent = new InputComponent();

            var inputMapping = new InputMapping();

            var actionMapping1 = new ActionMapping {ActionName = "Action 1"};
            var actionMapping2 = new ActionMapping {ActionName = "Action 2"};
            inputMapping.ActionMappings.Add(actionMapping1);
            inputMapping.ActionMappings.Add(actionMapping2);

            // Act
            inputComponent.InputMapping = inputMapping;

            // Assert
            Assert.That(inputComponent.ActionStates.Count, Is.EqualTo(2));
            Assert.That(inputComponent.ActionStates[actionMapping1.ActionName], Is.False);
            Assert.That(inputComponent.ActionStates[actionMapping2.ActionName], Is.False);
        }

        [Test]
        public void InputMapping_ShouldSetDefinedAxisStatesToZero_WhenSetWithAxisMappingGroups()
        {
            // Arrange
            var inputComponent = new InputComponent();

            var inputMapping = new InputMapping();

            var axisMapping1 = new AxisMapping {AxisName = "Axis 1"};
            var axisMapping2 = new AxisMapping {AxisName = "Axis 2"};
            inputMapping.AxisMappings.Add(axisMapping1);
            inputMapping.AxisMappings.Add(axisMapping2);

            // Act
            inputComponent.InputMapping = inputMapping;

            // Assert
            Assert.That(inputComponent.AxisStates.Count, Is.EqualTo(2));
            Assert.That(inputComponent.AxisStates[axisMapping1.AxisName], Is.Zero);
            Assert.That(inputComponent.AxisStates[axisMapping2.AxisName], Is.Zero);
        }

        #endregion

        [Test]
        public void BindAction_ShouldRegisterActionBinding()
        {
            // Arrange
            var inputComponent = new InputComponent();

            const string actionName = "ActionName";
            Action action = () => { };

            // Act
            inputComponent.BindAction(actionName, action);

            // Assert
            Assert.That(inputComponent.ActionBindings.ContainsKey(actionName), Is.True);
            Assert.That(inputComponent.ActionBindings[actionName], Is.EqualTo(action));
        }

        [Test]
        public void BindAxis_ShouldRegisterAxisBinding()
        {
            // Arrange
            var inputComponent = new InputComponent();

            const string axisName = "AxisName";
            Action<double> action = value => { };

            // Act
            inputComponent.BindAxis(axisName, action);

            // Assert
            Assert.That(inputComponent.AxisBindings.ContainsKey(axisName), Is.True);
            Assert.That(inputComponent.AxisBindings[axisName], Is.EqualTo(action));
        }

        [Test]
        public void GetActionState_ShouldReturnActionState()
        {
            // Arrange
            var inputComponent = new InputComponent();

            const string actionName = "ActionName";
            const bool actionState = true;
            inputComponent.ActionStates[actionName] = actionState;

            // Act
            var actual = inputComponent.GetActionState(actionName);

            // Assert
            Assert.That(actual, Is.EqualTo(actionState));
        }

        [Test]
        public void GetAxisState_ShouldReturnAxisState()
        {
            // Arrange
            var inputComponent = new InputComponent();

            const string axisName = "AxisName";
            const double axisState = Math.PI;
            inputComponent.AxisStates[axisName] = axisState;

            // Act
            var actual = inputComponent.GetAxisState(axisName);

            // Assert
            Assert.That(actual, Is.EqualTo(axisState));
        }

        #region Helpers

        private static InputMapping GetInputMapping(bool inputMappingIsNull)
        {
            return inputMappingIsNull ? null : new InputMapping();
        }

        #endregion
    }
}