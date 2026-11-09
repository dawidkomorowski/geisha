using System;

namespace Geisha.Engine.Input.Backend;

// TODO: Add documentation.
// TODO: Should it be named Controller Buttons or Gamepad buttons?
[Flags]
public enum ControllerButtons
{
    None = 0,
    DPadUp = 1,
    DPadDown = 2,
    DPadLeft = 4,
    DPadRight = 8,
    Start = 0x10,
    Back = 0x20,
    LeftThumb = 0x40,
    RightThumb = 0x80,
    LeftShoulder = 0x100,
    RightShoulder = 0x200,
    A = 0x1000,
    B = 0x2000,
    X = 0x4000,
    Y = 0x8000
}