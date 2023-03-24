using System;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Components
{
    [TestFixture]
    public class InputComponentTests
    {
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [Test]
        public void Constructor_ShouldSetHardwareInputToEmpty()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.HardwareInput, Is.EqualTo(HardwareInput.Empty));
        }

        [Test]
        public void ActionBindings_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.ActionBindings, Is.Not.Null);
            Assert.That(inputComponent.ActionBindings, Is.Empty);
        }

        [Test]
        public void AxisBindings_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.AxisBindings, Is.Not.Null);
            Assert.That(inputComponent.AxisBindings, Is.Empty);
        }

        [Test]
        public void ActionStates_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.ActionStates, Is.Not.Null);
            Assert.That(inputComponent.ActionStates, Is.Empty);
        }

        [Test]
        public void AxisStates_ShouldBeInitializedWithEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.AxisStates, Is.Not.Null);
            Assert.That(inputComponent.AxisStates, Is.Empty);
        }

        [Test]
        public void HasActionStatesInitialized_ShouldBeInitializedWithFalse()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.HasActionStatesInitialized, Is.False);
        }

        #region InputMapping

        [TestCase(false)]
        [TestCase(true)]
        public void InputMapping_ShouldClearActionBindings_WhenSet(bool inputMappingIsNull)
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
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
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.AxisBindings["AxisName"] = _ => { };

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
            var inputComponent = Entity.CreateComponent<InputComponent>();
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
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.AxisBindings["AxisName"] = _ => { };

            // Act
            inputComponent.InputMapping = GetInputMapping(inputMappingIsNull);

            // Assert
            Assert.That(inputComponent.AxisStates, Is.Empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void InputMapping_ShouldSet_HasActionStatesInitialized_ToFalse_WhenSet(bool inputMappingIsNull)
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.HasActionStatesInitialized = true;

            // Act
            inputComponent.InputMapping = GetInputMapping(inputMappingIsNull);

            // Assert
            Assert.That(inputComponent.HasActionStatesInitialized, Is.False);
        }

        [Test]
        public void InputMapping_ShouldSetDefinedActionStatesToFalse_WhenSetWithActionMappingGroups()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();

            var inputMapping = new InputMapping();

            var actionMapping1 = new ActionMapping { ActionName = "Action 1" };
            var actionMapping2 = new ActionMapping { ActionName = "Action 2" };
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
            var inputComponent = Entity.CreateComponent<InputComponent>();

            var inputMapping = new InputMapping();

            var axisMapping1 = new AxisMapping { AxisName = "Axis 1" };
            var axisMapping2 = new AxisMapping { AxisName = "Axis 2" };
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
            var inputComponent = Entity.CreateComponent<InputComponent>();

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
            var inputComponent = Entity.CreateComponent<InputComponent>();

            const string axisName = "AxisName";
            Action<double> action = _ => { };

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
            var inputComponent = Entity.CreateComponent<InputComponent>();

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
            var inputComponent = Entity.CreateComponent<InputComponent>();

            const string axisName = "AxisName";
            const double axisState = Math.PI;
            inputComponent.AxisStates[axisName] = axisState;

            // Act
            var actual = inputComponent.GetAxisState(axisName);

            // Assert
            Assert.That(actual, Is.EqualTo(axisState));
        }

        [Test]
        public void RemoveActionBinding_ShouldRemoveRegisteredActionBinding()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
            const string actionName = "ActionName";
            inputComponent.BindAction(actionName, () => { });

            // Act
            inputComponent.RemoveActionBinding(actionName);

            // Assert
            Assert.That(inputComponent.ActionBindings, Does.Not.ContainKey(actionName));
        }

        [Test]
        public void RemoveAxisBinding_ShouldRemoveRegisteredAxisBinding()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
            const string axisName = "AxisName";
            inputComponent.BindAxis(axisName, _ => { });

            // Act
            inputComponent.RemoveAxisBinding(axisName);

            // Assert
            Assert.That(inputComponent.AxisBindings, Does.Not.ContainKey(axisName));
        }

        [Test]
        public void RemoveAllBindings_ShouldRemoveRegisteredActionBindingAndAxisBinding()
        {
            // Arrange
            const string actionName = "ActionName";
            const string axisName = "AxisName";

            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.BindAction(actionName, () => { });
            inputComponent.BindAxis(axisName, _ => { });

            // Act
            inputComponent.RemoveAllBindings();

            // Assert
            Assert.That(inputComponent.ActionBindings, Is.Empty);
            Assert.That(inputComponent.AxisBindings, Is.Empty);
        }

        #region Helpers

        private static InputMapping? GetInputMapping(bool inputMappingIsNull)
        {
            return inputMappingIsNull ? null : new InputMapping();
        }

        #endregion
    }
}