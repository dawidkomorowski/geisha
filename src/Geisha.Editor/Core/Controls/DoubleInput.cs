using System.Globalization;

namespace Geisha.Editor.Core.Controls
{
    public class DoubleInput : NumberInput<double>
    {
        protected override string Convert(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override bool TryConvert(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}