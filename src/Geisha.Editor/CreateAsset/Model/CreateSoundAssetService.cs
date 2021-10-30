using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateSoundAssetService
    {
        void CreateSoundAsset(IProjectFile sourceSoundFile);
    }

    internal sealed class CreateSoundAssetService : ICreateSoundAssetService
    {
        public void CreateSoundAsset(IProjectFile sourceSoundFile)
        {
            // TODO Use AssetTool here.
            var soundAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(sourceSoundFile.Name));
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