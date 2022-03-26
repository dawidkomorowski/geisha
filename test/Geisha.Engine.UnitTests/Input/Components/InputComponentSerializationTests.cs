﻿using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Components
{
    [TestFixture]
    public class InputComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize_WhenInputMappingIsNotNull()
        {
            // Arrange
            var inputMapping = new InputMapping();
            var inputMappingAssetId = AssetId.CreateUnique();

            AssetStore.GetAssetId(inputMapping).Returns(inputMappingAssetId);
            AssetStore.GetAsset<InputMapping>(inputMappingAssetId).Returns(inputMapping);

            // Act
            var actual = SerializeAndDeserialize<InputComponent>(component => { component.InputMapping = inputMapping; });

            // Assert
            Assert.That(actual.InputMapping, Is.EqualTo(inputMapping));
            Assert.That(actual.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(actual.ActionBindings, Is.Empty);
            Assert.That(actual.AxisBindings, Is.Empty);
            Assert.That(actual.ActionStates, Is.Empty);
            Assert.That(actual.AxisStates, Is.Empty);
        }

        [Test]
        public void SerializeAndDeserialize_WhenInputMappingIsNull()
        {
            // Arrange
            // Act
            var actual = SerializeAndDeserialize<InputComponent>(component => { component.InputMapping = null; });

            // Assert
            Assert.That(actual.InputMapping, Is.Null);
            Assert.That(actual.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(actual.ActionBindings, Is.Empty);
            Assert.That(actual.AxisBindings, Is.Empty);
            Assert.That(actual.ActionStates, Is.Empty);
            Assert.That(actual.AxisStates, Is.Empty);
        }
    }
}