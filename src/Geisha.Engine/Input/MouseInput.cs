using Geisha.Common.Math;

namespace Geisha.Engine.Input
{
    // TODO Add XML docs.
    public struct MouseInput
    {
        public MouseInput(Vector2 position, bool leftButton, bool middleButton, bool rightButton, bool extendedButton1, bool extendedButton2)
        {
            Position = position;
            LeftButton = leftButton;
            MiddleButton = middleButton;
            RightButton = rightButton;
            ExtendedButton1 = extendedButton1;
            ExtendedButton2 = extendedButton2;
        }

        public Vector2 Position { get; }
        public bool LeftButton { get; }
        public bool MiddleButton { get; }
        public bool RightButton { get; }
        public bool ExtendedButton1 { get; }
        public bool ExtendedButton2 { get; }
    }
}