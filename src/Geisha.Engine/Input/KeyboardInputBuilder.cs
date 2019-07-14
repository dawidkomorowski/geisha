namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Provides simple way of setting initial state of <see cref="KeyboardInput" />. Represents temporary state of
    ///     keyboard and allows to create actual <see cref="KeyboardInput" /> in that state.
    /// </summary>
    /// <remarks>
    ///     Value <c>true</c> of key state means the key is in pressed state. Value <c>false</c> of key state means the
    ///     key is in released state.
    /// </remarks>
    public sealed class KeyboardInputBuilder
    {
        /// <summary>
        ///     State of the Backspace key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Backspace { get; set; }

        /// <summary>
        ///     State of the Tab key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Tab { get; set; }

        /// <summary>
        ///     State of the Enter key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Enter { get; set; }

        /// <summary>
        ///     State of the left Shift key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftShift { get; set; }

        /// <summary>
        ///     State of the right Shift key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightShift { get; set; }

        /// <summary>
        ///     State of the left CTRL key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftCtrl { get; set; }

        /// <summary>
        ///     State of the right CTRL key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightCtrl { get; set; }

        /// <summary>
        ///     State of the left ALT key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool LeftAlt { get; set; }

        /// <summary>
        ///     State of the right ALT key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool RightAlt { get; set; }

        /// <summary>
        ///     State of the Pause key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Pause { get; set; }

        /// <summary>
        ///     State of the Caps Lock key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool CapsLock { get; set; }

        /// <summary>
        ///     State of the ESC key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Escape { get; set; }

        /// <summary>
        ///     State of the Spacebar key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Space { get; set; }

        /// <summary>
        ///     State of the Page Up key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool PageUp { get; set; }

        /// <summary>
        ///     State of the Page Down key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool PageDown { get; set; }

        /// <summary>
        ///     State of the End key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool End { get; set; }

        /// <summary>
        ///     State of the Home key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Home { get; set; }

        /// <summary>
        ///     State of the Left Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Left { get; set; }

        /// <summary>
        ///     State of the Up Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Up { get; set; }

        /// <summary>
        ///     State of the Right Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Right { get; set; }

        /// <summary>
        ///     State of the Down Arrow key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Down { get; set; }

        /// <summary>
        ///     State of the Insert key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Insert { get; set; }

        /// <summary>
        ///     State of the Delete key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        ///     State of the 0 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D0 { get; set; }

        /// <summary>
        ///     State of the 1 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D1 { get; set; }

        /// <summary>
        ///     State of the 2 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D2 { get; set; }

        /// <summary>
        ///     State of the 3 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D3 { get; set; }

        /// <summary>
        ///     State of the 4 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D4 { get; set; }

        /// <summary>
        ///     State of the 5 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D5 { get; set; }

        /// <summary>
        ///     State of the 6 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D6 { get; set; }

        /// <summary>
        ///     State of the 7 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D7 { get; set; }

        /// <summary>
        ///     State of the 8 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D8 { get; set; }

        /// <summary>
        ///     State of the 9 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D9 { get; set; }

        /// <summary>
        ///     State of the A key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool A { get; set; }

        /// <summary>
        ///     State of the B key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool B { get; set; }

        /// <summary>
        ///     State of the C key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool C { get; set; }

        /// <summary>
        ///     State of the D key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool D { get; set; }

        /// <summary>
        ///     State of the E key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool E { get; set; }

        /// <summary>
        ///     State of the F key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F { get; set; }

        /// <summary>
        ///     State of the G key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool G { get; set; }

        /// <summary>
        ///     State of the H key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool H { get; set; }

        /// <summary>
        ///     State of the I key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool I { get; set; }

        /// <summary>
        ///     State of the J key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool J { get; set; }

        /// <summary>
        ///     State of the K key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool K { get; set; }

        /// <summary>
        ///     State of the L key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool L { get; set; }

        /// <summary>
        ///     State of the M key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool M { get; set; }

        /// <summary>
        ///     State of the N key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool N { get; set; }

        /// <summary>
        ///     State of the O key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool O { get; set; }

        /// <summary>
        ///     State of the P key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool P { get; set; }

        /// <summary>
        ///     State of the Q key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Q { get; set; }

        /// <summary>
        ///     State of the R key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool R { get; set; }

        /// <summary>
        ///     State of the S key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool S { get; set; }

        /// <summary>
        ///     State of the T key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool T { get; set; }

        /// <summary>
        ///     State of the U key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool U { get; set; }

        /// <summary>
        ///     State of the V key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool V { get; set; }

        /// <summary>
        ///     State of the W key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool W { get; set; }

        /// <summary>
        ///     State of the X key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool X { get; set; }

        /// <summary>
        ///     State of the Y key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Y { get; set; }

        /// <summary>
        ///     State of the Z key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Z { get; set; }

        /// <summary>
        ///     State of the 0 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad0 { get; set; }

        /// <summary>
        ///     State of the 1 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad1 { get; set; }

        /// <summary>
        ///     State of the 2 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad2 { get; set; }

        /// <summary>
        ///     State of the 3 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad3 { get; set; }

        /// <summary>
        ///     State of the 4 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad4 { get; set; }

        /// <summary>
        ///     State of the 5 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad5 { get; set; }

        /// <summary>
        ///     State of the 6 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad6 { get; set; }

        /// <summary>
        ///     State of the 7 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad7 { get; set; }

        /// <summary>
        ///     State of the 8 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad8 { get; set; }

        /// <summary>
        ///     State of the 9 key on the numeric keypad. <c>true</c> indicates pressed state, while <c>false</c> indicates
        ///     released state.
        /// </summary>
        public bool NumPad9 { get; set; }

        /// <summary>
        ///     State of the Multiply key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Multiply { get; set; }

        /// <summary>
        ///     State of the Add key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Add { get; set; }

        /// <summary>
        ///     State of the Subtract key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Subtract { get; set; }

        /// <summary>
        ///     State of the Decimal key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Decimal { get; set; }

        /// <summary>
        ///     State of the Divide key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Divide { get; set; }

        /// <summary>
        ///     State of the F1 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F1 { get; set; }

        /// <summary>
        ///     State of the F2 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F2 { get; set; }

        /// <summary>
        ///     State of the F3 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F3 { get; set; }

        /// <summary>
        ///     State of the F4 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F4 { get; set; }

        /// <summary>
        ///     State of the F5 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F5 { get; set; }

        /// <summary>
        ///     State of the F6 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F6 { get; set; }

        /// <summary>
        ///     State of the F7 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F7 { get; set; }

        /// <summary>
        ///     State of the F8 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F8 { get; set; }

        /// <summary>
        ///     State of the F9 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F9 { get; set; }

        /// <summary>
        ///     State of the F10 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F10 { get; set; }

        /// <summary>
        ///     State of the F11 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F11 { get; set; }

        /// <summary>
        ///     State of the F12 key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool F12 { get; set; }

        /// <summary>
        ///     State of the Num Lock key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool NumLock { get; set; }

        /// <summary>
        ///     State of the Scroll Lock key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool ScrollLock { get; set; }

        /// <summary>
        ///     State of the Semicolon key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Semicolon { get; set; }

        /// <summary>
        ///     State of the Equals Sign key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool EqualsSign { get; set; }

        /// <summary>
        ///     State of the Comma key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Comma { get; set; }

        /// <summary>
        ///     State of the Dash key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Dash { get; set; }

        /// <summary>
        ///     State of the Period key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Period { get; set; }

        /// <summary>
        ///     State of the Slash key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Slash { get; set; }

        /// <summary>
        ///     State of the Tilde key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Tilde { get; set; }

        /// <summary>
        ///     State of the Open Brackets key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool OpenBrackets { get; set; }

        /// <summary>
        ///     State of the Backslash key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Backslash { get; set; }

        /// <summary>
        ///     State of the Close Brackets key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool CloseBrackets { get; set; }

        /// <summary>
        ///     State of the Quotes key. <c>true</c> indicates pressed state, while <c>false</c> indicates released state.
        /// </summary>
        public bool Quotes { get; set; }

        /// <summary>
        ///     Creates new <see cref="KeyboardInput" /> with the state of keys defined by the current state of the builder.
        /// </summary>
        /// <returns><see cref="KeyboardInput" /> with the state of keys defined by the current state of the builder.</returns>
        public KeyboardInput Build()
        {
            return new KeyboardInput(this);
        }
    }
}