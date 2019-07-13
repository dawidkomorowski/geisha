using Geisha.Common.Math;

namespace Geisha.Engine.Input
{
    // TODO Add XML docs.
    public sealed class MouseInputBuilder
    {
        public Vector2 Position { get; set; }
        public bool LeftButton { get; set; }
        public bool MiddleButton { get; set; }
        public bool RightButton { get; set; }
        public bool XButton1 { get; set; }
        public bool XButton2 { get; set; }
        public int ScrollDelta { get; set; }

        public MouseInput Build()
        {
            return new MouseInput(this);
        }
    }
}