﻿using System;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Components
{
    [TestFixture]
    public class InputComponentTests
    {
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

        [Test]
        public void InputMapping_ShouldClearActionBindings_WhenSet()
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.ActionBindings["ActionName"] = () => { };

            // Act
            inputComponent.InputMapping = new InputMapping();

            // Assert
            Assert.That(inputComponent.ActionBindings, Is.Empty);
        }

        [Test]
        public void InputMapping_ShouldClearAxisBindings_WhenSet()
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.AxisBindings["AxisName"] = value => { };

            // Act
            inputComponent.InputMapping = new InputMapping();

            // Assert
            Assert.That(inputComponent.AxisBindings, Is.Empty);
        }

        [Test]
        public void InputMapping_ShouldClearActionStates_WhenSetWithNoActionMappingGroups()
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.ActionBindings["ActionName"] = () => { };

            // Act
            inputComponent.InputMapping = new InputMapping();

            // Assert
            Assert.That(inputComponent.ActionStates, Is.Empty);
        }

        [Test]
        public void InputMapping_ShouldClearAxisStates_WhenSetWithNoAxisMappingGroups()
        {
            // Arrange
            var inputComponent = new InputComponent();
            inputComponent.AxisBindings["AxisName"] = value => { };

            // Act
            inputComponent.InputMapping = new InputMapping();

            // Assert
            Assert.That(inputComponent.AxisStates, Is.Empty);
        }

        [Test]
        public void InputMapping_ShouldSetDefinedActionStatesToFalse_WhenSetWithActionMappingGroups()
        {
            // Arrange
            var inputComponent = new InputComponent();

            var inputMapping = new InputMapping();

            var actionMappingGroup1 = new ActionMappingGroup {ActionName = "Action 1"};
            var actionMappingGroup2 = new ActionMappingGroup {ActionName = "Action 2"};
            inputMapping.ActionMappingGroups.Add(actionMappingGroup1);
            inputMapping.ActionMappingGroups.Add(actionMappingGroup2);

            // Act
            inputComponent.InputMapping = inputMapping;

            // Assert
            Assert.That(inputComponent.ActionStates.Count, Is.EqualTo(2));
            Assert.That(inputComponent.ActionStates[actionMappingGroup1.ActionName], Is.False);
            Assert.That(inputComponent.ActionStates[actionMappingGroup2.ActionName], Is.False);
        }

        [Test]
        public void InputMapping_ShouldSetDefinedAxisStatesToZero_WhenSetWithAxisMappingGroups()
        {
            // Arrange
            var inputComponent = new InputComponent();

            var inputMapping = new InputMapping();

            var axisMappingGroup1 = new AxisMappingGroup {AxisName = "Axis 1"};
            var axisMappingGroup2 = new AxisMappingGroup {AxisName = "Axis 2"};
            inputMapping.AxisMappingGroups.Add(axisMappingGroup1);
            inputMapping.AxisMappingGroups.Add(axisMappingGroup2);

            // Act
            inputComponent.InputMapping = inputMapping;

            // Assert
            Assert.That(inputComponent.AxisStates.Count, Is.EqualTo(2));
            Assert.That(inputComponent.AxisStates[axisMappingGroup1.AxisName], Is.Zero);
            Assert.That(inputComponent.AxisStates[axisMappingGroup2.AxisName], Is.Zero);
        }

        [Test]
        public void BindAction_ShouldRegisterActionBinding()
        {
            // Arrange
            var inputComponent = new InputComponent();

            var actionName = "ActionName";
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

            var axisName = "AxisName";
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

            var actionName = "ActionName";
            var actionState = true;
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

            var axisName = "AxisName";
            var axisState = Math.PI;
            inputComponent.AxisStates[axisName] = axisState;

            // Act
            var actual = inputComponent.GetAxisState(axisName);

            // Assert
            Assert.That(actual, Is.EqualTo(axisState));
        }
    }
}