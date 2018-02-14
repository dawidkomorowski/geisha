using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Mapping
{
    public class HardwareInputVariant
    {
        private Key _key;

        public enum Variant
        {
            Keyboard
        }

        public Variant CurrentVariant { get; private set; }

        public Key Key
        {
            get => _key;
            set
            {
                _key = value;
                CurrentVariant = Variant.Keyboard;
            }
        }
    }
}