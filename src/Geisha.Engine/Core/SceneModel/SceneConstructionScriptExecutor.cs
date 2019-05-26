using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    internal interface ISceneConstructionScriptExecutor
    {
        void Execute(Scene scene);
    }

    internal class SceneConstructionScriptExecutor : ISceneConstructionScriptExecutor
    {
        private readonly IEnumerable<ISceneConstructionScript> _sceneConstructionScripts;

        public SceneConstructionScriptExecutor(IEnumerable<ISceneConstructionScript> sceneConstructionScripts)
        {
            _sceneConstructionScripts = sceneConstructionScripts;
        }

        public void Execute(Scene scene)
        {
            if (scene.ConstructionScript == null) return;

            var matchingConstructionScripts = _sceneConstructionScripts.Where(s => s.Name == scene.ConstructionScript).ToList();
            if (matchingConstructionScripts.Count == 1)
            {
                var script = matchingConstructionScripts.Single();
                script.Execute(scene);
            }
            else
            {
                //TODO Custom exception with additional information about discovered construction scripts?
                throw new InvalidOperationException(
                    $"There must be exactly one {nameof(ISceneConstructionScript)} implementation registered with name: {scene.ConstructionScript}");
            }
        }
    }
}