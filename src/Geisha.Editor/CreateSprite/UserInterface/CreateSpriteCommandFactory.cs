using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateSprite.UserInterface
{
    internal interface ICreateSpriteCommandFactory
    {
        CreateSpriteCommand Create(IProjectFile textureFile);
    }

    internal sealed class CreateSpriteCommandFactory : ICreateSpriteCommandFactory
    {
        private readonly ICreateSpriteService _createSpriteService;

        public CreateSpriteCommandFactory(ICreateSpriteService createSpriteService)
        {
            _createSpriteService = createSpriteService;
        }

        public CreateSpriteCommand Create(IProjectFile textureFile)
        {
            return new CreateSpriteCommand(_createSpriteService, textureFile);
        }
    }
}