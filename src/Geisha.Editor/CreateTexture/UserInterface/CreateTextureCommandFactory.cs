using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateTexture.UserInterface
{
    internal interface ICreateTextureCommandFactory
    {
        CreateTextureCommand Create(IProjectFile sourceTextureFile);
    }

    internal class CreateTextureCommandFactory : ICreateTextureCommandFactory
    {
        private readonly ICreateTextureService _createTextureService;

        public CreateTextureCommandFactory(ICreateTextureService createTextureService)
        {
            _createTextureService = createTextureService;
        }

        public CreateTextureCommand Create(IProjectFile sourceTextureFile)
        {
            return new CreateTextureCommand(_createTextureService, sourceTextureFile);
        }
    }
}