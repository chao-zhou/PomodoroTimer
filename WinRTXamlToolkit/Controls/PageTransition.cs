using System;
using System.Threading.Tasks;
using WinRTXamlToolkit.AsyncUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace WinRTXamlToolkit.Controls
{
    public enum PageTransitionMode
    {
        Sequential,
        Parallel
    }

    public abstract class PageTransition : DependencyObject
    {
        protected abstract PageTransitionMode Mode { get; }

        #region ForwardOutAnimation
        /// <summary>
        /// ForwardOutAnimation Dependency Property
        /// </summary>
        public static readonly DependencyProperty ForwardOutAnimationProperty =
            DependencyProperty.Register(
                "ForwardOutAnimation",
                typeof(PageTransitionAnimation),
                typeof(PageTransition),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ForwardOutAnimation property. This dependency property 
        /// indicates the animation to use during forward navigation to remove the previous page from view.
        /// </summary>
        public PageTransitionAnimation ForwardOutAnimation
        {
            get { return (PageTransitionAnimation)GetValue(ForwardOutAnimationProperty); }
            set { SetValue(ForwardOutAnimationProperty, value); }
        }
        #endregion

        #region ForwardInAnimation
        /// <summary>
        /// ForwardInAnimation Dependency Property
        /// </summary>
        public static readonly DependencyProperty ForwardInAnimationProperty =
            DependencyProperty.Register(
                "ForwardInAnimation",
                typeof(PageTransitionAnimation),
                typeof(PageTransition),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ForwardInAnimation property. This dependency property 
        /// indicates the animation to use during forward navigation to bring the new page into view.
        /// </summary>
        public PageTransitionAnimation ForwardInAnimation
        {
            get { return (PageTransitionAnimation)GetValue(ForwardInAnimationProperty); }
            set { SetValue(ForwardInAnimationProperty, value); }
        }
        #endregion

        #region BackwardOutAnimation
        /// <summary>
        /// BackwardOutAnimation Dependency Property
        /// </summary>
        public static readonly DependencyProperty BackwardOutAnimationProperty =
            DependencyProperty.Register(
                "BackwardOutAnimation",
                typeof(PageTransitionAnimation),
                typeof(PageTransition),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the BackwardOutAnimation property. This dependency property 
        /// indicates the animation to use during backward navigation to remove the previous page from view.
        /// </summary>
        public PageTransitionAnimation BackwardOutAnimation
        {
            get { return (PageTransitionAnimation)GetValue(BackwardOutAnimationProperty); }
            set { SetValue(BackwardOutAnimationProperty, value); }
        }
        #endregion

        #region BackwardInAnimation
        /// <summary>
        /// BackwardInAnimation Dependency Property
        /// </summary>
        public static readonly DependencyProperty BackwardInAnimationProperty =
            DependencyProperty.Register(
                "BackwardInAnimation",
                typeof(PageTransitionAnimation),
                typeof(PageTransition),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the BackwardInAnimation property. This dependency property 
        /// indicates the animation to use during backward navigation to bring the new page into view.
        /// </summary>
        public PageTransitionAnimation BackwardInAnimation
        {
            get { return (PageTransitionAnimation)GetValue(BackwardInAnimationProperty); }
            set { SetValue(BackwardInAnimationProperty, value); }
        }
        #endregion

        #region TransitionForward()
        public async Task TransitionForward(DependencyObject previousPage, DependencyObject newPage)
        {
            if (previousPage == null && newPage == null)
            {
                throw new InvalidOperationException();
            }

            if (previousPage == null)
            {
                await ForwardInAnimation.Animate(newPage);
            }
            else if (newPage == null)
            {
                await ForwardOutAnimation.Animate(previousPage);
            }
            else if (this.Mode == PageTransitionMode.Parallel)
            {
                var sb = new Storyboard();
                var outSb = ForwardOutAnimation.GetAnimation(previousPage);
                var inSb = ForwardInAnimation.GetAnimation(newPage);
                sb.Children.Add(outSb);
                sb.Children.Add(inSb);
                await sb.BeginAsync();
                sb.Stop();
                sb.Children.Clear();
                //await Task.WhenAll(
                //    ForwardOutAnimation.Animate(previousPage),
                //    ForwardInAnimation.Animate(newPage));
            }
            else
            {
                await ForwardOutAnimation.Animate(previousPage);
                await ForwardInAnimation.Animate(newPage);
            }
        } 
        #endregion

        #region TransitionBackward()
        public async Task TransitionBackward(DependencyObject previousPage, DependencyObject newPage)
        {
            if (previousPage == null && newPage == null)
            {
                throw new InvalidOperationException();
            }

            if (previousPage == null)
            {
                await BackwardInAnimation.Animate(newPage);
            }
            else if (newPage == null)
            {
                await BackwardOutAnimation.Animate(previousPage);
            }
            else if (this.Mode == PageTransitionMode.Parallel)
            {
                var sb = new Storyboard();
                var outSb = BackwardOutAnimation.GetAnimation(previousPage);
                var inSb = BackwardInAnimation.GetAnimation(newPage);
                sb.Children.Add(outSb);
                sb.Children.Add(inSb);
                await sb.BeginAsync();
                sb.Stop();
                sb.Children.Clear();
                //await Task.WhenAll(
                //    BackwardOutAnimation.Animate(previousPage),
                //    BackwardInAnimation.Animate(newPage));
            }
            else
            {
                await BackwardOutAnimation.Animate(previousPage);
                await BackwardInAnimation.Animate(newPage);
            }
        }
        #endregion
    }

    public abstract class PageTransitionAnimation : DependencyObject
    {
        protected abstract Storyboard Animation { get; }

        protected abstract void ApplyTargetProperties(DependencyObject target, Storyboard animation);

        internal Storyboard GetAnimation(DependencyObject target)
        {
            var anim = this.Animation;
            Storyboard.SetTarget(anim, target);
            this.ApplyTargetProperties(target, anim);
            return anim;
        }

        public async Task Animate(DependencyObject target)
        {
            var anim = this.Animation;
            Storyboard.SetTarget(anim, target);
            this.ApplyTargetProperties(target, anim);
            await anim.BeginAsync();
            anim.Stop();
        }
    }

    public enum SlideDirection
    {
        RightToLeft,
        LeftToRight,
        TopToBottom,
        BottomToTop
    }

    public enum AnimationMode
    {
        In,
        Out
    }

    public class PushTransition : PageTransition
    {
        protected override PageTransitionMode Mode
        {
            get
            {
                return PageTransitionMode.Parallel;
            }
        }

        #region ForwardDirection
        /// <summary>
        /// ForwardDirection Dependency Property
        /// </summary>
        public static readonly DependencyProperty ForwardDirectionProperty =
            DependencyProperty.Register(
                "ForwardDirection",
                typeof(SlideDirection),
                typeof(PushTransition),
                new PropertyMetadata(SlideDirection.RightToLeft, OnForwardDirectionChanged));

        /// <summary>
        /// Gets or sets the ForwardDirection property. This dependency property 
        /// indicates the forward transition direction.
        /// </summary>
        public SlideDirection ForwardDirection
        {
            get { return (SlideDirection)GetValue(ForwardDirectionProperty); }
            set { SetValue(ForwardDirectionProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ForwardDirection property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnForwardDirectionChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (PushTransition)d;
            SlideDirection oldForwardDirection = (SlideDirection)e.OldValue;
            SlideDirection newForwardDirection = target.ForwardDirection;
            target.OnForwardDirectionChanged(oldForwardDirection, newForwardDirection);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ForwardDirection property.
        /// </summary>
        /// <param name="oldForwardDirection">The old ForwardDirection value</param>
        /// <param name="newForwardDirection">The new ForwardDirection value</param>
        protected virtual void OnForwardDirectionChanged(
            SlideDirection oldForwardDirection, SlideDirection newForwardDirection)
        {
            ((SlideAnimation)this.ForwardInAnimation).Direction = newForwardDirection;
            ((SlideAnimation)this.ForwardOutAnimation).Direction = newForwardDirection;
        }
        #endregion

        #region BackwardDirection
        /// <summary>
        /// BackwardDirection Dependency Property
        /// </summary>
        public static readonly DependencyProperty BackwardDirectionProperty =
            DependencyProperty.Register(
                "BackwardDirection",
                typeof(SlideDirection),
                typeof(PushTransition),
                new PropertyMetadata(SlideDirection.LeftToRight, OnBackwardDirectionChanged));

        /// <summary>
        /// Gets or sets the BackwardDirection property. This dependency property 
        /// indicates the backward transition direction.
        /// </summary>
        public SlideDirection BackwardDirection
        {
            get { return (SlideDirection)GetValue(BackwardDirectionProperty); }
            set { SetValue(BackwardDirectionProperty, value); }
        }

        /// <summary>
        /// Handles changes to the BackwardDirection property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnBackwardDirectionChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (PushTransition)d;
            SlideDirection oldBackwardDirection = (SlideDirection)e.OldValue;
            SlideDirection newBackwardDirection = target.BackwardDirection;
            target.OnBackwardDirectionChanged(oldBackwardDirection, newBackwardDirection);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the BackwardDirection property.
        /// </summary>
        /// <param name="oldBackwardDirection">The old BackwardDirection value</param>
        /// <param name="newBackwardDirection">The new BackwardDirection value</param>
        protected virtual void OnBackwardDirectionChanged(
            SlideDirection oldBackwardDirection, SlideDirection newBackwardDirection)
        {
            ((SlideAnimation)this.BackwardInAnimation).Direction = newBackwardDirection;
            ((SlideAnimation)this.BackwardOutAnimation).Direction = newBackwardDirection;
        }
        #endregion

        public PushTransition()
        {
            this.ForwardOutAnimation =
                new SlideAnimation
                {
                    Direction = ForwardDirection,
                    Mode = AnimationMode.Out
                };
            this.ForwardInAnimation =
                new SlideAnimation
                {
                    Direction = ForwardDirection,
                    Mode = AnimationMode.In
                };
            this.BackwardOutAnimation =
                new SlideAnimation
                {
                    Direction = BackwardDirection,
                    Mode = AnimationMode.Out
                };
            this.BackwardInAnimation =
                new SlideAnimation
                {
                    Direction = BackwardDirection,
                    Mode = AnimationMode.In
                };
        }
    }

    public class SlideAnimation : PageTransitionAnimation
    {
        //private Storyboard _sb;
        //private DoubleAnimation _da;

        #region Direction
        /// <summary>
        /// Direction Dependency Property
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(
                "Direction",
                typeof(SlideDirection),
                typeof(SlideAnimation),
                new PropertyMetadata(SlideDirection.RightToLeft));

        /// <summary>
        /// Gets or sets the Direction property. This dependency property 
        /// indicates the slide direction.
        /// </summary>
        public SlideDirection Direction
        {
            get { return (SlideDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
        #endregion

        #region Mode
        /// <summary>
        /// Mode Dependency Property
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                "Mode",
                typeof(AnimationMode),
                typeof(SlideAnimation),
                new PropertyMetadata(AnimationMode.Out));

        /// <summary>
        /// Gets or sets the Mode property. This dependency property 
        /// indicates whether this is an animation to slide in or out.
        /// </summary>
        public AnimationMode Mode
        {
            get { return (AnimationMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        #endregion

        protected override Storyboard Animation
        {
            get
            {
                //return _sb;

                // NOTE: There seem to be problems with WinRT when reusing same Storyboard for multiple elements, so we need to always get a new storyboard.
                var sb = new Storyboard();
                var da = new DoubleAnimation();
                da.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
                da.Duration = TimeSpan.FromSeconds(0.4);
                sb.Children.Add(da);

                return sb;
            }
        }

        public SlideAnimation()
        {
            //_sb = new Storyboard();
            //_da = new DoubleAnimation();
            //_da.EasingFunction = new CubicEase {EasingMode = EasingMode.EaseOut};
            //_da.Duration = TimeSpan.FromSeconds(0.4);
            //_sb.Children.Add(_da);
        }

        protected override void ApplyTargetProperties(DependencyObject target, Storyboard animation)
        {
            var fe = (FrameworkElement)target;
            TranslateTransform tt = fe.RenderTransform as TranslateTransform;

            if (tt == null)
            {
                fe.RenderTransform = tt = new TranslateTransform();
            }

            var da = (DoubleAnimation)animation.Children[0];

            Storyboard.SetTarget(da, tt);

            if (Direction == SlideDirection.RightToLeft ||
                Direction == SlideDirection.LeftToRight)
            {
                Storyboard.SetTargetProperty(da, "X");

                if (Mode == AnimationMode.In)
                {
                    da.From =
                        Direction == SlideDirection.LeftToRight
                            ? -fe.ActualWidth
                            : fe.ActualWidth;
                    da.To = 0;
                }
                else
                {
                    da.From = 0;
                    da.To =
                        Direction == SlideDirection.LeftToRight
                            ? fe.ActualWidth
                            : -fe.ActualWidth;
                }
            }
            else
            {
                Storyboard.SetTargetProperty(da, "Y");

                if (Mode == AnimationMode.In)
                {
                    da.From =
                        Direction == SlideDirection.TopToBottom
                            ? -fe.ActualHeight
                            : fe.ActualHeight;
                    da.To = 0;
                }
                else
                {
                    da.From = 0;
                    da.To =
                        Direction == SlideDirection.TopToBottom
                            ? fe.ActualHeight
                            : -fe.ActualHeight;
                }
            }
        }
    }
}
