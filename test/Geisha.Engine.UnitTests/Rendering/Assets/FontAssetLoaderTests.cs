using System.IO;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Assets;

[TestFixture]
public class FontAssetLoaderTests
{
    [Test]
    public void LoadAsset_ShouldLoadFontFileInfoFromFile()
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var assetStore = Substitute.For<IAssetStore>();

        const string assetFilePath = @"some_directory\font_asset_path";
        const string fontFilePath = @"some_directory\font_file_path";

        var fontAssetContent = new FontAssetContent
        {
            FontFilePath = "font_file_path"
        };

        var assetInfo = new AssetInfo(AssetId.CreateUnique(), RenderingAssetTypes.Font, assetFilePath);
        var assetData = AssetData.CreateWithJsonContent(assetInfo.AssetId, assetInfo.AssetType, fontAssetContent);
        var memoryStream = new MemoryStream();
        assetData.Save(memoryStream);
        memoryStream.Position = 0;

        var assetFile = Substitute.For<IFile>();
        assetFile.OpenRead().Returns(memoryStream);
        fileSystem.GetFile(assetFilePath).Returns(assetFile);

        var fontAssetLoader = new FontAssetLoader(fileSystem);

        // Act
        var actual = (FontFileInfo)fontAssetLoader.LoadAsset(assetInfo, assetStore);

        // Assert
        Assert.That(actual.Path, Is.EqualTo(fontFilePath));
    }
}