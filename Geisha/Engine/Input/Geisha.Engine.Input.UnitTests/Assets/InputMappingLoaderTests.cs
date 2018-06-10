﻿using System.Collections.Generic;
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
            const string filePath = "input mapping file path";

            var inputMappingFile = new InputMappingFile
            {
                ActionMappings = new Dictionary<string, HardwareActionDefinition[]>
                {
                    ["Action 1"] = new[] {new HardwareActionDefinition {Key = Key.Space}},
                    ["Action 2"] = new[] {new HardwareActionDefinition {Key = Key.C}, new HardwareActionDefinition {Key = Key.LeftCtrl}}
                },
                AxisMappings = new Dictionary<string, HardwareAxisDefinition[]>
                {
                    ["Axis 1"] = new[] {new HardwareAxisDefinition {Key = Key.Up, Scale = 1.02}, new HardwareAxisDefinition {Key = Key.Down, Scale = -1.03}},
                    ["Axis 2"] = new[] {new HardwareAxisDefinition {Key = Key.Right, Scale = 1.05}, new HardwareAxisDefinition {Key = Key.Left, Scale = -9.79}}
                }
            };

            _fileSystem.ReadAllTextFromFile(filePath).Returns(Serializer.SerializeJson(inputMappingFile));

            // Act
            var actual = (InputMapping) _inputMappingLoader.Load(filePath);

            // Assert
            // Action mappings
            Assert.That(actual.ActionMappings, Has.Count.EqualTo(2));

            Assert.That(actual.ActionMappings.ElementAt(0).ActionName, Is.EqualTo("Action 1"));
            Assert.That(actual.ActionMappings.ElementAt(0).HardwareActions, Has.Count.EqualTo(1));
            Assert.That(actual.ActionMappings.ElementAt(0).HardwareActions.Single().HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Space)));

            Assert.That(actual.ActionMappings.ElementAt(1).ActionName, Is.EqualTo("Action 2"));
            Assert.That(actual.ActionMappings.ElementAt(1).HardwareActions, Has.Count.EqualTo(2));
            Assert.That(actual.ActionMappings.ElementAt(1).HardwareActions.ElementAt(0).HardwareInputVariant,
                Is.EqualTo(new HardwareInputVariant(Key.C)));
            Assert.That(actual.ActionMappings.ElementAt(1).HardwareActions.ElementAt(1).HardwareInputVariant,
                Is.EqualTo(new HardwareInputVariant(Key.LeftCtrl)));

            // Axis mappings
            Assert.That(actual.AxisMappings, Has.Count.EqualTo(2));

            Assert.That(actual.AxisMappings.ElementAt(0).AxisName, Is.EqualTo("Axis 1"));
            Assert.That(actual.AxisMappings.ElementAt(0).HardwareAxes, Has.Count.EqualTo(2));
            Assert.That(actual.AxisMappings.ElementAt(0).HardwareAxes.ElementAt(0).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Up)));
            Assert.That(actual.AxisMappings.ElementAt(0).HardwareAxes.ElementAt(0).Scale, Is.EqualTo(1.02));
            Assert.That(actual.AxisMappings.ElementAt(0).HardwareAxes.ElementAt(1).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Down)));
            Assert.That(actual.AxisMappings.ElementAt(0).HardwareAxes.ElementAt(1).Scale, Is.EqualTo(-1.03));

            Assert.That(actual.AxisMappings.ElementAt(1).AxisName, Is.EqualTo("Axis 2"));
            Assert.That(actual.AxisMappings.ElementAt(1).HardwareAxes, Has.Count.EqualTo(2));
            Assert.That(actual.AxisMappings.ElementAt(1).HardwareAxes.ElementAt(0).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Right)));
            Assert.That(actual.AxisMappings.ElementAt(1).HardwareAxes.ElementAt(0).Scale, Is.EqualTo(1.05));
            Assert.That(actual.AxisMappings.ElementAt(1).HardwareAxes.ElementAt(1).HardwareInputVariant, Is.EqualTo(new HardwareInputVariant(Key.Left)));
            Assert.That(actual.AxisMappings.ElementAt(1).HardwareAxes.ElementAt(1).Scale, Is.EqualTo(-9.79));
        }
    }
}