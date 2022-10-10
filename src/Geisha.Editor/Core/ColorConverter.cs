using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Geisha.Engine.Core.Math;
using NLog;

namespace Geisha.Editor.Core
{
    public sealed class ColorConverter : IValueConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            }
            else
            {
                Logger.Error("Color could not be converted. Expected instance of type {0} but received {1}.", typeof(Color).FullName, value.GetType().FullName);
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var wpfColor = (System.Windows.Media.Color)value;
            return Color.FromArgb(wpfColor.A, wpfColor.R, wpfColor.G, wpfColor.B);
        }
    }
}