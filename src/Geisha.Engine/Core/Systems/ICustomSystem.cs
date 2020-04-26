using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface ICustomSystem
    {
        string Name { get; }
        void ProcessFixedUpdate(Scene scene);
        void ProcessUpdate(Scene scene, GameTime gameTime);
    }
}