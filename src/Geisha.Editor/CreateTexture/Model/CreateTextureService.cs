using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateTexture.Model
{
    internal interface ICreateTextureService
    {
        void CreateTexture();
    }

    internal sealed class CreateTextureService : ICreateTextureService
    {
        private readonly IProjectFile _sourceTextureFile;

        public CreateTextureService(IProjectFile sourceTextureFile)
        {
            _sourceTextureFile = sourceTextureFile;
        }

        public void CreateTexture()
        {
            throw new System.NotImplementedException();
        }
    }
}