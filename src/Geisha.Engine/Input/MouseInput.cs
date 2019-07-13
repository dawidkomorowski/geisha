using Geisha.Common.Math;

namespace Geisha.Engine.Input
{
    // TODO Add XML docs.
    public struct MouseInput
    {
        public MouseInput(MouseInputBuilder mouseInputBuilder)
        {
            Position = mouseInputBuilder.Position;
            LeftButton = mouseInputBuilder.LeftButton;
            MiddleButton = mouseInputBuilder.MiddleButton;
            RightButton = mouseInputBuilder.RightButton;
            XButton1 = mouseInputBuilder.XButton1;
            XButton2 = mouseInputBuilder.XButton2;
            ScrollDelta = mouseInputBuilder.ScrollDelta;
        }

        public Vector2 Position { get; }
        public bool LeftButton { get; }
        public bool MiddleButton { get; }
        public bool RightButton { get; }
        public bool XButton1 { get; }
        public bool XButton2 { get; }
        public int ScrollDelta { get; }
    }
}