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

        #region Constructor

        [Test]
        public void Constructor_ShouldSet_HardwareInput_ToEmpty()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.HardwareInput, Is.EqualTo(HardwareInput.Empty));
        }

        [Test]
        public void Constructor_ShouldSet_HasActionStatesInitialized_ToFalse()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.HasActionStatesInitialized, Is.False);
        }

        [Test]
        public void Constructor_ShouldSet_Enabled_ToTrue()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.Enabled, Is.True);
        }

        [Test]
        public void Constructor_ShouldSet_ActionBindings_ToEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.ActionBindings, Is.Not.Null);
            Assert.That(inputComponent.ActionBindings, Is.Empty);
        }

        [Test]
        public void Constructor_ShouldSet_AxisBindings_ToEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.AxisBindings, Is.Not.Null);
            Assert.That(inputComponent.AxisBindings, Is.Empty);
        }

        [Test]
        public void Constructor_ShouldSet_ActionStates_ToEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.ActionStates, Is.Not.Null);
            Assert.That(inputComponent.ActionStates, Is.Empty);
        }

        [Test]
        public void Constructor_ShouldSet_AxisStates_ToEmptyDictionary()
        {
            // Arrange
            // Act
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Assert
            Assert.That(inputComponent.AxisStates, Is.Not.Null);
            Assert.That(inputComponent.AxisStates, Is.Empty);
        }

        #endregion

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

        #region Enabled

        [TestCase(true)]
        [TestCase(false)]
        public void Enabled_ShouldSet_Enabled(bool enabled)
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();

            // Act
            inputComponent.Enabled = enabled;

            // Assert
            Assert.That(inputComponent.Enabled, Is.EqualTo(enabled));
        }

        [Test]
        public void Enabled_ShouldSet_HardwareInput_ToEmpty_GivenFalse()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.HardwareInput = new HardwareInput(new KeyboardInput(), new MouseInput());

            // Act
            inputComponent.Enabled = false;

            // Assert
            Assert.That(inputComponent.HardwareInput, Is.EqualTo(HardwareInput.Empty));
        }

        [Test]
        public void Enabled_ShouldKeepExisting_HardwareInput_GivenTrue()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
            var hardwareInput = new HardwareInput(new KeyboardInput(), new MouseInput());
            inputComponent.HardwareInput = hardwareInput;

            // Act
            inputComponent.Enabled = true;

            // Assert
            Assert.That(inputComponent.HardwareInput, Is.EqualTo(hardwareInput));
        }

        [Test]
        public void Enabled_ShouldSet_HasActionStatesInitialized_ToFalse_GivenFalse()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.HasActionStatesInitialized = true;

            // Act
            inputComponent.Enabled = false;

            // Assert
            Assert.That(inputComponent.HasActionStatesInitialized, Is.False);
        }

        [Test]
        public void Enabled_ShouldKeepExisting_HasActionStatesInitialized_GivenTrue()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();
            inputComponent.HasActionStatesInitialized = true;

            // Act
            inputComponent.Enabled = true;

            // Assert
            Assert.That(inputComponent.HasActionStatesInitialized, Is.True);
        }

        [Test]
        public void Enabled_ShouldSet_ActionStates_ToFalse_GivenFalse()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();

            var inputMapping = new InputMapping();

            var actionMapping1 = new ActionMapping { ActionName = "Action 1" };
            var actionMapping2 = new ActionMapping { ActionName = "Action 2" };
            inputMapping.ActionMappings.Add(actionMapping1);
            inputMapping.ActionMappings.Add(actionMapping2);
            inputComponent.InputMapping = inputMapping;

            inputComponent.ActionStates[actionMapping1.ActionName] = true;
            inputComponent.ActionStates[actionMapping2.ActionName] = true;

            // Act
            inputComponent.Enabled = false;

            // Assert
            Assert.That(inputComponent.GetActionState(actionMapping1.ActionName), Is.False);
            Assert.That(inputComponent.GetActionState(actionMapping2.ActionName), Is.False);
        }

        [Test]
        public void Enabled_ShouldKeepExisting_ActionStates_GivenTrue()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();

            var inputMapping = new InputMapping();

            var actionMapping1 = new ActionMapping { ActionName = "Action 1" };
            var actionMapping2 = new ActionMapping { ActionName = "Action 2" };
            inputMapping.ActionMappings.Add(actionMapping1);
            inputMapping.ActionMappings.Add(actionMapping2);
            inputComponent.InputMapping = inputMapping;

            inputComponent.ActionStates[actionMapping1.ActionName] = true;
            inputComponent.ActionStates[actionMapping2.ActionName] = true;

            // Act
            inputComponent.Enabled = true;

            // Assert
            Assert.That(inputComponent.GetActionState(actionMapping1.ActionName), Is.True);
            Assert.That(inputComponent.GetActionState(actionMapping2.ActionName), Is.True);
        }

        [Test]
        public void Enabled_ShouldSet_AxisStates_ToZero_GivenFalse()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();

            var inputMapping = new InputMapping();

            var axisMapping1 = new AxisMapping { AxisName = "Axis 1" };
            var axisMapping2 = new AxisMapping { AxisName = "Axis 2" };
            inputMapping.AxisMappings.Add(axisMapping1);
            inputMapping.AxisMappings.Add(axisMapping2);
            inputComponent.InputMapping = inputMapping;

            inputComponent.AxisStates[axisMapping1.AxisName] = 1d;
            inputComponent.AxisStates[axisMapping2.AxisName] = 1d;

            // Act
            inputComponent.Enabled = false;

            // Assert
            Assert.That(inputComponent.GetAxisState(axisMapping1.AxisName), Is.Zero);
            Assert.That(inputComponent.GetAxisState(axisMapping2.AxisName), Is.Zero);
        }

        [Test]
        public void Enabled_ShouldKeepExisting_AxisStates_GivenTrue()
        {
            // Arrange
            var inputComponent = Entity.CreateComponent<InputComponent>();

            var inputMapping = new InputMapping();

            var axisMapping1 = new AxisMapping { AxisName = "Axis 1" };
            var axisMapping2 = new AxisMapping { AxisName = "Axis 2" };
            inputMapping.AxisMappings.Add(axisMapping1);
            inputMapping.AxisMappings.Add(axisMapping2);
            inputComponent.InputMapping = inputMapping;

            inputComponent.AxisStates[axisMapping1.AxisName] = 1d;
            inputComponent.AxisStates[axisMapping2.AxisName] = 1d;

            // Act
            inputComponent.Enabled = true;

            // Assert
            Assert.That(inputComponent.GetAxisState(axisMapping1.AxisName), Is.EqualTo(1d));
            Assert.That(inputComponent.GetAxisState(axisMapping2.AxisName), Is.EqualTo(1d));
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