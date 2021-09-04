using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    [TestFixture]
    public class AssetStoreTests
    {
        private IFileSystem _fileSystem = null!;
        private IManagedAssetFactory _managedAssetFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            _managedAssetFactory.Create(Arg.Any<AssetInfo>(), Arg.Any<IAssetStore>()).Returns(callInfo =>
            {
                var assetInfo = callInfo.ArgAt<AssetInfo>(0);
                var managedAsset = (IManagedAsset)ManagedAssetSubstitute.Create(assetInfo, new object());
                return SingleOrEmpty.Single(managedAsset);
            });
        }

        private AssetStore GetAssetStore()
        {
            return GetAssetStore(new[] { _managedAssetFactory });
        }

        private AssetStore GetAssetStore(IEnumerable<IManagedAssetFactory> assetFactories)
        {
            return new AssetStore(_fileSystem, assetFactories);
        }

        #region GetAsset

        [Test]
        public void GetAsset_ShouldThrowException_WhenAssetWasNotRegistered()
        {
            // Arrange
            var notRegisteredAssetId = AssetId.CreateUnique();
            var assetStore = GetAssetStore();

            // Act
            // Assert
            Assert.That(() => assetStore.GetAsset<object>(notRegisteredAssetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void GetAsset_ShouldThrowException_WhenThereIsAssetIdMismatch()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var notRegisteredAssetId = AssetId.CreateUnique();
            var assetStore = GetAssetStore();

            assetStore.RegisterAsset(new AssetInfo(assetId, new AssetType("AssetType.Object"), "some file path"));

            // Act
            // Assert
            Assert.That(() => assetStore.GetAsset<object>(notRegisteredAssetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void GetAsset_ShouldThrowException_WhenThereIsAssetTypeMismatch()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetStore = GetAssetStore();
            assetStore.RegisterAsset(new AssetInfo(assetId, new AssetType("AssetType.Object"), "some file path"));

            // Act
            // Assert
            Assert.That(() => assetStore.GetAsset<int>(assetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturnAsset_WhenAssetWasNotYetLoaded()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });
            var managedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore, false);

            var assetId = managedAsset.AssetInfo.AssetId;
            var asset = managedAsset.TestAssetInstance;

            // Assume
            Assume.That(managedAsset.LoadWasCalled, Is.False);

            // Act
            var actual = assetStore.GetAsset<object>(assetId);

            // Assert
            Assert.That(actual, Is.EqualTo(asset));
            Assert.That(managedAsset.LoadWasCalled, Is.True);
        }

        [Test]
        public void GetAsset_ShouldNotLoadAssetAndReturnAlreadyLoadedAsset_WhenAssetWasAlreadyLoaded()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });
            var managedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);

            var assetId = managedAsset.AssetInfo.AssetId;
            var asset = managedAsset.TestAssetInstance;

            // Assume
            Assume.That(managedAsset.LoadWasCalled, Is.True);
            managedAsset.ClearCalledFlags();
            Assume.That(managedAsset.LoadWasCalled, Is.False);

            // Act
            var actual = assetStore.GetAsset<object>(assetId);

            // Assert
            Assert.That(actual, Is.EqualTo(asset));
            Assert.That(managedAsset.LoadWasCalled, Is.False);
        }

        #endregion

        #region GetAssetId

        [Test]
        public void GetAssetId_ShouldThrowException_GivenAssetNotLoadedByAssetStore()
        {
            // Arrange
            var asset = new object();
            var assetStore = GetAssetStore();

            // Act
            // Assert
            Assert.That(() => assetStore.GetAssetId(asset), Throws.ArgumentException);
        }

        [Test]
        public void GetAssetId_ShouldReturnAssetId_GivenAssetLoadedByAssetStore()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });
            var managedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);

            var assetId = managedAsset.AssetInfo.AssetId;
            var asset = managedAsset.TestAssetInstance;

            // Act
            var actual = assetStore.GetAssetId(asset);

            // Assert
            Assert.That(actual, Is.EqualTo(assetId));
        }

        #endregion

        #region RegisterAsset

        [Test]
        public void RegisterAsset_ShouldThrowException_WhenThereIsNoAssetFactory()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var assetStore = GetAssetStore(Enumerable.Empty<IManagedAssetFactory>());

            // Act
            // Assert
            Assert.That(() => { assetStore.RegisterAsset(assetInfo); }, Throws.TypeOf<AssetFactoryNotFoundException>());
        }

        [Test]
        public void RegisterAsset_ShouldThrowException_WhenThereAreMultipleAssetFactoriesForGivenAssetInfo()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var managedAssetFactory1 = Substitute.For<IManagedAssetFactory>();
            var managedAssetFactory2 = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory1, managedAssetFactory2 });

            var managedAsset = Substitute.For<IManagedAsset>();
            managedAssetFactory1.Create(assetInfo, assetStore).Returns(SingleOrEmpty.Single(managedAsset));
            managedAssetFactory2.Create(assetInfo, assetStore).Returns(SingleOrEmpty.Single(managedAsset));

            // Act
            // Assert
            Assert.That(() => { assetStore.RegisterAsset(assetInfo); }, Throws.TypeOf<MultipleAssetFactoriesFoundException>());
        }

        [Test]
        public void RegisterAsset_ShouldRegisterAssetInAssetStore()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var assetStore = GetAssetStore();

            // Act
            assetStore.RegisterAsset(assetInfo);

            // Assert
            var actual = assetStore.GetRegisteredAssets().Single();
            Assert.That(actual, Is.EqualTo(assetInfo));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RegisterAsset_ShouldUnloadAlreadyRegisteredAsset_WhenAssetWithTheSameIdWasAlreadyRegisteredAndLoaded(bool wasLoaded)
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();

            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });

            var managedAsset1 = ManagedAssetSubstitute.Create(assetInfo, new object());
            var managedAsset2 = ManagedAssetSubstitute.Create(assetInfo, new object());
            managedAssetFactory.Create(assetInfo, assetStore).Returns(SingleOrEmpty.Single((IManagedAsset)managedAsset1));
            assetStore.RegisterAsset(assetInfo);

            if (wasLoaded) assetStore.GetAsset<object>(assetInfo.AssetId);

            managedAssetFactory.Create(assetInfo, assetStore).Returns(SingleOrEmpty.Single((IManagedAsset)managedAsset2));

            // Act
            assetStore.RegisterAsset(assetInfo);

            // Assert
            Assert.That(managedAsset1.UnloadWasCalled, Is.EqualTo(wasLoaded));
        }

        [TestCase("AssetType.Object", "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            "AssetType.Object", "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", true)]
        [TestCase("AssetType.Object", "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            "AssetType.Object", "345E30DC-5F18-472C-B539-15ECE44B6B60", "other file path", true)]
        [TestCase("AssetType.Object", "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            "AssetType.Int", "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", true)]
        [TestCase("AssetType.Object", "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            "AssetType.Object", "C7CF6FFC-FF65-48D8-BF1B-041E51F8E1C4", "some file path", false)]
        public void RegisterAsset_ShouldOverrideAlreadyRegisteredAsset_WhenAssetWithTheSameIdWasAlreadyRegistered(
            string assetTypeString1, string assetIdString1, string assetFilePath1,
            string assetTypeString2, string assetIdString2, string assetFilePath2,
            bool overridden)
        {
            // Arrange
            var assetId1 = new AssetId(new Guid(assetIdString1));
            var assetId2 = new AssetId(new Guid(assetIdString2));

            var assetType1 = new AssetType(assetTypeString1);
            var assetType2 = new AssetType(assetTypeString2);

            var assetInfo1 = new AssetInfo(assetId1, assetType1, assetFilePath1);
            var assetInfo2 = new AssetInfo(assetId2, assetType2, assetFilePath2);

            var assetStore = GetAssetStore();

            assetStore.RegisterAsset(assetInfo1);

            // Act
            assetStore.RegisterAsset(assetInfo2);

            // Assert
            if (overridden)
            {
                var registeredAssets = assetStore.GetRegisteredAssets().ToList();
                Assert.That(registeredAssets, Has.Exactly(1).Items);
                var registeredAssetInfo = registeredAssets.Single();
                Assert.That(registeredAssetInfo, Is.EqualTo(assetInfo2));
            }
            else
            {
                var registeredAssets = assetStore.GetRegisteredAssets().ToList();
                Assert.That(registeredAssets, Has.Exactly(2).Items);
            }
        }

        #endregion

        #region RegisterAssets

        [Test]
        public void RegisterAssets_ShouldSearchInSpecifiedDirectory()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";

            // Act
            GetAssetStore().RegisterAssets(assetDiscoveryPath);

            // Assert
            _fileSystem.Received().GetDirectory(assetDiscoveryPath);
        }

        [Test]
        public void RegisterAssets_ShouldRegisterNoAssetInfos_WhenDirectoryHasNoFiles()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";
            var root = new DirectoryStub();

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var assetStore = GetAssetStore();

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Is.Empty);
        }

        [Test]
        public void RegisterAssets_ShouldRegisterThreeAssetInfos_WhenDirectoryHasThreeAssetFiles()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";

            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();
            var assetInfo3 = CreateNewAssetInfo();


            var root = new DirectoryStub
            {
                Files = new[]
                {
                    CreateAssetFile(assetInfo1),
                    CreateAssetFile(assetInfo2),
                    CreateAssetFile(assetInfo3)
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var assetStore = GetAssetStore();

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Has.Exactly(3).Items);
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo1));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo2));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo3));
        }

        [Test]
        public void RegisterAssets_ShouldRegisterOneAssetInfo_WhenDirectoryHasThreeFiles_ButOnlyOneAssetFile()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";
            var assetInfo = CreateNewAssetInfo();

            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>(),
                    CreateAssetFile(assetInfo)
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var assetStore = GetAssetStore();

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Has.One.Items);
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo));
        }

        [Test]
        public void RegisterAssets_ShouldRegisterTenAssetInfos_WhenDirectoryHasFilesAndSubdirectoriesWithFilesAllHavingTenAssetFilesInTotal()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";

            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();
            var assetInfo3 = CreateNewAssetInfo();
            var assetInfo4 = CreateNewAssetInfo();
            var assetInfo5 = CreateNewAssetInfo();
            var assetInfo6 = CreateNewAssetInfo();
            var assetInfo7 = CreateNewAssetInfo();
            var assetInfo8 = CreateNewAssetInfo();
            var assetInfo9 = CreateNewAssetInfo();
            var assetInfo10 = CreateNewAssetInfo();

            var root = new DirectoryStub
            {
                Files = new[]
                {
                    CreateAssetFile(assetInfo1),
                    CreateAssetFile(assetInfo2)
                },
                Directories = new[]
                {
                    new DirectoryStub
                    {
                        Files = new[]
                        {
                            CreateAssetFile(assetInfo3),
                            CreateAssetFile(assetInfo4)
                        }
                    },
                    new DirectoryStub
                    {
                        Files = new[]
                        {
                            CreateAssetFile(assetInfo5),
                            CreateAssetFile(assetInfo6)
                        },
                        Directories = new[]
                        {
                            new DirectoryStub
                            {
                                Files = new[]
                                {
                                    CreateAssetFile(assetInfo7),
                                    CreateAssetFile(assetInfo8)
                                }
                            },
                            new DirectoryStub
                            {
                                Files = new[]
                                {
                                    CreateAssetFile(assetInfo9),
                                    CreateAssetFile(assetInfo10)
                                }
                            }
                        }
                    }
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var assetStore = GetAssetStore();

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Has.Exactly(10).Items);
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo1));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo2));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo3));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo4));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo5));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo6));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo7));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo8));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo9));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo10));
        }

        #endregion

        #region UnloadAsset

        [Test]
        public void UnloadAsset_ShouldThrowException_GivenIdOfNotRegisteredAsset()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();

            var assetStore = GetAssetStore();

            // Act
            // Assert
            Assert.That(() => { assetStore.UnloadAsset(assetId); }, Throws.TypeOf<AssetNotRegisteredException>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void UnloadAsset_ShouldUnloadAsset_WhenAssetIsLoaded_GivenItsId(bool assetIsLoaded)
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });
            var managedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore, assetIsLoaded);

            var assetId = managedAsset.AssetInfo.AssetId;

            // Act
            assetStore.UnloadAsset(assetId);

            // Assert
            Assert.That(managedAsset.UnloadWasCalled, Is.EqualTo(assetIsLoaded));
        }

        [Test]
        public void UnloadAsset_ShouldMakeAssetIdUnavailable()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });
            var managedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);

            var assetId = managedAsset.AssetInfo.AssetId;
            var asset = managedAsset.TestAssetInstance;

            // Act
            assetStore.UnloadAsset(assetId);

            // Assert
            Assert.That(() => { assetStore.GetAssetId(asset); }, Throws.ArgumentException);
        }

        #endregion

        #region UnloadAssets

        [Test]
        public void UnloadAssets_ShouldUnloadAllLoadedAssets()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });

            var loadedManagedAsset1 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var loadedManagedAsset2 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var loadedManagedAsset3 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var notLoadedManagedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore, false);

            // Act
            assetStore.UnloadAssets();

            // Assert
            Assert.That(loadedManagedAsset1.UnloadWasCalled, Is.True);
            Assert.That(loadedManagedAsset2.UnloadWasCalled, Is.True);
            Assert.That(loadedManagedAsset3.UnloadWasCalled, Is.True);
            Assert.That(notLoadedManagedAsset.UnloadWasCalled, Is.False);
        }

        [Test]
        public void UnloadAssets_ShouldMakeAllAssetsIdsUnavailable()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });

            var managedAsset1 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var managedAsset2 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var managedAsset3 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);

            var asset1 = managedAsset1.TestAssetInstance;
            var asset2 = managedAsset2.TestAssetInstance;
            var asset3 = managedAsset3.TestAssetInstance;

            // Act
            assetStore.UnloadAssets();

            // Assert
            Assert.That(() => { assetStore.GetAssetId(asset1); }, Throws.ArgumentException);
            Assert.That(() => { assetStore.GetAssetId(asset2); }, Throws.ArgumentException);
            Assert.That(() => { assetStore.GetAssetId(asset3); }, Throws.ArgumentException);
        }

        #endregion

        #region Dispose

        [Test]
        public void Dispose_ShouldUnloadAllLoadedAssets()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });

            var loadedManagedAsset1 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var loadedManagedAsset2 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var loadedManagedAsset3 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var notLoadedManagedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore, false);

            // Act
            assetStore.Dispose();

            // Assert
            Assert.That(loadedManagedAsset1.UnloadWasCalled, Is.True);
            Assert.That(loadedManagedAsset2.UnloadWasCalled, Is.True);
            Assert.That(loadedManagedAsset3.UnloadWasCalled, Is.True);
            Assert.That(notLoadedManagedAsset.UnloadWasCalled, Is.False);
        }

        [Test]
        public void Dispose_ShouldMakeAllAssetsIdsUnavailable()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] { managedAssetFactory });

            var managedAsset1 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var managedAsset2 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);
            var managedAsset3 = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);

            var asset1 = managedAsset1.TestAssetInstance;
            var asset2 = managedAsset2.TestAssetInstance;
            var asset3 = managedAsset3.TestAssetInstance;

            // Act
            assetStore.Dispose();

            // Assert
            Assert.That(() => { assetStore.GetAssetId(asset1); }, Throws.ArgumentException);
            Assert.That(() => { assetStore.GetAssetId(asset2); }, Throws.ArgumentException);
            Assert.That(() => { assetStore.GetAssetId(asset3); }, Throws.ArgumentException);
        }

        #endregion

        #region Helpers

        private static AssetInfo CreateNewAssetInfo()
        {
            return new AssetInfo(AssetId.CreateUnique(), new AssetType("AssetType.Object"), "asset.object");
        }

        private static IFile CreateAssetFile(AssetInfo assetInfo)
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns(Utils.Random.GetString());
            file.Extension.Returns(AssetFileExtension.Value);
            file.Path.Returns(assetInfo.AssetFilePath);

            var memoryStream = new MemoryStream();
            var assetData = AssetData.CreateWithStringContent(assetInfo.AssetId, assetInfo.AssetType, Utils.Random.GetString());
            assetData.Save(memoryStream);
            memoryStream.Position = 0;
            file.OpenRead().Returns(memoryStream);

            return file;
        }

        private class DirectoryStub : IDirectory
        {
            public string Name { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public IEnumerable<IDirectory> Directories { get; set; } = Enumerable.Empty<IDirectory>();
            public IEnumerable<IFile> Files { get; set; } = Enumerable.Empty<IFile>();
        }

        private static IManagedAssetSubstitute MockAndRegisterManagedAsset(IManagedAssetFactory managedAssetFactory, IAssetStore assetStore,
            bool loadAsset = true)
        {
            var assetId = AssetId.CreateUnique();
            var assetInfo = new AssetInfo(assetId, new AssetType("AssetType.Object"), "some file path");
            var asset = new object();

            var managedAsset = ManagedAssetSubstitute.Create(assetInfo, asset);
            managedAssetFactory.Create(assetInfo, assetStore).Returns(SingleOrEmpty.Single(managedAsset));

            assetStore.RegisterAsset(assetInfo);

            if (loadAsset)
            {
                assetStore.GetAsset<object>(assetId);
            }

            return managedAsset;
        }

        #endregion
    }
}