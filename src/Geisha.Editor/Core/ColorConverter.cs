using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Geisha.Common.Logging;
using Geisha.Common.Math;

namespace Geisha.Editor.Core
{
    public sealed class ColorConverter : IValueConverter
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ColorConverter));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            }
            else
            {
                Log.Error(
                    $"Color could not be converted. Expected instance of type {typeof(Color).FullName} but received {value?.GetType().FullName}.");
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var wpfColor = (System.Windows.Media.Color) value;
            return Color.FromArgb(wpfColor.A, wpfColor.R, wpfColor.G, wpfColor.B);
        }
    }
}