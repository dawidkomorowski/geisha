using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Input.Mapping;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Assets
{
    [TestFixture]
    public class InputMappingAssetLoaderTests
    {
        private IFileSystem _fileSystem = null!;
        private IAssetStore _assetStore = null!;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _assetStore = Substitute.For<IAssetStore>();
        }

        #region Load throws exception test cases

        private static IEnumerable<TestCaseData> LoadThrowsExceptionTestCases => new[]
        {
            new TestCaseData(
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Invalid Action"] = new[]
                        {
                            new SerializableHardwareAction()
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                }
            ).SetName("Hardware action does not specify input device."),
            new TestCaseData(
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Invalid Action"] = new[]
                        {
                            new SerializableHardwareAction
                            {
                                Key = Key.Space,
                                MouseButton = MouseButton.Left
                            }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                }
            ).SetName("Hardware action specifies multiple input devices."),
            new TestCaseData(
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>(),
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>
                    {
                        ["Invalid Axis"] = new[]
                        {
                            new SerializableHardwareAxis()
                        }
                    }
                }
            ).SetName("Hardware axis does not specify input device."),
            new TestCaseData(
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>(),
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>
                    {
                        ["Invalid Axis"] = new[]
                        {
                            new SerializableHardwareAxis
                            {
                                Key = Key.Up,
                                MouseAxis = MouseAxis.X
                            }
                        }
                    }
                }
            ).SetName("Hardware axis specifies multiple input devices.")
        };

        [TestCaseSource(nameof(LoadThrowsExceptionTestCases))]
        public void LoadAsset_ThrowsException_WhenFileContentIsInvalid(InputMappingAssetContent inputMappingAssetContent)
        {
            // Arrange
            const string assetFilePath = "input mapping file path";

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), InputAssetTypes.InputMapping, assetFilePath);
            var assetData = AssetData.CreateWithJsonContent(assetInfo.AssetId, assetInfo.AssetType, inputMappingAssetContent);
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            var file = Substitute.For<IFile>();
            file.OpenRead().Returns(memoryStream);
            _fileSystem.GetFile(assetFilePath).Returns(file);

            var inputMappingAssetLoader = new InputMappingAssetLoader(_fileSystem);

            // Act
            // Assert
            Assert.That(() => inputMappingAssetLoader.LoadAsset(assetInfo, _assetStore), Throws.InvalidOperationException);
        }

        #endregion

        #region Load loads input mapping from file test cases

        private static IEnumerable<LoadTestCase> LoadTestCases => new[]
        {
            new LoadTestCase("Single keyboard key mapped as action",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Action Key.Space"] = new[]
                        {
                            new SerializableHardwareAction { Key = Key.Space }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(0));

                    Assert.That(mapping.ActionMappings.Single().ActionName, Is.EqualTo("Action Key.Space"));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions, Has.Length.EqualTo(1));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.Single().InputSource,
                        Is.EqualTo(InputSource.Create(Key.Space)));
                }),
            new LoadTestCase("Multiple keyboard keys mapped as single action",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Action Key.C and Key.LeftCtrl"] = new[]
                        {
                            new SerializableHardwareAction { Key = Key.C },
                            new SerializableHardwareAction { Key = Key.LeftCtrl }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(0));

                    Assert.That(mapping.ActionMappings.Single().ActionName, Is.EqualTo("Action Key.C and Key.LeftCtrl"));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions, Has.Length.EqualTo(2));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.ElementAt(0).InputSource,
                        Is.EqualTo(InputSource.Create(Key.C)));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.ElementAt(1).InputSource,
                        Is.EqualTo(InputSource.Create(Key.LeftCtrl)));
                }),
            new LoadTestCase("Left mouse button mapped as action",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Action MouseButton.Left"] = new[]
                        {
                            new SerializableHardwareAction { MouseButton = MouseButton.Left }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(0));

                    Assert.That(mapping.ActionMappings.Single().ActionName, Is.EqualTo("Action MouseButton.Left"));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions, Has.Length.EqualTo(1));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.Single().InputSource,
                        Is.EqualTo(InputSource.Create(MouseButton.Left)));
                }),
            new LoadTestCase("Middle mouse button mapped as action",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Action MouseButton.Middle"] = new[]
                        {
                            new SerializableHardwareAction { MouseButton = MouseButton.Middle }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(0));

                    Assert.That(mapping.ActionMappings.Single().ActionName, Is.EqualTo("Action MouseButton.Middle"));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions, Has.Length.EqualTo(1));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.Single().InputSource,
                        Is.EqualTo(InputSource.Create(MouseButton.Middle)));
                }),
            new LoadTestCase("Right mouse button mapped as action",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Action MouseButton.Right"] = new[]
                        {
                            new SerializableHardwareAction { MouseButton = MouseButton.Right }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(0));

                    Assert.That(mapping.ActionMappings.Single().ActionName, Is.EqualTo("Action MouseButton.Right"));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions, Has.Length.EqualTo(1));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.Single().InputSource,
                        Is.EqualTo(InputSource.Create(MouseButton.Right)));
                }),
            new LoadTestCase("Extended mouse button 1 mapped as action",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Action MouseButton.XButton1"] = new[]
                        {
                            new SerializableHardwareAction { MouseButton = MouseButton.XButton1 }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(0));

                    Assert.That(mapping.ActionMappings.Single().ActionName, Is.EqualTo("Action MouseButton.XButton1"));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions, Has.Length.EqualTo(1));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.Single().InputSource,
                        Is.EqualTo(InputSource.Create(MouseButton.XButton1)));
                }),
            new LoadTestCase("Extended mouse button 2 mapped as action",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                    {
                        ["Action MouseButton.XButton2"] = new[]
                        {
                            new SerializableHardwareAction { MouseButton = MouseButton.XButton2 }
                        }
                    },
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(0));

                    Assert.That(mapping.ActionMappings.Single().ActionName, Is.EqualTo("Action MouseButton.XButton2"));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions, Has.Length.EqualTo(1));
                    Assert.That(mapping.ActionMappings.Single().HardwareActions.Single().InputSource,
                        Is.EqualTo(InputSource.Create(MouseButton.XButton2)));
                }),
            new LoadTestCase("Multiple keyboard keys mapped as single axis",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>(),
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>
                    {
                        ["Axis Key.Up and Key.Down"] = new[]
                        {
                            new SerializableHardwareAxis { Key = Key.Up, Scale = 1.02 },
                            new SerializableHardwareAxis { Key = Key.Down, Scale = -1.03 }
                        }
                    }
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(0));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(1));

                    Assert.That(mapping.AxisMappings.Single().AxisName, Is.EqualTo("Axis Key.Up and Key.Down"));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes, Has.Length.EqualTo(2));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.ElementAt(0).InputSource,
                        Is.EqualTo(InputSource.Create(Key.Up)));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.ElementAt(0).Scale, Is.EqualTo(1.02));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.ElementAt(1).InputSource,
                        Is.EqualTo(InputSource.Create(Key.Down)));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.ElementAt(1).Scale, Is.EqualTo(-1.03));
                }),
            new LoadTestCase("Mouse axis X mapped as axis",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>(),
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>
                    {
                        ["Axis MouseAxis.X"] = new[]
                        {
                            new SerializableHardwareAxis { MouseAxis = MouseAxis.X, Scale = 1.02 }
                        }
                    }
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(0));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(1));

                    Assert.That(mapping.AxisMappings.Single().AxisName, Is.EqualTo("Axis MouseAxis.X"));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.Single().InputSource,
                        Is.EqualTo(InputSource.Create(MouseAxis.X)));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.Single().Scale, Is.EqualTo(1.02));
                }),
            new LoadTestCase("Mouse axis Y mapped as axis",
                new InputMappingAssetContent
                {
                    ActionMappings = new Dictionary<string, SerializableHardwareAction[]>(),
                    AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>
                    {
                        ["Axis MouseAxis.Y"] = new[]
                        {
                            new SerializableHardwareAxis { MouseAxis = MouseAxis.Y, Scale = 1.02 }
                        }
                    }
                },
                mapping =>
                {
                    Assert.That(mapping.ActionMappings, Has.Length.EqualTo(0));
                    Assert.That(mapping.AxisMappings, Has.Length.EqualTo(1));

                    Assert.That(mapping.AxisMappings.Single().AxisName, Is.EqualTo("Axis MouseAxis.Y"));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes, Has.Length.EqualTo(1));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.Single().InputSource,
                        Is.EqualTo(InputSource.Create(MouseAxis.Y)));
                    Assert.That(mapping.AxisMappings.Single().HardwareAxes.Single().Scale, Is.EqualTo(1.02));
                })
        };

        [TestCaseSource(nameof(LoadTestCases))]
        public void Load_ShouldLoadInputMappingFromFile(LoadTestCase testCase)
        {
            // Arrange
            const string filePath = "input mapping file path";

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), InputAssetTypes.InputMapping, filePath);
            var assetData = AssetData.CreateWithJsonContent(assetInfo.AssetId, assetInfo.AssetType, testCase.InputMappingAssetContent);
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            var file = Substitute.For<IFile>();
            file.OpenRead().Returns(memoryStream);
            _fileSystem.GetFile(filePath).Returns(file);

            var inputMappingAssetLoader = new InputMappingAssetLoader(_fileSystem);

            // Act
            var actual = (InputMapping)inputMappingAssetLoader.LoadAsset(assetInfo, _assetStore);

            // Assert
            testCase.Assert(actual);
        }

        public sealed class LoadTestCase
        {
            private readonly string _name;

            public LoadTestCase(string name, InputMappingAssetContent inputMappingAssetContent, Action<InputMapping> assert)
            {
                _name = name;
                InputMappingAssetContent = inputMappingAssetContent;
                Assert = assert;
            }

            public InputMappingAssetContent InputMappingAssetContent { get; }
            public Action<InputMapping> Assert { get; }

            public override string ToString() => _name;
        }

        #endregion
    }
}