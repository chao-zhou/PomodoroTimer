using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Controls.Extensions
{
    public static class ControlExtensions
    {
        #region Cursor
        /// <summary>
        /// Cursor Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached(
                "Cursor",
                typeof(CoreCursor),
                typeof(ControlExtensions),
                new PropertyMetadata(null, OnCursorChanged));

        /// <summary>
        /// Gets the Cursor property. This dependency property 
        /// indicates the cursor to use when a mouse cursor is moved over the control.
        /// </summary>
        public static CoreCursor GetCursor(DependencyObject d)
        {
            return (CoreCursor)d.GetValue(CursorProperty);
        }

        /// <summary>
        /// Sets the Cursor property. This dependency property 
        /// indicates the cursor to use when a mouse cursor is moved over the control.
        /// </summary>
        public static void SetCursor(DependencyObject d, CoreCursor value)
        {
            d.SetValue(CursorProperty, value);
        }

        /// <summary>
        /// Handles changes to the Cursor property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnCursorChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoreCursor oldCursor = (CoreCursor)e.OldValue;
            CoreCursor newCursor = (CoreCursor)d.GetValue(CursorProperty);

            if (oldCursor == null)
            {
                var handler = new CursorDisplayHandler();
                handler.Attach((Control)d);
                SetCursorDisplayHandler(d, handler);
            }
            else
            {
                var handler = GetCursorDisplayHandler(d);

                if (newCursor == null)
                {
                    handler.Detach();
                    SetCursorDisplayHandler(d, null);
                }
                else
                {
                    handler.UpdateCursor();
                }
            }
        }
        #endregion

        #region CursorDisplayHandler
        /// <summary>
        /// CursorDisplayHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CursorDisplayHandlerProperty =
            DependencyProperty.RegisterAttached(
                "CursorDisplayHandler",
                typeof(CursorDisplayHandler),
                typeof(ControlExtensions),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the CursorDisplayHandler property. This dependency property 
        /// indicates the handler for displaying the Cursor when a mouse is moved over the control.
        /// </summary>
        public static CursorDisplayHandler GetCursorDisplayHandler(DependencyObject d)
        {
            return (CursorDisplayHandler)d.GetValue(CursorDisplayHandlerProperty);
        }

        /// <summary>
        /// Sets the CursorDisplayHandler property. This dependency property 
        /// indicates the handler for displaying the Cursor when a mouse is moved over the control.
        /// </summary>
        public static void SetCursorDisplayHandler(DependencyObject d, CursorDisplayHandler value)
        {
            d.SetValue(CursorDisplayHandlerProperty, value);
        }
        #endregion
    }

    public class CursorDisplayHandler
    {
        private static CoreCursor defaultCursor;
        private static CoreCursor DefaultCursor
        {
            get
            {
                return defaultCursor ?? (defaultCursor = Window.Current.CoreWindow.PointerCursor);
            }
        }

        private Control control;
        private bool isHovering;

        public void Attach(Control c)
        {
            this.control = c;
            this.control.PointerEntered += OnPointerEntered;
            this.control.PointerExited += OnPointerExited;
            this.control.Unloaded += OnControlUnloaded;
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        public void Detach()
        {
            this.control.PointerEntered -= OnPointerEntered;
            this.control.PointerExited -= OnPointerExited;
            this.control.Unloaded -= OnControlUnloaded;

            if (isHovering)
            {
                Window.Current.CoreWindow.PointerCursor = DefaultCursor;
            }
        }

        private void OnPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                this.isHovering = true;
                UpdateCursor();
            }
        }

        private void OnPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                this.isHovering = false;
                Window.Current.CoreWindow.PointerCursor = DefaultCursor;
            }
        }

        internal void UpdateCursor()
        {
            if (defaultCursor == null)
            {
                defaultCursor = Window.Current.CoreWindow.PointerCursor;
            }

            var cursor = ControlExtensions.GetCursor(this.control);

            if (this.isHovering)
            {
                if (cursor != null)
                {
                    Window.Current.CoreWindow.PointerCursor = cursor;
                }
                else
                {
                    Window.Current.CoreWindow.PointerCursor = DefaultCursor;
                }
            }
        }
    }
}
