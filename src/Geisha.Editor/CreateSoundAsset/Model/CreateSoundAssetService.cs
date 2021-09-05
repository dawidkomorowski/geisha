using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Editor.CreateSoundAsset.Model
{
    internal interface ICreateSoundAssetService
    {
        void CreateSoundAsset(IProjectFile sourceSoundFile);
    }

    internal sealed class CreateSoundAssetService : ICreateSoundAssetService
    {
        public void CreateSoundAsset(IProjectFile sourceSoundFile)
        {
            var soundAssetFileName = $"{Path.GetFileNameWithoutExtension(sourceSoundFile.Name)}{AssetFileExtension.Value}";
            var folder = sourceSoundFile.ParentFolder;

            var soundAssetContent = new SoundAssetContent
            {
                SoundFilePath = sourceSoundFile.Name
            };

            using var memoryStream = new MemoryStream();
            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), AudioAssetTypes.Sound, soundAssetContent);
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            folder.AddFile(soundAssetFileName, memoryStream);
        }
    }
}