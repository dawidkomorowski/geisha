using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Represents state of the mouse.
    /// </summary>
    /// <remarks>
    ///     Value <c>true</c> of button state means the button is in pressed state. Value <c>false</c> of button state means
    ///     the button is in released state.
    /// </remarks>
    public readonly struct MouseInput
    {
        /// <summary>
        ///     Creates new instance of <see cref="MouseInput" /> struct initialized as defined by
        ///     <see cref="MouseInputBuilder" /> instance.
        /// </summary>
        /// <param name="mouseInputBuilder">
        ///     <see cref="MouseInputBuilder" /> instance that defines position, scroll delta and state of all mouse buttons.
        /// </param>
        public MouseInput(MouseInputBuilder mouseInputBuilder)
        {
            Position = mouseInputBuilder.Position;
            PositionDelta = mouseInputBuilder.PositionDelta;
            LeftButton = mouseInputBuilder.LeftButton;
            MiddleButton = mouseInputBuilder.MiddleButton;
            RightButton = mouseInputBuilder.RightButton;
            XButton1 = mouseInputBuilder.XButton1;
            XButton2 = mouseInputBuilder.XButton2;
            ScrollDelta = mouseInputBuilder.ScrollDelta;
        }

        /// <summary>
        ///     Position of the mouse relative to left upper corner of the window with X axis towards right and Y axis towards
        ///     bottom of the window.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        ///     Delta of the mouse position since last input capture. It is mouse movement vector relative to last captured
        ///     position of the mouse. <see cref="PositionDelta" /> is defined in the same coordinates as <see cref="Position" />.
        /// </summary>
        public Vector2 PositionDelta { get; }

        /// <summary>
        ///     State of the left mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftButton { get; }

        /// <summary>
        ///     State of the middle mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool MiddleButton { get; }

        /// <summary>
        ///     State of the right mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightButton { get; }

        /// <summary>
        ///     State of the first extended mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool XButton1 { get; }

        /// <summary>
        ///     State of the second extended mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool XButton2 { get; }

        /// <summary>
        ///     Value indicating rotation of mouse wheel since last mouse input capture (typically last fixed update). Sign of the
        ///     value defines direction of rotation.
        /// </summary>
        /// <remarks>
        ///     This value is not normalized so the game should check for a positive or negative value rather than an aggregate
        ///     total.
        /// </remarks>
        public int ScrollDelta { get; }
    }
}