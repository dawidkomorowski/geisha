using System;
using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    internal interface ICreateEmptySceneService
    {
        void CreateEmptyScene(string name, IProject project);
        void CreateEmptyScene(string name, IProjectFolder folder);
    }

    internal sealed class CreateEmptySceneService : ICreateEmptySceneService
    {
        private readonly ISceneLoader _sceneLoader;

        public CreateEmptySceneService(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void CreateEmptyScene(string name, IProject project)
        {
            CreateEmptyScene(name, (fileName, stream) => project.AddFile(fileName, stream));
        }

        public void CreateEmptyScene(string name, IProjectFolder folder)
        {
            CreateEmptyScene(name, (fileName, stream) => folder.AddFile(fileName, stream));
        }

        private void CreateEmptyScene(string name, Action<string, Stream> addFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                _sceneLoader.Save(new Scene(), memoryStream);
                memoryStream.Position = 0;
                addFile($"{name}{SceneEditorConstants.SceneFileExtension}", memoryStream);
            }
        }
    }
}