using System.Globalization;

namespace Geisha.Editor.Core.Controls
{
    public class IntInput : NumberInput<int>
    {
        protected override string Convert(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override bool TryConvert(string text, out int value)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
    }
}