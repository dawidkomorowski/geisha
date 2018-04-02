using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Input;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Assets
{
    [TestFixture]
    public class InputMappingLoaderTests
    {
        private IFileSystem _fileSystem;
        private InputMappingLoader _inputMappingLoader;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _inputMappingLoader = new InputMappingLoader(_fileSystem);
        }

        [Test]
        public void Load_ShouldReturnInputMappingWithDataAsDefinedInInputMappingFile()
        {
            // Arrange
            var inputMappingFile = new InputMappingFile
            {
                ActionMappings = new Dictionary<string, ActionMappingDefinition[]>
                {
                    ["Action 1"] = new[] {new ActionMappingDefinition {Key = Key.Space}},
                    ["Action 2"] = new[] {new ActionMappingDefinition {Key = Key.C}, new ActionMappingDefinition {Key = Key.LeftCtrl}}
                },
                AxisMappings = new Dictionary<string, AxisMappingDefinition[]>
                {
                    ["Axis 1"] = new[] {new AxisMappingDefinition {Key = Key.Up, Scale = 1.02}, new AxisMappingDefinition {Key = Key.Down, Scale = -1.03}},
                    ["Axis 2"] = new[] {new AxisMappingDefinition {Key = Key.Right, Scale = 1.05}, new AxisMappingDefinition {Key = Key.Left, Scale = -9.79}}
                }
            };

            _fileSystem.ReadAllTextFromFile("input mapping file path").Returns(Serializer.SerializeJson(inputMappingFile));

            // Act
            var actual = (InputMapping) _inputMappingLoader.Load("input mapping file path");

            // Assert
            // Action mappings
            Assert.That(actual.ActionMappingGroups, Has.Count.EqualTo(2));

            Assert.That(actual.ActionMappingGroups.ElementAt(0).ActionName, Is.EqualTo("Action 1"));
            Assert.That(actual.ActionMappingGroups.ElementAt(0).ActionMappings, Has.Count.EqualTo(1));
            Assert.That(actual.ActionMappingGroups.ElementAt(0).ActionMappings.Single().HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Space)));

            Assert.That(actual.ActionMappingGroups.ElementAt(1).ActionName, Is.EqualTo("Action 2"));
            Assert.That(actual.ActionMappingGroups.ElementAt(1).ActionMappings, Has.Count.EqualTo(2));
            Assert.That(actual.ActionMappingGroups.ElementAt(1).ActionMappings.ElementAt(0).HardwareInputVariant,
                Is.EqualTo(new HardwareInputVariant(Key.C)));
            Assert.That(actual.ActionMappingGroups.ElementAt(1).ActionMappings.ElementAt(1).HardwareInputVariant,
                Is.EqualTo(new HardwareInputVariant(Key.LeftCtrl)));

            // Axis mappings
            Assert.That(actual.AxisMappingGroups, Has.Count.EqualTo(2));

            Assert.That(actual.AxisMappingGroups.ElementAt(0).AxisName, Is.EqualTo("Axis 1"));
            Assert.That(actual.AxisMappingGroups.ElementAt(0).AxisMappings, Has.Count.EqualTo(2));
            Assert.That(actual.AxisMappingGroups.ElementAt(0).AxisMappings.ElementAt(0).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Up)));
            Assert.That(actual.AxisMappingGroups.ElementAt(0).AxisMappings.ElementAt(0).Scale, Is.EqualTo(1.02));
            Assert.That(actual.AxisMappingGroups.ElementAt(0).AxisMappings.ElementAt(1).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Down)));
            Assert.That(actual.AxisMappingGroups.ElementAt(0).AxisMappings.ElementAt(1).Scale, Is.EqualTo(-1.03));

            Assert.That(actual.AxisMappingGroups.ElementAt(1).AxisName, Is.EqualTo("Axis 2"));
            Assert.That(actual.AxisMappingGroups.ElementAt(1).AxisMappings, Has.Count.EqualTo(2));
            Assert.That(actual.AxisMappingGroups.ElementAt(1).AxisMappings.ElementAt(0).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Right)));
            Assert.That(actual.AxisMappingGroups.ElementAt(1).AxisMappings.ElementAt(0).Scale, Is.EqualTo(1.05));
            Assert.That(actual.AxisMappingGroups.ElementAt(1).AxisMappings.ElementAt(1).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Left)));
            Assert.That(actual.AxisMappingGroups.ElementAt(1).AxisMappings.ElementAt(1).Scale, Is.EqualTo(-9.79));
        }
    }
}