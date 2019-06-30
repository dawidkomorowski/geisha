using Geisha.Common.Math;

namespace Geisha.Engine.Input
{
    // TODO Add XML docs.
    public struct MouseInput
    {
        public MouseInput(Vector2 position, bool leftButton, bool middleButton, bool rightButton, bool xButton1, bool xButton2)
        {
            Position = position;
            LeftButton = leftButton;
            MiddleButton = middleButton;
            RightButton = rightButton;
            XButton1 = xButton1;
            XButton2 = xButton2;
        }

        public Vector2 Position { get; }
        public bool LeftButton { get; }
        public bool MiddleButton { get; }
        public bool RightButton { get; }
        public bool XButton1 { get; }
        public bool XButton2 { get; }
    }
}