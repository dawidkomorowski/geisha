namespace Geisha.Engine.Physics
{
    /// <summary>
    ///     Configuration of engine physics subsystem.
    /// </summary>
    public sealed class PhysicsConfiguration
    {
        private PhysicsConfiguration(bool renderCollisionGeometry)
        {
            RenderCollisionGeometry = renderCollisionGeometry;
        }

        /// <summary>
        ///     If true, collision geometry is rendered on top of regular graphics to help with debugging.
        /// </summary>
        public bool RenderCollisionGeometry { get; }

        public static IBuilder CreateBuilder() => new Builder();

        public interface IBuilder
        {
            IBuilder WithRenderCollisionGeometry(bool renderCollisionGeometry);
            PhysicsConfiguration Build();
        }

        private sealed class Builder : IBuilder
        {
            private bool _renderCollisionGeometry;

            public IBuilder WithRenderCollisionGeometry(bool renderCollisionGeometry)
            {
                _renderCollisionGeometry = renderCollisionGeometry;
                return this;
            }

            public PhysicsConfiguration Build() => new(_renderCollisionGeometry);
        }
    }
}