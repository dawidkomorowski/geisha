namespace Geisha.Engine.Core
{
    public interface IUpdatable
    {
        void Update(double deltaTime);
        void FixedUpdate();
    }
}