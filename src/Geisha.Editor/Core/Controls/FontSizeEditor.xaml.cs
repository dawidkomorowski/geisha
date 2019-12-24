using System.Windows;
using System.Windows.Controls;
using Geisha.Engine.Rendering;

namespace Geisha.Editor.Core.Controls
{
    /// <summary>
    /// Interaction logic for FontSizeEditor.xaml
    /// </summary>
    public partial class FontSizeEditor : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(FontSize), typeof(FontSizeEditor),
            new FrameworkPropertyMetadata(default(FontSize), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public FontSizeEditor()
        {
            InitializeComponent();
        }

        public FontSize Value
        {
            get => (FontSize) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (FontSizeEditor) d;
            var newValue = (FontSize) e.NewValue;

            editor.Dips.Value = newValue.Dips;
        }

        private void Dips_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Engine.Rendering.FontSize.FromDips(Dips.Value);
        }
    }
}