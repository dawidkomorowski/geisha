using System.IO;
using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Editor.CreateSound.Model
{
    internal interface ICreateSoundService
    {
        void CreateSound(IProjectFile sourceSoundFile);
    }

    internal sealed class CreateSoundService : ICreateSoundService
    {
        private readonly IJsonSerializer _jsonSerializer;

        public CreateSoundService(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public void CreateSound(IProjectFile sourceSoundFile)
        {
            var soundFileName = $"{Path.GetFileNameWithoutExtension(sourceSoundFile.Name)}{AudioFileExtensions.Sound}";
            var folder = sourceSoundFile.ParentFolder;

            var soundFileContent = new SoundFileContent
            {
                AssetId = AssetId.CreateUnique().Value,
                SoundFilePath = sourceSoundFile.Name
            };

            var json = _jsonSerializer.Serialize(soundFileContent);

            using (var stream = json.ToStream())
            {
                folder.AddFile(soundFileName, stream);
            }
        }
    }
}