﻿using System;
using System.IO;
using System.Linq;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.CreateSpriteAsset.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.IntegrationTestsData;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateSpriteAsset.Model
{
    [TestFixture]
    public class CreateSpriteAssetServiceIntegrationTests
    {
        private TemporaryDirectory _temporaryDirectory = null!;

        [SetUp]
        public void SetUp()
        {
            _temporaryDirectory = new TemporaryDirectory();
        }

        [TearDown]
        public void TearDown()
        {
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void CreateSpriteAsset_ShouldCreateSpriteAssetFileInTheSameFolderAsTextureAssetFile_GivenTextureAssetFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;

            var project = Project.Create(projectName, projectLocation);
            var projectFilePath = project.ProjectFilePath;

            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInProject = Path.Combine(project.FolderPath, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInProject);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\TestTexture"));
            var textureAssetFilePathInProject = Path.Combine(project.FolderPath, AssetFileUtils.AppendExtension("TestTexture"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInProject);

            project = Project.Open(projectFilePath);
            var textureAssetFile = project.Files.Single(f => CreateSpriteAssetUtils.IsTextureAssetFile(f.Path));

            var createTextureAssetService = new CreateTextureAssetService();
            var createSpriteAssetService = new CreateSpriteAssetService(createTextureAssetService);

            // Act
            createSpriteAssetService.CreateSpriteAsset(textureAssetFile);

            // Assert
            var spriteAssetFilePath = Path.Combine(project.FolderPath, AssetFileUtils.AppendExtension("TestTexture.sprite"));
            Assert.That(File.Exists(spriteAssetFilePath), Is.True, "Sprite asset file was not created.");

            var assetData = AssetData.Load(spriteAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));
            var spriteAssetContent = assetData.ReadJsonContent<SpriteAssetContent>();
            Assert.That(spriteAssetContent.TextureAssetId, Is.EqualTo(AssetsIds.TestTexture.Value));
            Assert.That(spriteAssetContent.SourceUV.X, Is.Zero);
            Assert.That(spriteAssetContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteAssetContent.SourceDimension.X, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceDimension.Y, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceAnchor.X, Is.EqualTo(5));
            Assert.That(spriteAssetContent.SourceAnchor.Y, Is.EqualTo(5));
            Assert.That(spriteAssetContent.PixelsPerUnit, Is.EqualTo(1));
        }

        [Test]
        public void CreateSprite_ShouldCreateTextureAssetFileAndSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;

            var project = Project.Create(projectName, projectLocation);
            var projectFilePath = project.ProjectFilePath;

            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInProject = Path.Combine(project.FolderPath, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInProject);

            project = Project.Open(projectFilePath);
            var textureAssetFile = project.Files.Single();

            var createTextureAssetService = new CreateTextureAssetService();
            var createSpriteAssetService = new CreateSpriteAssetService(createTextureAssetService);

            // Act
            createSpriteAssetService.CreateSpriteAsset(textureAssetFile);

            // Assert
            var textureAssetFilePath = Path.Combine(project.FolderPath, AssetFileUtils.AppendExtension("TestTexture"));
            Assert.That(File.Exists(textureAssetFilePath), Is.True, "Texture asset file was not created.");

            var textureAssetData = AssetData.Load(textureAssetFilePath);
            Assert.That(textureAssetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(textureAssetData.AssetType, Is.EqualTo(RenderingAssetTypes.Texture));
            var textureAssetContent = textureAssetData.ReadJsonContent<TextureAssetContent>();
            Assert.That(textureAssetContent.TextureFilePath, Is.EqualTo("TestTexture.png"));

            var spriteAssetFilePath = Path.Combine(project.FolderPath, AssetFileUtils.AppendExtension("TestTexture.sprite"));
            Assert.That(File.Exists(spriteAssetFilePath), Is.True, "Sprite asset file was not created.");

            var spriteAssetData = AssetData.Load(spriteAssetFilePath);
            Assert.That(spriteAssetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(spriteAssetData.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));
            var spriteAssetContent = spriteAssetData.ReadJsonContent<SpriteAssetContent>();
            Assert.That(spriteAssetContent.TextureAssetId, Is.EqualTo(textureAssetData.AssetId.Value));
            Assert.That(spriteAssetContent.SourceUV.X, Is.Zero);
            Assert.That(spriteAssetContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteAssetContent.SourceDimension.X, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceDimension.Y, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceAnchor.X, Is.EqualTo(5));
            Assert.That(spriteAssetContent.SourceAnchor.Y, Is.EqualTo(5));
            Assert.That(spriteAssetContent.PixelsPerUnit, Is.EqualTo(1));
        }
    }
}