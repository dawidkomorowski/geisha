using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Components.Serialization;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Components.Serialization
{
    [TestFixture]
    public class SerializableInputComponentMapperTests
    {
        private IAssetStore _assetStore;
        private SerializableInputComponentMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _mapper = new SerializableInputComponentMapper(_assetStore);
        }

        #region MapToSerializable

        [Test]
        public void MapToSerializable_MapsInputMappingToInputMappingAssetId_GivenNotNullInputMapping()
        {
            // Arrange
            var inputMapping = new InputMapping();
            var inputMappingAssetId = Guid.NewGuid();

            var inputComponent = new InputComponent
            {
                InputMapping = inputMapping
            };

            _assetStore.GetAssetId(inputMapping).Returns(new AssetId(inputMappingAssetId));

            // Act
            var actual = (SerializableInputComponent) _mapper.MapToSerializable(inputComponent);

            // Assert
            Assert.That(actual.InputMappingAssetId, Is.EqualTo(inputMappingAssetId));
        }

        [Test]
        public void MapToSerializable_SetsInputMappingAssetIdToNull_GivenNullInputMapping()
        {
            // Arrange
            var inputComponent = new InputComponent
            {
                InputMapping = null
            };

            // Act
            var actual = (SerializableInputComponent) _mapper.MapToSerializable(inputComponent);

            // Assert
            Assert.That(actual.InputMappingAssetId, Is.Null);
        }

        #endregion

        #region MapFromSerializable

        [Test]
        public void MapFromSerializable_MapsInputMappingAssetIdToInputMapping_GivenNotNullInputMappingAssetId()
        {
            // Arrange
            var inputMapping = new InputMapping();
            var inputMappingAssetId = Guid.NewGuid();

            var serializableInputComponent = new SerializableInputComponent
            {
                InputMappingAssetId = inputMappingAssetId
            };

            _assetStore.GetAsset<InputMapping>(new AssetId(inputMappingAssetId)).Returns(inputMapping);

            // Act
            var actual = (InputComponent) _mapper.MapFromSerializable(serializableInputComponent);

            // Assert
            Assert.That(actual.InputMapping, Is.EqualTo(inputMapping));
            Assert.That(actual.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(actual.ActionBindings, Is.Empty);
            Assert.That(actual.AxisBindings, Is.Empty);
            Assert.That(actual.ActionStates, Is.Empty);
            Assert.That(actual.AxisStates, Is.Empty);
        }

        [Test]
        public void MapFromSerializable_SetsInputMappingToNull_GivenNullInputMappingAssetId()
        {
            // Arrange
            var serializableInputComponent = new SerializableInputComponent
            {
                InputMappingAssetId = null
            };

            // Act
            var actual = (InputComponent) _mapper.MapFromSerializable(serializableInputComponent);

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