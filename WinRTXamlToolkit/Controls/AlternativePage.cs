using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinRTXamlToolkit.Controls
{
    public class AlternativePage : UserControl
    {
        #region Frame
        /// <summary>
        /// Frame Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrameProperty =
            DependencyProperty.Register(
                "Frame",
                typeof(AlternativeFrame),
                typeof(AlternativePage),
                new PropertyMetadata(null, OnFrameChanged));

        /// <summary>
        /// Gets or sets the Frame property. This dependency property 
        /// indicates the frame the page is hosted in.
        /// </summary>
        public AlternativeFrame Frame
        {
            get { return (AlternativeFrame)GetValue(FrameProperty); }
            internal set { SetValue(FrameProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Frame property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnFrameChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (AlternativePage)d;
            AlternativeFrame oldFrame = (AlternativeFrame)e.OldValue;
            AlternativeFrame newFrame = target.Frame;
            target.OnFrameChanged(oldFrame, newFrame);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the Frame property.
        /// </summary>
        /// <param name="oldFrame">The old Frame value</param>
        /// <param name="newFrame">The new Frame value</param>
        protected virtual void OnFrameChanged(
            AlternativeFrame oldFrame, AlternativeFrame newFrame)
        {
        }
        #endregion

        #region ShouldWaitForImagesToLoad
        /// <summary>
        /// ShouldWaitForImagesToLoad Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShouldWaitForImagesToLoadProperty =
            DependencyProperty.Register(
                "ShouldWaitForImagesToLoad",
                typeof(bool?),
                typeof(AlternativePage),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ShouldWaitForImagesToLoad property. This dependency property 
        /// indicates whether the frame should wait for all images in a page to load
        /// before transitioning to the next page.
        /// </summary>
        public bool? ShouldWaitForImagesToLoad
        {
            get { return (bool?)GetValue(ShouldWaitForImagesToLoadProperty); }
            set { SetValue(ShouldWaitForImagesToLoadProperty, value); }
        }
        #endregion

        

        public AlternativePage()
        {
        }

        // Summary:
        //     Invoked immediately after the Page is unloaded and is no longer the current
        //     source of a parent Frame.
        //
        // Parameters:
        //   e:
        //     Event data that can be examined by overriding code. The event data is representative
        //     of the navigation that has unloaded the current Page.
        protected virtual async Task OnNavigatedFrom(AlternativeNavigationEventArgs e)
        {
        }

        //
        // Summary:
        //     Invoked when the Page is loaded and becomes the current source of a parent
        //     Frame.
        //
        // Parameters:
        //   e:
        //     Event data that can be examined by overriding code. The event data is representative
        //     of the pending navigation that will load the current Page. Usually the most
        //     relevant property to examine is Parameter.
        protected virtual async Task OnNavigatedTo(AlternativeNavigationEventArgs e)
        {
        }

        //
        // Summary:
        //     Invoked immediately before the Page is unloaded and is no longer the current
        //     source of a parent Frame.
        //
        // Parameters:
        //   e:
        //     Event data that can be examined by overriding code. The event data is representative
        //     of the navigation that will unload the current Page unless canceled. The
        //     navigation can potentially be canceled by setting Cancel.
        protected virtual async Task OnNavigatingFrom(AlternativeNavigatingCancelEventArgs e)
        {
        }

        protected virtual async Task OnNavigatingTo(AlternativeNavigationEventArgs e)
        {
        }

        internal async Task OnNavigatingFromInternal(AlternativeNavigatingCancelEventArgs e)
        {
            await OnNavigatingFrom(e);
        }

        /// <summary>
        /// The last call before page transition occurs, but after the page has been added to visual tree.
        /// An opportunity to wait for some limited loading to complete before the transition animation is played.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal async Task OnNavigatingToInternal(AlternativeNavigationEventArgs e)
        {
            await OnNavigatingTo(e);
        }

        internal async Task OnNavigatedFromInternal(AlternativeNavigationEventArgs e)
        {
            await OnNavigatedFrom(e);
        }

        internal async Task OnNavigatedToInternal(AlternativeNavigationEventArgs e)
        {
            await OnNavigatedTo(e);
        }

        protected virtual async Task Preload(object parameter)
        {
        }

        protected virtual async Task UnloadPreloaded()
        {
        }

        internal async Task PreloadInternal(object parameter)
        {
            await Preload(parameter);
        }

        internal async Task UnloadPreloadedInternal()
        {
            await UnloadPreloaded();
        }
    }

    // Summary:
    //     Provides data for navigation methods and event handlers that cannot cancel
    //     the navigation request.
    public class AlternativeNavigationEventArgs
    {
        // Summary:
        //     Gets the root node of the target page's content.
        //
        // Returns:
        //     The root node of the target page's content.
        public object Content { get; private set; }
        //
        // Summary:
        //     Gets a value that indicates the direction of movement during navigation
        //
        // Returns:
        //     A value of the enumeration.
        public NavigationMode NavigationMode { get; private set; }
        //
        // Summary:
        //     Gets any Parameter object passed to the target page for the navigation.
        //
        // Returns:
        //     An object that potentially passes parameters to the navigation target. May
        //     be null.
        public object Parameter { get; private set; }
        //
        // Summary:
        //     Gets the data type of the target page.
        //
        // Returns:
        //     The data type of the target page, represented as namespace.type or simply
        //     type.
        public Type SourcePageType { get; private set; }

        public AlternativeNavigationEventArgs(object content, NavigationMode navigationMode, object parameter, Type sourcePageType)
        {
            this.Content = content;
            this.NavigationMode = navigationMode;
            this.Parameter = parameter;
            this.SourcePageType = sourcePageType;
        }
    }

    // Summary:
    //     Provides event data for the OnNavigatingFrom callback that can be used to
    //     cancel a navigation request from origination.
    public class AlternativeNavigatingCancelEventArgs
    {
        // Summary:
        //     Specifies whether a pending navigation should be canceled.
        //
        // Returns:
        //     True to cancel the pending cancelable navigation; false to continue with
        //     navigation.
        public bool Cancel { get; set; }
        //
        // Summary:
        //     Gets the value of the mode parameter from the originating Navigate call.
        //
        // Returns:
        //     The value of the mode parameter from the originating Navigate call.
        public NavigationMode NavigationMode { get; private set; }
        //
        // Summary:
        //     Gets the value of the SourcePageType parameter from the originating Navigate
        //     call.
        //
        // Returns:
        //     The value of the SourcePageType parameter from the originating Navigate call.
        public Type SourcePageType { get; private set; }

        public AlternativeNavigatingCancelEventArgs(NavigationMode navigationMode, Type sourcePageType)
        {
            this.NavigationMode = navigationMode;
            this.SourcePageType = sourcePageType;
        }
    }
}
