﻿using NUnit.Framework;

namespace Geisha.Framework.Input.Wpf.UnitTests
{
    [TestFixture]
    public class KeyMapperTests
    {
        [TestCase(Key.Backspace, System.Windows.Input.Key.Back)]
        [TestCase(Key.Tab, System.Windows.Input.Key.Tab)]
        [TestCase(Key.Enter, System.Windows.Input.Key.Enter)]
        [TestCase(Key.LeftShift, System.Windows.Input.Key.LeftShift)]
        [TestCase(Key.RightShift, System.Windows.Input.Key.RightShift)]
        [TestCase(Key.LeftCtrl, System.Windows.Input.Key.LeftCtrl)]
        [TestCase(Key.RightCtrl, System.Windows.Input.Key.RightCtrl)]
        [TestCase(Key.LeftAlt, System.Windows.Input.Key.LeftAlt)]
        [TestCase(Key.RightAlt, System.Windows.Input.Key.RightAlt)]
        [TestCase(Key.Pause, System.Windows.Input.Key.Pause)]
        [TestCase(Key.CapsLock, System.Windows.Input.Key.CapsLock)]
        [TestCase(Key.Escape, System.Windows.Input.Key.Escape)]
        [TestCase(Key.Space, System.Windows.Input.Key.Space)]
        [TestCase(Key.PageUp, System.Windows.Input.Key.PageUp)]
        [TestCase(Key.PageDown, System.Windows.Input.Key.PageDown)]
        [TestCase(Key.End, System.Windows.Input.Key.End)]
        [TestCase(Key.Home, System.Windows.Input.Key.Home)]
        [TestCase(Key.Left, System.Windows.Input.Key.Left)]
        [TestCase(Key.Up, System.Windows.Input.Key.Up)]
        [TestCase(Key.Right, System.Windows.Input.Key.Right)]
        [TestCase(Key.Down, System.Windows.Input.Key.Down)]
        [TestCase(Key.Insert, System.Windows.Input.Key.Insert)]
        [TestCase(Key.Delete, System.Windows.Input.Key.Delete)]
        [TestCase(Key.D0, System.Windows.Input.Key.D0)]
        [TestCase(Key.D1, System.Windows.Input.Key.D1)]
        [TestCase(Key.D2, System.Windows.Input.Key.D2)]
        [TestCase(Key.D3, System.Windows.Input.Key.D3)]
        [TestCase(Key.D4, System.Windows.Input.Key.D4)]
        [TestCase(Key.D5, System.Windows.Input.Key.D5)]
        [TestCase(Key.D6, System.Windows.Input.Key.D6)]
        [TestCase(Key.D7, System.Windows.Input.Key.D7)]
        [TestCase(Key.D8, System.Windows.Input.Key.D8)]
        [TestCase(Key.D9, System.Windows.Input.Key.D9)]
        [TestCase(Key.A, System.Windows.Input.Key.A)]
        [TestCase(Key.B, System.Windows.Input.Key.B)]
        [TestCase(Key.C, System.Windows.Input.Key.C)]
        [TestCase(Key.D, System.Windows.Input.Key.D)]
        [TestCase(Key.E, System.Windows.Input.Key.E)]
        [TestCase(Key.F, System.Windows.Input.Key.F)]
        [TestCase(Key.G, System.Windows.Input.Key.G)]
        [TestCase(Key.H, System.Windows.Input.Key.H)]
        [TestCase(Key.I, System.Windows.Input.Key.I)]
        [TestCase(Key.J, System.Windows.Input.Key.J)]
        [TestCase(Key.K, System.Windows.Input.Key.K)]
        [TestCase(Key.L, System.Windows.Input.Key.L)]
        [TestCase(Key.M, System.Windows.Input.Key.M)]
        [TestCase(Key.N, System.Windows.Input.Key.N)]
        [TestCase(Key.O, System.Windows.Input.Key.O)]
        [TestCase(Key.P, System.Windows.Input.Key.P)]
        [TestCase(Key.Q, System.Windows.Input.Key.Q)]
        [TestCase(Key.R, System.Windows.Input.Key.R)]
        [TestCase(Key.S, System.Windows.Input.Key.S)]
        [TestCase(Key.T, System.Windows.Input.Key.T)]
        [TestCase(Key.U, System.Windows.Input.Key.U)]
        [TestCase(Key.V, System.Windows.Input.Key.V)]
        [TestCase(Key.W, System.Windows.Input.Key.W)]
        [TestCase(Key.X, System.Windows.Input.Key.X)]
        [TestCase(Key.Y, System.Windows.Input.Key.Y)]
        [TestCase(Key.Z, System.Windows.Input.Key.Z)]
        [TestCase(Key.NumPad0, System.Windows.Input.Key.NumPad0)]
        [TestCase(Key.NumPad1, System.Windows.Input.Key.NumPad1)]
        [TestCase(Key.NumPad2, System.Windows.Input.Key.NumPad2)]
        [TestCase(Key.NumPad3, System.Windows.Input.Key.NumPad3)]
        [TestCase(Key.NumPad4, System.Windows.Input.Key.NumPad4)]
        [TestCase(Key.NumPad5, System.Windows.Input.Key.NumPad5)]
        [TestCase(Key.NumPad6, System.Windows.Input.Key.NumPad6)]
        [TestCase(Key.NumPad7, System.Windows.Input.Key.NumPad7)]
        [TestCase(Key.NumPad8, System.Windows.Input.Key.NumPad8)]
        [TestCase(Key.NumPad9, System.Windows.Input.Key.NumPad9)]
        [TestCase(Key.Multiply, System.Windows.Input.Key.Multiply)]
        [TestCase(Key.Add, System.Windows.Input.Key.Add)]
        [TestCase(Key.Subtract, System.Windows.Input.Key.Subtract)]
        [TestCase(Key.Decimal, System.Windows.Input.Key.Decimal)]
        [TestCase(Key.Divide, System.Windows.Input.Key.Divide)]
        [TestCase(Key.F1, System.Windows.Input.Key.F1)]
        [TestCase(Key.F2, System.Windows.Input.Key.F2)]
        [TestCase(Key.F3, System.Windows.Input.Key.F3)]
        [TestCase(Key.F4, System.Windows.Input.Key.F4)]
        [TestCase(Key.F5, System.Windows.Input.Key.F5)]
        [TestCase(Key.F6, System.Windows.Input.Key.F6)]
        [TestCase(Key.F7, System.Windows.Input.Key.F7)]
        [TestCase(Key.F8, System.Windows.Input.Key.F8)]
        [TestCase(Key.F9, System.Windows.Input.Key.F9)]
        [TestCase(Key.F10, System.Windows.Input.Key.F10)]
        [TestCase(Key.F11, System.Windows.Input.Key.F11)]
        [TestCase(Key.F12, System.Windows.Input.Key.F12)]
        [TestCase(Key.NumLock, System.Windows.Input.Key.NumLock)]
        [TestCase(Key.ScrollLock, System.Windows.Input.Key.Scroll)]
        [TestCase(Key.Semicolon, System.Windows.Input.Key.OemSemicolon)]
        [TestCase(Key.EqualsSign, System.Windows.Input.Key.OemPlus)]
        [TestCase(Key.Comma, System.Windows.Input.Key.OemComma)]
        [TestCase(Key.Dash, System.Windows.Input.Key.OemMinus)]
        [TestCase(Key.Period, System.Windows.Input.Key.OemPeriod)]
        [TestCase(Key.Slash, System.Windows.Input.Key.OemQuestion)]
        [TestCase(Key.Tilde, System.Windows.Input.Key.OemTilde)]
        [TestCase(Key.OpenBrackets, System.Windows.Input.Key.OemOpenBrackets)]
        [TestCase(Key.Backslash, System.Windows.Input.Key.OemPipe)]
        [TestCase(Key.CloseBrackets, System.Windows.Input.Key.OemCloseBrackets)]
        [TestCase(Key.Quotes, System.Windows.Input.Key.OemQuotes)]
        public void Map(Key key, System.Windows.Input.Key expected)
        {
            // Arrange
            var keyMapper = new KeyMapper();

            // Act
            var actual = keyMapper.Map(key);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}