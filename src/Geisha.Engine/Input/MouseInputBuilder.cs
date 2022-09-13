using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Provides simple way of setting initial state of <see cref="MouseInput" />. Represents temporary state of
    ///     mouse and allows to create actual <see cref="MouseInput" /> in that state.
    /// </summary>
    /// <remarks>
    ///     Value <c>true</c> of button state means the button is in pressed state. Value <c>false</c> of button state means
    ///     the button is in released state.
    /// </remarks>
    public sealed class MouseInputBuilder
    {
        /// <summary>
        ///     Position of the mouse relative to left upper corner of the window with X axis towards right and Y axis towards
        ///     bottom of the window.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     Delta of the mouse position since last input capture. It is mouse movement vector relative to last captured
        ///     position of the mouse. <see cref="PositionDelta" /> is defined in the same coordinates as <see cref="Position" />.
        /// </summary>
        public Vector2 PositionDelta { get; set; }

        /// <summary>
        ///     State of the left mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftButton { get; set; }

        /// <summary>
        ///     State of the middle mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool MiddleButton { get; set; }

        /// <summary>
        ///     State of the right mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightButton { get; set; }

        /// <summary>
        ///     State of the first extended mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool XButton1 { get; set; }

        /// <summary>
        ///     State of the second extended mouse button. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool XButton2 { get; set; }

        /// <summary>
        ///     Value indicating rotation of mouse wheel since last mouse input capture (typically last fixed update). Sign of the
        ///     value defines direction of rotation.
        /// </summary>
        /// <remarks>
        ///     This value is not normalized so the game should check for a positive or negative value rather than an aggregate
        ///     total.
        /// </remarks>
        public int ScrollDelta { get; set; }

        /// <summary>
        ///     Creates new <see cref="MouseInput" /> with the state defined by the current state of the builder.
        /// </summary>
        /// <returns><see cref="MouseInput" /> with the state defined by the current state of the builder.</returns>
        public MouseInput Build()
        {
            return new MouseInput(this);
        }
    }
}