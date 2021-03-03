using System;
using System.Linq;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Geisha.Editor.UnitTests.SceneEditor
{
    public static class TestSceneModelFactory
    {
        public static SceneModel Create(Scene scene)
        {
            var sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            sceneBehaviorFactoryProvider.Get(Arg.Any<string>()).ThrowsForAnyArgs(new InvalidOperationException("Missing substitute configuration."));

            var sceneModel = new SceneModel(scene, Enumerable.Empty<SceneBehaviorName>(), sceneBehaviorFactoryProvider);

            return sceneModel;
        }
    }
}