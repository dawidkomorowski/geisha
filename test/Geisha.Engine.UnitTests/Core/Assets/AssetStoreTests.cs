using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    [TestFixture]
    public class AssetStoreTests
    {
        private IFileSystem _fileSystem;
        private IManagedAssetFactory _managedAssetFactory;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            _managedAssetFactory.Create(Arg.Any<AssetInfo>(), Arg.Any<IAssetStore>()).Returns(callInfo =>
            {
                var assetInfo = callInfo.ArgAt<AssetInfo>(0);
                var managedAsset = ManagedAssetSubstitute.Create(assetInfo, new object());
                return SingleOrEmpty.Single(managedAsset);
            });
        }

        private AssetStore GetAssetStore()
        {
            return GetAssetStore(Enumerable.Empty<IAssetDiscoveryRule>());
        }

        private AssetStore GetAssetStore(IEnumerable<IAssetDiscoveryRule> assetDiscoveryRules)
        {
            return GetAssetStore(assetDiscoveryRules, new[] {_managedAssetFactory});
        }

        private AssetStore GetAssetStore(IEnumerable<IManagedAssetFactory> assetFactories)
        {
            return GetAssetStore(Enumerable.Empty<IAssetDiscoveryRule>(), assetFactories);
        }

        private AssetStore GetAssetStore(IEnumerable<IAssetDiscoveryRule> assetDiscoveryRules, IEnumerable<IManagedAssetFactory> assetFactories)
        {
            return new AssetStore(_fileSystem, assetDiscoveryRules, assetFactories);
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

            assetStore.RegisterAsset(new AssetInfo(assetId, typeof(object), "some file path"));

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
            assetStore.RegisterAsset(new AssetInfo(assetId, typeof(object), "some file path"));

            // Act
            // Assert
            Assert.That(() => assetStore.GetAsset<int>(assetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturnAsset_WhenAssetWasNotYetLoaded()
        {
            // Arrange
            var managedAssetFactory = Substitute.For<IManagedAssetFactory>();
            var assetStore = GetAssetStore(new[] {managedAssetFactory});
            var managedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore, false);

            var assetId = managedAsset.AssetInfo.AssetId;
            var asset = managedAsset.TestAssetInstance;

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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});
            var managedAsset = MockAndRegisterManagedAsset(managedAssetFactory, assetStore);

            var assetId = managedAsset.AssetInfo.AssetId;
            var asset = managedAsset.TestAssetInstance;

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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});
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
            var assetStore = GetAssetStore(new[] {managedAssetFactory1, managedAssetFactory2});

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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});

            var managedAsset1 = ManagedAssetSubstitute.Create(assetInfo, new object());
            var managedAsset2 = ManagedAssetSubstitute.Create(assetInfo, new object());
            managedAssetFactory.Create(assetInfo, assetStore).Returns(SingleOrEmpty.Single(managedAsset1));
            assetStore.RegisterAsset(assetInfo);

            if (wasLoaded) assetStore.GetAsset<object>(assetInfo.AssetId);

            managedAssetFactory.Create(assetInfo, assetStore).Returns(SingleOrEmpty.Single(managedAsset2));

            // Act
            assetStore.RegisterAsset(assetInfo);

            // Assert
            Assert.That(managedAsset1.UnloadWasCalled, Is.EqualTo(wasLoaded));
        }

        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", true)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "other file path", true)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(int), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", true)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "C7CF6FFC-FF65-48D8-BF1B-041E51F8E1C4", "some file path", false)]
        public void RegisterAsset_ShouldOverrideAlreadyRegisteredAsset_WhenAssetWithTheSameIdWasAlreadyRegistered(Type assetType1, string assetIdString1,
            string assetFilePath1, Type assetType2, string assetIdString2, string assetFilePath2, bool overridden)
        {
            // Arrange
            var assetId1 = new AssetId(new Guid(assetIdString1));
            var assetId2 = new AssetId(new Guid(assetIdString2));

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
            GetAssetStore(Enumerable.Empty<IAssetDiscoveryRule>()).RegisterAssets(assetDiscoveryPath);

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

            var allFilesAreAssetsDiscoveryRule = Substitute.For<IAssetDiscoveryRule>();
            allFilesAreAssetsDiscoveryRule.Discover(Arg.Any<IFile>()).Returns(_ => SingleOrEmpty.Single(CreateNewAssetInfo()));

            var assetStore = GetAssetStore(new[] {allFilesAreAssetsDiscoveryRule});

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Is.Empty);
        }

        [Test]
        public void RegisterAssets_ShouldRegisterNoAssetInfos_WhenThereAreNoDiscoveryRules()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";
            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>()
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var assetStore = GetAssetStore(Enumerable.Empty<IAssetDiscoveryRule>());

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Is.Empty);
        }

        [Test]
        public void RegisterAssets_ShouldRegisterThreeAssetInfos_WhenDirectoryHasThreeFiles_AndThereIsMatchingRule()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";
            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>()
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var allFilesAreAssetsDiscoveryRule = Substitute.For<IAssetDiscoveryRule>();
            allFilesAreAssetsDiscoveryRule.Discover(Arg.Any<IFile>()).Returns(_ => SingleOrEmpty.Single(CreateNewAssetInfo()));

            var assetStore = GetAssetStore(new[] {allFilesAreAssetsDiscoveryRule});

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Has.Exactly(3).Items);
        }

        [Test]
        public void RegisterAssets_ShouldRegisterOneAssetInfo_WhenDirectoryHasThreeFiles_AndOnlyOneMatchesDiscoveryRule()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";
            var matchingFile = Substitute.For<IFile>();
            var assetInfo = CreateNewAssetInfo();

            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>(),
                    matchingFile
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var singleFileIsAssetDiscoveryRule = Substitute.For<IAssetDiscoveryRule>();
            singleFileIsAssetDiscoveryRule.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Empty<AssetInfo>());
            singleFileIsAssetDiscoveryRule.Discover(matchingFile).Returns(SingleOrEmpty.Single(assetInfo));

            var assetStore = GetAssetStore(new[] {singleFileIsAssetDiscoveryRule});

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Has.One.Items);
            Assert.That(assetStore.GetRegisteredAssets().Single(), Is.EqualTo(assetInfo));
        }

        [Test]
        public void
            RegisterAssets_ShouldRegisterTwoAssetInfos_WhenDirectoryHasThreeFilesAndTwoDiscoveryRules_AndFirstFileMatchesFirstRule_AndSecondFileMatchesSecondRule()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";
            var file1 = Substitute.For<IFile>();
            var file2 = Substitute.For<IFile>();
            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();

            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    file1,
                    file2
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var assetDiscoveryRule1 = Substitute.For<IAssetDiscoveryRule>();
            assetDiscoveryRule1.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Empty<AssetInfo>());
            assetDiscoveryRule1.Discover(file1).Returns(SingleOrEmpty.Single(assetInfo1));

            var assetDiscoveryRule2 = Substitute.For<IAssetDiscoveryRule>();
            assetDiscoveryRule2.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Empty<AssetInfo>());
            assetDiscoveryRule2.Discover(file2).Returns(SingleOrEmpty.Single(assetInfo2));

            var assetStore = GetAssetStore(new[] {assetDiscoveryRule1, assetDiscoveryRule2});

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Has.Exactly(2).Items);
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo1));
            Assert.That(assetStore.GetRegisteredAssets(), Contains.Item(assetInfo2));
        }

        [Test]
        public void RegisterAssets_ShouldRegisterTenAssetInfos_WhenDirectoryHasFilesAndSubdirectoriesWithFilesAllHavingTenFilesInTotal_AndThereIsMatchingRule()
        {
            // Arrange
            const string assetDiscoveryPath = "Assets Root Directory";
            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>()
                },
                Directories = new[]
                {
                    new DirectoryStub
                    {
                        Files = new[]
                        {
                            Substitute.For<IFile>(),
                            Substitute.For<IFile>()
                        }
                    },
                    new DirectoryStub
                    {
                        Files = new[]
                        {
                            Substitute.For<IFile>(),
                            Substitute.For<IFile>()
                        },
                        Directories = new[]
                        {
                            new DirectoryStub
                            {
                                Files = new[]
                                {
                                    Substitute.For<IFile>(),
                                    Substitute.For<IFile>()
                                }
                            },
                            new DirectoryStub
                            {
                                Files = new[]
                                {
                                    Substitute.For<IFile>(),
                                    Substitute.For<IFile>()
                                }
                            }
                        }
                    }
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var allFilesAreAssetsDiscoveryRule = Substitute.For<IAssetDiscoveryRule>();
            allFilesAreAssetsDiscoveryRule.Discover(Arg.Any<IFile>()).Returns(_ => SingleOrEmpty.Single(CreateNewAssetInfo()));

            var assetStore = GetAssetStore(new[] {allFilesAreAssetsDiscoveryRule});

            // Act
            assetStore.RegisterAssets(assetDiscoveryPath);

            // Assert
            Assert.That(assetStore.GetRegisteredAssets(), Has.Exactly(10).Items);
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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});
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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});
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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});

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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});

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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});

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
            var assetStore = GetAssetStore(new[] {managedAssetFactory});

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
            return new AssetInfo(AssetId.CreateUnique(), typeof(object), "asset.object");
        }

        private class DirectoryStub : IDirectory
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public IEnumerable<IDirectory> Directories { get; set; } = Enumerable.Empty<IDirectory>();
            public IEnumerable<IFile> Files { get; set; } = Enumerable.Empty<IFile>();
        }

        private static IManagedAssetSubstitute MockAndRegisterManagedAsset(IManagedAssetFactory managedAssetFactory, IAssetStore assetStore,
            bool loadAsset = true)
        {
            var assetId = AssetId.CreateUnique();
            var assetInfo = new AssetInfo(assetId, typeof(object), "some file path");
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