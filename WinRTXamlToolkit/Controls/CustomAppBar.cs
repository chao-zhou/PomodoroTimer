using System;
using System.Linq;
using WinRTXamlToolkit.Controls.Extensions;
using Windows.Devices.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace WinRTXamlToolkit.Controls
{
    [TemplateVisualState(GroupName=FloatingStatesGroupName, Name=FloatingVisibleStateName)]
    [TemplateVisualState(GroupName=FloatingStatesGroupName, Name=FloatingHiddenStateName)]
    [TemplatePart(Name=LayoutRootPartName)]
    [TemplatePart(Name=TransitionTransformPartName, Type=typeof(CompositeTransform))]
    [TemplatePart(Name=FloatingVisibleHorizontalTransitionPartName, Type=typeof(DoubleAnimation))]
    [TemplatePart(Name=FloatingVisibleVerticalTransitionPartName, Type=typeof(DoubleAnimation))]
    [TemplatePart(Name=FloatingHiddenHorizontalTransitionPartName, Type=typeof(DoubleAnimation))]
    [TemplatePart(Name=FloatingHiddenVerticalTransitionPartName, Type=typeof(DoubleAnimation))]
    public class CustomAppBar : ContentControl
    {
        #region Template Part and Visual State names
        private const string FloatingStatesGroupName = "FloatingStates";
        private const string FloatingVisibleStateName = "FloatingVisible";
        private const string FloatingHiddenStateName = "FloatingHidden";
        private const string LayoutRootPartName = "PART_LayoutRoot";
        private const string TransitionTransformPartName = "PART_TransitionTransform";
        private const string FloatingVisibleHorizontalTransitionPartName = "PART_FloatingVisibleHorizontalTransition";
        private const string FloatingVisibleVerticalTransitionPartName = "PART_FloatingVisibleVerticalTransition";
        private const string FloatingHiddenHorizontalTransitionPartName = "PART_FloatingHiddenHorizontalTransition";
        private const string FloatingHiddenVerticalTransitionPartName = "PART_FloatingHiddenVerticalTransition"; 
        #endregion

        private Popup _lightDismissPopup;
        bool _rightMouseButtonPressed;

        private DoubleAnimation _floatingVisibleHorizontalTransition;
        private DoubleAnimation _floatingVisibleVerticalTransition;
        private DoubleAnimation _floatingHiddenHorizontalTransition;
        private DoubleAnimation _floatingHiddenVerticalTransition;
        private CompositeTransform _transitionTransform;
        private FrameworkElement _layoutRoot;

        public event EventHandler<object> Opened;
        public event EventHandler<object> Closed;

        #region IsOpen
        /// <summary>
        /// IsOpen Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                "IsOpen",
                typeof(bool),
                typeof(CustomAppBar),
                new PropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// Gets or sets the IsOpen property. This dependency property 
        /// indicates whether the AppBar is visible.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsOpen property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsOpenChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var appBar = (CustomAppBar)d;
            bool oldIsOpen = (bool)e.OldValue;
            bool newIsOpen = appBar.IsOpen;
            appBar.OnIsOpenChanged(oldIsOpen, newIsOpen);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the IsOpen property.
        /// </summary>
        /// <param name="oldIsOpen">The old IsOpen value</param>
        /// <param name="newIsOpen">The new IsOpen value</param>
        protected virtual void OnIsOpenChanged(
            bool oldIsOpen, bool newIsOpen)
        {
            if (newIsOpen)
            {
                GoToFloatingVisibleVisualState();

                if (IsLightDismissEnabled)
                    OpenLightDismissPopup();

                OnOpenedInternal();
            }
            else
            {
                GoToFloatingHiddenVisualState();

                if (IsLightDismissEnabled)
                    CloseLightDismissPopup();

                OnClosedInternal();
            }
        }
        #endregion

        #region CanOpen
        /// <summary>
        /// CanOpen Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanOpenProperty =
            DependencyProperty.Register(
                "CanOpen",
                typeof(bool),
                typeof(CustomAppBar),
                new PropertyMetadata(true, OnCanOpenChanged));

        /// <summary>
        /// Gets or sets the CanOpen property. This dependency property 
        /// indicates whether the AppBar can open using the standard gestures.
        /// </summary>
        public bool CanOpen
        {
            get { return (bool)GetValue(CanOpenProperty); }
            set { SetValue(CanOpenProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CanOpen property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnCanOpenChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var appBar = (CustomAppBar)d;
            bool oldCanOpen = (bool)e.OldValue;
            bool newCanOpen = appBar.CanOpen;
            appBar.OnCanOpenChanged(oldCanOpen, newCanOpen);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the CanOpen property.
        /// </summary>
        /// <param name="oldCanOpen">The old CanOpen value</param>
        /// <param name="newCanOpen">The new CanOpen value</param>
        protected virtual void OnCanOpenChanged(
            bool oldCanOpen, bool newCanOpen)
        {
            if (!newCanOpen)
            {
                IsOpen = false;
            }
        }
        #endregion

        #region CanDismiss
        /// <summary>
        /// CanDismiss Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanDismissProperty =
            DependencyProperty.Register(
                "CanDismiss",
                typeof(bool),
                typeof(CustomAppBar),
                new PropertyMetadata(true, OnCanDismissChanged));

        /// <summary>
        /// Gets or sets the CanDismiss property. This dependency property 
        /// indicates whether the AppBar can be dismissed.
        /// </summary>
        public bool CanDismiss
        {
            get { return (bool)GetValue(CanDismissProperty); }
            set { SetValue(CanDismissProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CanDismiss property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnCanDismissChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var appBar = (CustomAppBar)d;
            bool oldCanDismiss = (bool)e.OldValue;
            bool newCanDismiss = appBar.CanDismiss;
            appBar.OnCanDismissChanged(oldCanDismiss, newCanDismiss);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the CanDismiss property.
        /// </summary>
        /// <param name="oldCanDismiss">The old CanDismiss value</param>
        /// <param name="newCanDismiss">The new CanDismiss value</param>
        protected virtual void OnCanDismissChanged(
            bool oldCanDismiss, bool newCanDismiss)
        {
            if (!newCanDismiss)
            {
                IsOpen = true;
            }
        }
        #endregion

        #region CanOpenInSnappedView
        /// <summary>
        /// CanOpenInSnappedView Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanOpenInSnappedViewProperty =
            DependencyProperty.Register(
                "CanOpenInSnappedView",
                typeof(bool),
                typeof(CustomAppBar),
                new PropertyMetadata(true, OnCanOpenInSnappedViewChanged));

        /// <summary>
        /// Gets or sets the CanOpenInSnappedView property. This dependency property 
        /// indicates whether the AppBar can be opened in snapped view.
        /// </summary>
        public bool CanOpenInSnappedView
        {
            get { return (bool)GetValue(CanOpenInSnappedViewProperty); }
            set { SetValue(CanOpenInSnappedViewProperty, value); }
        }

        /// <summary>
        /// Handles changes to the CanOpenInSnappedView property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnCanOpenInSnappedViewChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var customAppBar = (CustomAppBar)d;
            bool oldCanOpenInSnappedView = (bool)e.OldValue;
            bool newCanOpenInSnappedView = customAppBar.CanOpenInSnappedView;
            customAppBar.OnCanOpenInSnappedViewChanged(oldCanOpenInSnappedView, newCanOpenInSnappedView);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the CanOpenInSnappedView property.
        /// </summary>
        /// <param name="oldCanOpenInSnappedView">The old CanOpenInSnappedView value</param>
        /// <param name="newCanOpenInSnappedView">The new CanOpenInSnappedView value</param>
        protected virtual void OnCanOpenInSnappedViewChanged(
            bool oldCanOpenInSnappedView, bool newCanOpenInSnappedView)
        {
            if (!newCanOpenInSnappedView &&
                ApplicationView.Value == ApplicationViewState.Snapped)
            {
                IsOpen = false;
            }
        }
        #endregion

        #region IsLightDismissEnabled
        /// <summary>
        /// IsLightDismissEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsLightDismissEnabledProperty =
            DependencyProperty.Register(
                "IsLightDismissEnabled",
                typeof(bool),
                typeof(CustomAppBar),
                new PropertyMetadata(false, OnIsLightDismissEnabledChanged));

        /// <summary>
        /// Gets or sets the IsLightDismissEnabled property. This dependency property 
        /// indicates whether the app bar can be dismissed by tapping anywhere outside of the control.
        /// </summary>
        public bool IsLightDismissEnabled
        {
            get { return (bool)GetValue(IsLightDismissEnabledProperty); }
            set { SetValue(IsLightDismissEnabledProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsLightDismissEnabled property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsLightDismissEnabledChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (CustomAppBar)d;
            bool oldIsLightDismissEnabled = (bool)e.OldValue;
            bool newIsLightDismissEnabled = target.IsLightDismissEnabled;
            target.OnIsLightDismissEnabledChanged(oldIsLightDismissEnabled, newIsLightDismissEnabled);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the IsLightDismissEnabled property.
        /// </summary>
        /// <param name="oldIsLightDismissEnabled">The old IsLightDismissEnabled value</param>
        /// <param name="newIsLightDismissEnabled">The new IsLightDismissEnabled value</param>
        protected virtual void OnIsLightDismissEnabledChanged(
            bool oldIsLightDismissEnabled, bool newIsLightDismissEnabled)
        {
            if (IsOpen)
            {
                if (newIsLightDismissEnabled)
                    OpenLightDismissPopup();
                else
                    CloseLightDismissPopup();
            }
        }
        #endregion

        public CustomAppBar()
        {
            this.DefaultStyleKey = typeof(CustomAppBar);
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EdgeGesture.GetForCurrentView().Completed += OnEdgeGestureCompleted;
            Window.Current.SizeChanged += WindowSizeChanged;
            Window.Current.Content.PointerPressed += OnLayoutRootPointerPressed;
            Window.Current.Content.PointerReleased += OnLayoutRootPointerReleased;

            if (!IsOpen)
            {
                SetAppBarPositionOutsideClipBounds();
            }
        }

        private void SetAppBarPositionOutsideClipBounds()
        {
            if (_transitionTransform == null)
                return;
            ;
            if (this.VerticalAlignment == VerticalAlignment.Bottom)
            {
                _transitionTransform.TranslateY = this.ActualHeight;
            }
            else if (this.VerticalAlignment == VerticalAlignment.Top)
            {
                _transitionTransform.TranslateY = -this.ActualHeight;
            }
            if (this.HorizontalAlignment == HorizontalAlignment.Left)
            {
                _transitionTransform.TranslateX = -this.ActualWidth;
            }
            else if (this.HorizontalAlignment == HorizontalAlignment.Right)
            {
                _transitionTransform.TranslateX = this.ActualWidth;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            IsOpen = false; // TODO: Find a better solution for this workaround for an issue with the light dismiss popup staying visible after the user navigates to another page by clicking a button on the app bar
            EdgeGesture.GetForCurrentView().Completed -= OnEdgeGestureCompleted;
            Window.Current.SizeChanged -= WindowSizeChanged;
            Window.Current.Content.PointerPressed -= OnLayoutRootPointerPressed;
            Window.Current.Content.PointerReleased -= OnLayoutRootPointerReleased;
        }

        private void OnClosedInternal()
        {
            OnClosed(null);

            if (this.Closed != null)
                this.Closed(this, null);
        }

        internal void OnOpenedInternal()
        {
            OnOpened(null);

            if (this.Opened!= null)
                this.Opened(this, null);
        }

        protected virtual void OnClosed(object e)
        {
        }

        protected virtual void OnOpened(object e)
        {
        }

        private void OnEdgeGestureCompleted(EdgeGesture sender, EdgeGestureEventArgs args)
        {
            OnSwitchGesture();
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (!CanOpenInSnappedView)
            {
                this.IsOpen = false;
            }
        }

        private void OnLayoutRootPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _rightMouseButtonPressed = 
                    e.GetCurrentPoint((UIElement)sender).Properties.IsRightButtonPressed &&
                    !e.GetCurrentPoint((UIElement)sender).Properties.IsLeftButtonPressed &&
                    !e.GetCurrentPoint((UIElement)sender).Properties.IsMiddleButtonPressed;
            }
        }

        private void OnLayoutRootPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                !e.GetCurrentPoint((UIElement)sender).Properties.IsLeftButtonPressed &&
                !e.GetCurrentPoint((UIElement)sender).Properties.IsMiddleButtonPressed &&
                _rightMouseButtonPressed)
            {
                OnSwitchGesture();
            }

            _rightMouseButtonPressed = false;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _transitionTransform = GetTemplateChild(TransitionTransformPartName) as CompositeTransform;
            _layoutRoot = GetTemplateChild(LayoutRootPartName) as FrameworkElement;

            if (_layoutRoot == null)
                return;

            var visualStateGroups = VisualStateManager.GetVisualStateGroups(_layoutRoot);

            if (visualStateGroups == null)
                return;

            var floatingStatesGroup = visualStateGroups.FirstOrDefault(group => group.Name == FloatingStatesGroupName);

            if (floatingStatesGroup == null)
                return;

            var floatingVisibleState =
                floatingStatesGroup.States.FirstOrDefault(
                    state => state.Name == FloatingVisibleStateName);

            if (floatingVisibleState != null &&
                floatingVisibleState.Storyboard != null)
            {
                _floatingVisibleHorizontalTransition =
                    floatingVisibleState.Storyboard.Children.FirstOrDefault(
                        timeline =>
                            Storyboard.GetTargetName(timeline) == TransitionTransformPartName &&
                            Storyboard.GetTargetProperty(timeline) == "TranslateX") as DoubleAnimation;
                _floatingVisibleVerticalTransition =
                    floatingVisibleState.Storyboard.Children.FirstOrDefault(
                        timeline =>
                            Storyboard.GetTargetName(timeline) == TransitionTransformPartName &&
                            Storyboard.GetTargetProperty(timeline) == "TranslateY") as DoubleAnimation;
            }

            var floatingHiddenState =
                floatingStatesGroup.States.FirstOrDefault(
                    state => state.Name == FloatingHiddenStateName);

            if (floatingHiddenState != null &&
                floatingHiddenState.Storyboard != null)
            {
                _floatingHiddenHorizontalTransition =
                    floatingHiddenState.Storyboard.Children.FirstOrDefault(
                        timeline =>
                            Storyboard.GetTargetName(timeline) == TransitionTransformPartName &&
                            Storyboard.GetTargetProperty(timeline) == "TranslateX") as DoubleAnimation;
                _floatingHiddenVerticalTransition =
                    floatingHiddenState.Storyboard.Children.FirstOrDefault(
                        timeline =>
                            Storyboard.GetTargetName(timeline) == TransitionTransformPartName &&
                            Storyboard.GetTargetProperty(timeline) == "TranslateY") as DoubleAnimation;
            }

            //this.FloatingVisibleHorizontalTransition = GetTemplateChild(FloatingVisibleHorizontalTransitionPartName) as DoubleAnimation;
            //this.FloatingVisibleVerticalTransition = GetTemplateChild(FloatingVisibleVerticalTransitionPartName) as DoubleAnimation;
            //this.FloatingHiddenHorizontalTransition = GetTemplateChild(FloatingVisibleHorizontalTransitionPartName) as DoubleAnimation;
            //this.FloatingHiddenVerticalTransition = GetTemplateChild(FloatingVisibleVerticalTransitionPartName) as DoubleAnimation;

            GoToFloatingHiddenVisualState();
        }

        private void OnSwitchGesture()
        {
            if (CanOpen && !IsOpen)
            {
                IsOpen = true;
            }
            else if (CanDismiss && IsOpen)
            {
                IsOpen = false;
            }
        }

        private void GoToFloatingHiddenVisualState()
        {
            if (this.VerticalAlignment == VerticalAlignment.Bottom &&
                _floatingHiddenVerticalTransition != null)
            {
                _floatingHiddenVerticalTransition.To = this.ActualHeight;
            }
            else if (this.VerticalAlignment == VerticalAlignment.Top &&
                _floatingHiddenVerticalTransition != null)
            {
                _floatingHiddenVerticalTransition.To = -this.ActualHeight;
            }
            else if (this.HorizontalAlignment == HorizontalAlignment.Left &&
                _floatingHiddenHorizontalTransition != null)
            {
                _floatingHiddenHorizontalTransition.To = -this.ActualWidth;
            }
            else if (this.HorizontalAlignment == HorizontalAlignment.Right &&
                _floatingHiddenVerticalTransition != null)
            {
                _floatingHiddenHorizontalTransition.To = this.ActualWidth;
            }

            VisualStateManager.GoToState(this, FloatingHiddenStateName, true);
        }

        private void GoToFloatingVisibleVisualState()
        {
            SetAppBarPositionOutsideClipBounds();

            if (_floatingVisibleVerticalTransition!= null)
            {
                _floatingVisibleVerticalTransition.To = 0;
            }

            if (_floatingVisibleHorizontalTransition != null)
            {
                _floatingVisibleHorizontalTransition.To = 0;
            }

            VisualStateManager.GoToState(this, FloatingVisibleStateName, true);
        }

        private void OpenLightDismissPopup()
        {
            var windowRect = ((FrameworkElement) Window.Current.Content).GetBoundingRect();
            var appBarRect = this.GetBoundingRect();

            _lightDismissPopup = new Popup
            {
                Width = windowRect.Width,
                Height = windowRect.Height
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(appBarRect.Top)});
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(appBarRect.Height) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(windowRect.Height - appBarRect.Bottom) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(appBarRect.Left) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(appBarRect.Width) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(windowRect.Width - appBarRect.Right) });
            _lightDismissPopup.Child = grid;

            if (appBarRect.Top > 0)
            {
                var tapTarget = new Rectangle();
                tapTarget.VerticalAlignment = VerticalAlignment.Stretch;
                tapTarget.HorizontalAlignment = HorizontalAlignment.Stretch;
                tapTarget.Fill = new SolidColorBrush(Colors.Transparent);
                Grid.SetRow(tapTarget, 0);
                Grid.SetColumnSpan(tapTarget, 3);
                tapTarget.Tapped += OnTapTargetTapped;
                tapTarget.RightTapped += OnTapTargetRightTapped;
                grid.Children.Add(tapTarget);
            }

            if (appBarRect.Left > 0)
            {
                var tapTarget = new Rectangle();
                tapTarget.VerticalAlignment = VerticalAlignment.Stretch;
                tapTarget.HorizontalAlignment = HorizontalAlignment.Stretch;
                tapTarget.Fill = new SolidColorBrush(Colors.Transparent);
                Grid.SetRow(tapTarget, 1);
                tapTarget.Tapped += OnTapTargetTapped;
                tapTarget.RightTapped += OnTapTargetRightTapped;
                grid.Children.Add(tapTarget);
            }

            if (appBarRect.Right < windowRect.Right)
            {
                var tapTarget = new Rectangle();
                tapTarget.VerticalAlignment = VerticalAlignment.Stretch;
                tapTarget.HorizontalAlignment = HorizontalAlignment.Stretch;
                tapTarget.Fill = new SolidColorBrush(Colors.Transparent);
                Grid.SetRow(tapTarget, 1);
                Grid.SetColumn(tapTarget, 2);
                tapTarget.Tapped += OnTapTargetTapped;
                tapTarget.RightTapped += OnTapTargetRightTapped;
                grid.Children.Add(tapTarget);
            }

            if (appBarRect.Bottom < windowRect.Bottom)
            {
                var tapTarget = new Rectangle();
                tapTarget.VerticalAlignment = VerticalAlignment.Stretch;
                tapTarget.HorizontalAlignment = HorizontalAlignment.Stretch;
                tapTarget.Fill = new SolidColorBrush(Colors.Transparent);
                Grid.SetRow(tapTarget, 2);
                Grid.SetColumnSpan(tapTarget, 3);
                tapTarget.Tapped += OnTapTargetTapped;
                tapTarget.RightTapped += OnTapTargetRightTapped;
                grid.Children.Add(tapTarget);
            }

            _lightDismissPopup.IsOpen = true;
        }

        private void OnTapTargetRightTapped(object sender, RightTappedRoutedEventArgs rightTappedRoutedEventArgs)
        {
            this.IsOpen = false;
        }

        private void OnTapTargetTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            this.IsOpen = false;
        }

        private void CloseLightDismissPopup()
        {
            var grid = (Grid) _lightDismissPopup.Child;

            foreach (var rect in grid.Children)
            {
                rect.Tapped -= OnTapTargetTapped;
                rect.RightTapped -= OnTapTargetRightTapped;
            }

            _lightDismissPopup.IsOpen = false;
            _lightDismissPopup = null;
        }
    }
}
