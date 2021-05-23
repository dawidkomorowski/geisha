namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Base class of all components to be supported by engine.
    /// </summary>
    public abstract class Component
    {
        protected Component()
        {
            ComponentId = ComponentId.Of(GetType());
        }

        public ComponentId ComponentId { get; }
    }
}