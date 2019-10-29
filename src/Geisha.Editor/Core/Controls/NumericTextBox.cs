using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Geisha.Editor.Core.Controls
{
    public class NumericTextBox : TextBox
    {
        private bool _shouldSelectAll = true;

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            var regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _shouldSelectAll = true;
        }

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (_shouldSelectAll && IsFocused)
            {
                _shouldSelectAll = false;
                SelectAll();
            }
        }
    }
}