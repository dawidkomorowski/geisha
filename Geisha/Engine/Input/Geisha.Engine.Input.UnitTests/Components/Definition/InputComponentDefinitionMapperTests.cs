using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Components.Definition;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Components.Definition
{
    [TestFixture]
    public class InputComponentDefinitionMapperTests
    {
        private IAssetStore _assetStore;
        private InputComponentDefinitionMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _mapper = new InputComponentDefinitionMapper(_assetStore);
        }

        #region MapToSerializable

        [Test]
        public void ToDefinition_MapsInputMappingToInputMappingAssetId_GivenNotNullInputMapping()
        {
            // Arrange
            var inputMapping = new InputMapping();
            var inputMappingAssetId = Guid.NewGuid();

            var inputComponent = new InputComponent
            {
                InputMapping = inputMapping
            };

            _assetStore.GetAssetId(inputMapping).Returns(inputMappingAssetId);

            // Act
            var actual = (InputComponentDefinition) _mapper.MapToSerializable(inputComponent);

            // Assert
            Assert.That(actual.InputMappingAssetId, Is.EqualTo(inputMappingAssetId));
        }

        [Test]
        public void ToDefinition_SetsInputMappingAssetIdToNull_GivenNullInputMapping()
        {
            // Arrange
            var inputComponent = new InputComponent
            {
                InputMapping = null
            };

            // Act
            var actual = (InputComponentDefinition) _mapper.MapToSerializable(inputComponent);

            // Assert
            Assert.That(actual.InputMappingAssetId, Is.Null);
        }

        #endregion

        #region MapFromSerializable

        [Test]
        public void FromDefinition_MapsInputMappingAssetIdToInputMapping_GivenNotNullInputMappingAssetId()
        {
            // Arrange
            var inputMapping = new InputMapping();
            var inputMappingAssetId = Guid.NewGuid();

            var inputComponentDefinition = new InputComponentDefinition
            {
                InputMappingAssetId = inputMappingAssetId
            };

            _assetStore.GetAsset<InputMapping>(inputMappingAssetId).Returns(inputMapping);

            // Act
            var actual = (InputComponent) _mapper.MapFromSerializable(inputComponentDefinition);

            // Assert
            Assert.That(actual.InputMapping, Is.EqualTo(inputMapping));
            Assert.That(actual.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(actual.ActionBindings, Is.Empty);
            Assert.That(actual.AxisBindings, Is.Empty);
            Assert.That(actual.ActionStates, Is.Empty);
            Assert.That(actual.AxisStates, Is.Empty);
        }

        [Test]
        public void FromDefinition_SetsInputMappingToNull_GivenNullInputMappingAssetId()
        {
            // Arrange
            var inputComponentDefinition = new InputComponentDefinition
            {
                InputMappingAssetId = null
            };


            // Act
            var actual = (InputComponent) _mapper.MapFromSerializable(inputComponentDefinition);

            // Assert
            Assert.That(actual.InputMapping, Is.Null);
            Assert.That(actual.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(actual.ActionBindings, Is.Empty);
            Assert.That(actual.AxisBindings, Is.Empty);
            Assert.That(actual.ActionStates, Is.Empty);
            Assert.That(actual.AxisStates, Is.Empty);
        }

        #endregion
    }
}