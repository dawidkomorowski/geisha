using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Benchmark
{
    internal sealed class BenchmarkSystem : ICustomSystem
    {
        public string Name => nameof(BenchmarkSystem);

        public void ProcessFixedUpdate(Scene scene)
        {
        }

        public void ProcessUpdate(Scene scene, GameTime gameTime)
        {
        }
    }
}