using System.IO;
using System.Text.Json;
using Geisha.Common;
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
            var soundFileName = $"{Path.GetFileNameWithoutExtension(sourceSoundFile.Name)}{AudioFileExtensions.Sound}";
            var folder = sourceSoundFile.ParentFolder;

            var soundFileContent = new SoundFileContent
            {
                AssetId = AssetId.CreateUnique().Value,
                SoundFilePath = sourceSoundFile.Name
            };

            var json = JsonSerializer.Serialize(soundFileContent);

            using var stream = json.ToStream();
            folder.AddFile(soundFileName, stream);
        }
    }
}