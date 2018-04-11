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

        [Test]
        public void ToDefinition()
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
            var actual = (InputComponentDefinition) _mapper.ToDefinition(inputComponent);

            // Assert
            Assert.That(actual.InputMappingAssetId, Is.EqualTo(inputMappingAssetId));
        }

        [Test]
        public void FromDefinition()
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
            var actual = (InputComponent) _mapper.FromDefinition(inputComponentDefinition);

            // Assert
            Assert.That(actual.InputMapping, Is.EqualTo(inputMapping));
            Assert.That(actual.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(actual.ActionBindings, Is.Empty);
            Assert.That(actual.AxisBindings, Is.Empty);
            Assert.That(actual.ActionStates, Is.Empty);
            Assert.That(actual.AxisStates, Is.Empty);
        }
    }
}