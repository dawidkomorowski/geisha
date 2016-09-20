namespace Geisha.Framework.Input
{
    public struct HardwareInput
    {
        public HardwareInput(KeyInput keyInput)
        {
            KeyInput = keyInput;
        }

        public KeyInput KeyInput { get; }
    }
}