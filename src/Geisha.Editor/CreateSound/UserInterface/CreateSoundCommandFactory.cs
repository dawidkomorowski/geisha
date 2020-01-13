using Geisha.Editor.CreateSound.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateSound.UserInterface
{
    internal interface ICreateSoundCommandFactory
    {
        CreateSoundCommand Create(IProjectFile sourceSoundFile);
    }

    internal sealed class CreateSoundCommandFactory : ICreateSoundCommandFactory
    {
        private readonly ICreateSoundService _createSoundService;

        public CreateSoundCommandFactory(ICreateSoundService createSoundService)
        {
            _createSoundService = createSoundService;
        }

        public CreateSoundCommand Create(IProjectFile sourceSoundFile)
        {
            return new CreateSoundCommand(_createSoundService, sourceSoundFile);
        }
    }
}