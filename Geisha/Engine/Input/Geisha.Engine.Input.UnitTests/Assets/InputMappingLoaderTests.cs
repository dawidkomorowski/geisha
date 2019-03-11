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
        [Test]
        public void Load_ShouldReturnInputMappingWithDataAsDefinedInInputMappingFile()
        {
            // Arrange
            const string filePath = "input mapping file path";
            const string json = "serialized data";

            var inputMappingFileContent = new InputMappingFileContent
            {
                ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                {
                    ["Action 1"] = new[]
                    {
                        new SerializableHardwareAction {Key = Key.Space}
                    },
                    ["Action 2"] = new[]
                    {
                        new SerializableHardwareAction {Key = Key.C},
                        new SerializableHardwareAction {Key = Key.LeftCtrl}
                    }
                },
                AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>
                {
                    ["Axis 1"] = new[]
                    {
                        new SerializableHardwareAxis {Key = Key.Up, Scale = 1.02},
                        new SerializableHardwareAxis {Key = Key.Down, Scale = -1.03}
                    },
                    ["Axis 2"] = new[]
                    {
                        new SerializableHardwareAxis {Key = Key.Right, Scale = 1.05},
                        new SerializableHardwareAxis {Key = Key.Left, Scale = -9.79}
                    }
                }
            };

            var file = Substitute.For<IFile>();
            file.ReadAllText().Returns(json);
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetFile(filePath).Returns(file);
            var jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.Deserialize<InputMappingFileContent>(json).Returns(inputMappingFileContent);
            var inputMappingLoader = new InputMappingLoader(fileSystem, jsonSerializer);

            // Act
            var actual = (InputMapping) inputMappingLoader.Load(filePath);

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