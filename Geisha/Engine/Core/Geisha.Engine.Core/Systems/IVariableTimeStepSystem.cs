using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface IVariableTimeStepSystem
    {
        string Name { get; }
        void Update(Scene scene, GameTime gameTime);
    }
}