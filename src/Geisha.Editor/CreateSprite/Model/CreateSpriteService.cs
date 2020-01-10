using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateSprite.Model
{
    internal interface ICreateSpriteService
    {
        void CreateSprite(IProjectFile textureFile);
    }

    internal sealed class CreateSpriteService : ICreateSpriteService
    {
        public void CreateSprite(IProjectFile textureFile)
        {
            throw new System.NotImplementedException();
        }
    }
}