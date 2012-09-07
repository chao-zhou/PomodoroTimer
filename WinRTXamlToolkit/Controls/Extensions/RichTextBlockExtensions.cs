using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace WinRTXamlToolkit.Controls.Extensions
{
    public static class RichTextBlockExtensions
    {
        #region PlainText
        /// <summary>
        /// PlainText Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty PlainTextProperty =
            DependencyProperty.RegisterAttached(
                "PlainText",
                typeof(string),
                typeof(RichTextBlockExtensions),
                new PropertyMetadata("", OnPlainTextChanged));

        /// <summary>
        /// Gets the PlainText property. This dependency property 
        /// indicates the plain text to assign to the RichTextBlock.
        /// </summary>
        public static string GetPlainText(DependencyObject d)
        {
            return (string)d.GetValue(PlainTextProperty);
        }

        /// <summary>
        /// Sets the PlainText property. This dependency property 
        /// indicates the plain text to assign to the RichTextBlock.
        /// </summary>
        public static void SetPlainText(DependencyObject d, string value)
        {
            d.SetValue(PlainTextProperty, value);
        }

        /// <summary>
        /// Handles changes to the PlainText property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnPlainTextChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string oldPlainText = (string)e.OldValue;
            string newPlainText = (string)d.GetValue(PlainTextProperty);
            ((RichTextBlock)d).Blocks.Clear();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run { Text = newPlainText });
            ((RichTextBlock)d).Blocks.Add(paragraph);
        }
        #endregion
    }
}
