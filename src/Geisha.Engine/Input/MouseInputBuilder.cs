using Geisha.Common.Math;

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
        ///     This value is not normalized and the scale can differ depending on user settings and across different devices.
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