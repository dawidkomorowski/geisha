using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    [TestFixture]
    public class AssetStoreTests
    {
        private IFileSystem _fileSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
        }

        private AssetStore GetAssetStore()
        {
            return GetAssetStore(CreateObjectAssetLoader());
        }

        private AssetStore GetAssetStore(params IAssetLoader[] assetLoaders)
        {
            return new AssetStore(_fileSystem, assetLoaders);
        }

        #region Constructor

        [Test]
        public void Constructor_ShouldThrowException_GivenMultipleAssetLoadersForTheSameAssetType()
        {
            // Arrange
            var assetLoader1 = CreateObjectAssetLoader();
            var assetLoader2 = CreateObjectAssetLoader();

            // Act
            // Assert
            Assert.That(() => new AssetStore(_fileSystem, new[] { assetLoader1, assetLoader2 }),
                Throws.ArgumentException.And.Message.Contains("Multiple asset loaders for the same asset type"));
        }

        #endregion

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
            var notRegisteredAssetId = AssetId.CreateUnique();
            var assetStore = GetAssetStore();

            assetStore.RegisterAsset(CreateNewAssetInfo());

            // Act
            // Assert
            Assert.That(() => assetStore.GetAsset<object>(notRegisteredAssetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void GetAsset_ShouldThrowException_WhenThereIsAssetTypeMismatch()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var assetStore = GetAssetStore();
            assetStore.RegisterAsset(assetInfo);

            // Act
            // Assert
            Assert.That(() => assetStore.GetAsset<int>(assetInfo.AssetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturnAsset_WhenAssetWasNotYetLoaded()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);

            // Act
            var actual = assetStore.GetAsset<object>(assetInfo.AssetId);

            // Assert
            Assert.That(actual, Is.EqualTo(asset));
            assetLoader.Received(1).LoadAsset(assetInfo, assetStore);
            Assert.That(assetStore.GetAssetId(asset), Is.EqualTo(assetInfo.AssetId));
        }

        [Test]
        public void GetAsset_ShouldNotLoadAssetAndReturnAlreadyLoadedAsset_WhenAssetWasAlreadyLoaded()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);

            // Assume
            assetStore.GetAsset<object>(assetInfo.AssetId);
            assetLoader.Received(1).LoadAsset(assetInfo, assetStore);
            assetLoader.ClearReceivedCalls();

            // Act
            var actual = assetStore.GetAsset<object>(assetInfo.AssetId);

            // Assert
            Assert.That(actual, Is.EqualTo(asset));
            assetLoader.DidNotReceive().LoadAsset(assetInfo, assetStore);
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
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);
            assetStore.GetAsset<object>(assetInfo.AssetId);

            // Act
            var actual = assetStore.GetAssetId(asset);

            // Assert
            Assert.That(actual, Is.EqualTo(assetInfo.AssetId));
        }

        #endregion

        #region RegisterAsset

        [Test]
        public void RegisterAsset_ShouldThrowException_WhenThereIsNoAssetLoader()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var assetStore = GetAssetStore(Array.Empty<IAssetLoader>());

            // Act
            // Assert
            Assert.That(() => { assetStore.RegisterAsset(assetInfo); }, Throws.TypeOf<AssetLoaderNotFoundException>());
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
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);

            if (wasLoaded) assetStore.GetAsset<object>(assetInfo.AssetId);

            // Act
            assetStore.RegisterAsset(assetInfo);

            // Assert
            assetLoader.Received(wasLoaded ? 1 : 0).UnloadAsset(asset);
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
            var assetId1 = AssetId.Parse(assetIdString1);
            var assetId2 = AssetId.Parse(assetIdString2);

            var assetType1 = new AssetType(assetTypeString1);
            var assetType2 = new AssetType(assetTypeString2);

            var assetInfo1 = new AssetInfo(assetId1, assetType1, assetFilePath1);
            var assetInfo2 = new AssetInfo(assetId2, assetType2, assetFilePath2);

            var assetStore = GetAssetStore(CreateObjectAssetLoader(), CreateObjectAssetLoader<int>(new AssetType("AssetType.Int")));

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

        #region LoadAsset

        [Test]
        public void LoadAsset_ShouldThrowException_WhenAssetWasNotRegistered()
        {
            // Arrange
            var notRegisteredAssetId = AssetId.CreateUnique();
            var assetStore = GetAssetStore();

            // Act
            // Assert
            Assert.That(() => assetStore.LoadAsset(notRegisteredAssetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void LoadAsset_ShouldThrowException_WhenThereIsAssetIdMismatch()
        {
            // Arrange
            var notRegisteredAssetId = AssetId.CreateUnique();
            var assetStore = GetAssetStore();

            assetStore.RegisterAsset(CreateNewAssetInfo());

            // Act
            // Assert
            Assert.That(() => assetStore.LoadAsset(notRegisteredAssetId), Throws.TypeOf<AssetNotRegisteredException>());
        }

        [Test]
        public void LoadAsset_ShouldLoadAsset_WhenAssetWasNotYetLoaded()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);

            // Act
            assetStore.LoadAsset(assetInfo.AssetId);

            // Assert
            assetLoader.Received(1).LoadAsset(assetInfo, assetStore);
            Assert.That(assetStore.GetAssetId(asset), Is.EqualTo(assetInfo.AssetId));
        }

        [Test]
        public void LoadAsset_ShouldNotLoadAsset_WhenAssetWasAlreadyLoaded()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);

            // Assume
            assetStore.LoadAsset(assetInfo.AssetId);
            assetLoader.Received(1).LoadAsset(assetInfo, assetStore);
            assetLoader.ClearReceivedCalls();

            // Act
            assetStore.LoadAsset(assetInfo.AssetId);

            // Assert
            assetLoader.DidNotReceive().LoadAsset(assetInfo, assetStore);
            Assert.That(assetStore.GetAssetId(asset), Is.EqualTo(assetInfo.AssetId));
        }

        #endregion

        #region LoadAssets

        [Test]
        public void LoadAssets_ShouldLoadAllRegisteredAssets()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo1 = CreateNewAssetInfo();
            var asset1 = new object();
            assetLoader.LoadAsset(assetInfo1, assetStore).Returns(asset1);

            var assetInfo2 = CreateNewAssetInfo();
            var asset2 = new object();
            assetLoader.LoadAsset(assetInfo2, assetStore).Returns(asset2);

            var assetInfo3 = CreateNewAssetInfo();
            var asset3 = new object();
            assetLoader.LoadAsset(assetInfo3, assetStore).Returns(asset3);

            assetStore.RegisterAsset(assetInfo1);
            assetStore.RegisterAsset(assetInfo2);
            assetStore.RegisterAsset(assetInfo3);

            // Act
            assetStore.LoadAssets();

            // Assert
            assetLoader.Received(1).LoadAsset(assetInfo1, assetStore);
            assetLoader.Received(1).LoadAsset(assetInfo2, assetStore);
            assetLoader.Received(1).LoadAsset(assetInfo3, assetStore);
            Assert.That(assetStore.GetAssetId(asset1), Is.EqualTo(assetInfo1.AssetId));
            Assert.That(assetStore.GetAssetId(asset2), Is.EqualTo(assetInfo2.AssetId));
            Assert.That(assetStore.GetAssetId(asset3), Is.EqualTo(assetInfo3.AssetId));
        }

        [Test]
        public void LoadAssets_ShouldNotLoadAssets_WhenAssetsAreAlreadyLoaded()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo1 = CreateNewAssetInfo();
            var asset1 = new object();
            assetLoader.LoadAsset(assetInfo1, assetStore).Returns(asset1);

            var assetInfo2 = CreateNewAssetInfo();
            var asset2 = new object();
            assetLoader.LoadAsset(assetInfo2, assetStore).Returns(asset2);

            var assetInfo3 = CreateNewAssetInfo();
            var asset3 = new object();
            assetLoader.LoadAsset(assetInfo3, assetStore).Returns(asset3);

            assetStore.RegisterAsset(assetInfo1);
            assetStore.RegisterAsset(assetInfo2);
            assetStore.RegisterAsset(assetInfo3);

            // Assume
            assetStore.LoadAsset(assetInfo2.AssetId);
            assetLoader.Received(1).LoadAsset(assetInfo2, assetStore);
            assetLoader.ClearReceivedCalls();

            // Act
            assetStore.LoadAssets();

            // Assert
            assetLoader.Received(1).LoadAsset(assetInfo1, assetStore);
            assetLoader.DidNotReceive().LoadAsset(assetInfo2, assetStore);
            assetLoader.Received(1).LoadAsset(assetInfo3, assetStore);
            Assert.That(assetStore.GetAssetId(asset1), Is.EqualTo(assetInfo1.AssetId));
            Assert.That(assetStore.GetAssetId(asset2), Is.EqualTo(assetInfo2.AssetId));
            Assert.That(assetStore.GetAssetId(asset3), Is.EqualTo(assetInfo3.AssetId));
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
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);

            if (assetIsLoaded)
            {
                assetStore.GetAsset<object>(assetInfo.AssetId);
            }

            // Act
            assetStore.UnloadAsset(assetInfo.AssetId);

            // Assert
            assetLoader.Received(assetIsLoaded ? 1 : 0).UnloadAsset(asset);
        }

        [Test]
        public void UnloadAsset_ShouldMakeAssetIdUnavailable()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            assetLoader.LoadAsset(assetInfo, assetStore).Returns(asset);

            assetStore.RegisterAsset(assetInfo);
            assetStore.GetAsset<object>(assetInfo.AssetId);

            // Act
            assetStore.UnloadAsset(assetInfo.AssetId);

            // Assert
            Assert.That(() => { assetStore.GetAssetId(asset); }, Throws.ArgumentException);
        }

        #endregion

        #region UnloadAssets

        [Test]
        public void UnloadAssets_ShouldUnloadAllLoadedAssets()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();
            var assetInfo3 = CreateNewAssetInfo();
            var assetInfo4 = CreateNewAssetInfo();

            var asset1 = new object();
            var asset2 = new object();
            var asset3 = new object();
            var asset4 = new object();

            assetLoader.LoadAsset(assetInfo1, assetStore).Returns(asset1);
            assetLoader.LoadAsset(assetInfo2, assetStore).Returns(asset2);
            assetLoader.LoadAsset(assetInfo3, assetStore).Returns(asset3);
            assetLoader.LoadAsset(assetInfo4, assetStore).Returns(asset4);

            assetStore.RegisterAsset(assetInfo1);
            assetStore.RegisterAsset(assetInfo2);
            assetStore.RegisterAsset(assetInfo3);
            assetStore.RegisterAsset(assetInfo4);

            assetStore.GetAsset<object>(assetInfo1.AssetId);
            assetStore.GetAsset<object>(assetInfo2.AssetId);
            assetStore.GetAsset<object>(assetInfo3.AssetId);

            // Act
            assetStore.UnloadAssets();

            // Assert
            assetLoader.Received(1).UnloadAsset(asset1);
            assetLoader.Received(1).UnloadAsset(asset2);
            assetLoader.Received(1).UnloadAsset(asset3);
            assetLoader.DidNotReceive().UnloadAsset(asset4);
        }

        [Test]
        public void UnloadAssets_ShouldMakeAllAssetsIdsUnavailable()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();
            var assetInfo3 = CreateNewAssetInfo();

            var asset1 = new object();
            var asset2 = new object();
            var asset3 = new object();

            assetLoader.LoadAsset(assetInfo1, assetStore).Returns(asset1);
            assetLoader.LoadAsset(assetInfo2, assetStore).Returns(asset2);
            assetLoader.LoadAsset(assetInfo3, assetStore).Returns(asset3);

            assetStore.RegisterAsset(assetInfo1);
            assetStore.RegisterAsset(assetInfo2);
            assetStore.RegisterAsset(assetInfo3);

            assetStore.GetAsset<object>(assetInfo1.AssetId);
            assetStore.GetAsset<object>(assetInfo2.AssetId);
            assetStore.GetAsset<object>(assetInfo3.AssetId);

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
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();
            var assetInfo3 = CreateNewAssetInfo();
            var assetInfo4 = CreateNewAssetInfo();

            var asset1 = new object();
            var asset2 = new object();
            var asset3 = new object();
            var asset4 = new object();

            assetLoader.LoadAsset(assetInfo1, assetStore).Returns(asset1);
            assetLoader.LoadAsset(assetInfo2, assetStore).Returns(asset2);
            assetLoader.LoadAsset(assetInfo3, assetStore).Returns(asset3);
            assetLoader.LoadAsset(assetInfo4, assetStore).Returns(asset4);

            assetStore.RegisterAsset(assetInfo1);
            assetStore.RegisterAsset(assetInfo2);
            assetStore.RegisterAsset(assetInfo3);
            assetStore.RegisterAsset(assetInfo4);

            assetStore.GetAsset<object>(assetInfo1.AssetId);
            assetStore.GetAsset<object>(assetInfo2.AssetId);
            assetStore.GetAsset<object>(assetInfo3.AssetId);

            // Act
            assetStore.Dispose();

            // Assert
            assetLoader.Received(1).UnloadAsset(asset1);
            assetLoader.Received(1).UnloadAsset(asset2);
            assetLoader.Received(1).UnloadAsset(asset3);
            assetLoader.DidNotReceive().UnloadAsset(asset4);
        }

        [Test]
        public void Dispose_ShouldMakeAllAssetsIdsUnavailable()
        {
            // Arrange
            var assetLoader = CreateObjectAssetLoader();
            var assetStore = GetAssetStore(assetLoader);

            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();
            var assetInfo3 = CreateNewAssetInfo();

            var asset1 = new object();
            var asset2 = new object();
            var asset3 = new object();

            assetLoader.LoadAsset(assetInfo1, assetStore).Returns(asset1);
            assetLoader.LoadAsset(assetInfo2, assetStore).Returns(asset2);
            assetLoader.LoadAsset(assetInfo3, assetStore).Returns(asset3);

            assetStore.RegisterAsset(assetInfo1);
            assetStore.RegisterAsset(assetInfo2);
            assetStore.RegisterAsset(assetInfo3);

            assetStore.GetAsset<object>(assetInfo1.AssetId);
            assetStore.GetAsset<object>(assetInfo2.AssetId);
            assetStore.GetAsset<object>(assetInfo3.AssetId);

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
            return new AssetInfo(AssetId.CreateUnique(), new AssetType("AssetType.Object"), AssetFileUtils.AppendExtension("asset-file"));
        }

        private static IAssetLoader CreateObjectAssetLoader()
        {
            return CreateObjectAssetLoader<object>(CreateNewAssetInfo().AssetType);
        }

        private static IAssetLoader CreateObjectAssetLoader<T>(AssetType assetType) where T : new()
        {
            var assetLoader = Substitute.For<IAssetLoader>();

            assetLoader.AssetType.Returns(assetType);
            assetLoader.AssetClassType.Returns(typeof(T));
            assetLoader.LoadAsset(Arg.Any<AssetInfo>(), Arg.Any<IAssetStore>()).Returns(new T());

            return assetLoader;
        }

        private static IFile CreateAssetFile(AssetInfo assetInfo)
        {
            var file = Substitute.For<IFile>();
            file.Name.Returns(Utils.Random.GetString());
            file.Extension.Returns(AssetFileUtils.Extension);
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

        #endregion
    }
}