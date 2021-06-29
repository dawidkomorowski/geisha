using System;
using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Editor.SceneEditor.Model
{
    internal interface ICreateEmptySceneService
    {
        void CreateEmptyScene(string name, IProject project);
        void CreateEmptyScene(string name, IProjectFolder folder);
    }

    internal sealed class CreateEmptySceneService : ICreateEmptySceneService
    {
        private readonly ISceneFactory _sceneFactory;
        private readonly ISceneSerializer _sceneSerializer;

        public CreateEmptySceneService(ISceneFactory sceneFactory, ISceneSerializer sceneSerializer)
        {
            _sceneFactory = sceneFactory;
            _sceneSerializer = sceneSerializer;
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
            using var memoryStream = new MemoryStream();
            _sceneSerializer.Serialize(_sceneFactory.Create(), memoryStream);
            memoryStream.Position = 0;
            addFile($"{name}{SceneEditorConstants.SceneFileExtension}", memoryStream);
        }
    }
}