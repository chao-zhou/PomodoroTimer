using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using WinRTXamlToolkit.AsyncUI;

namespace WinRTXamlToolkit.Controls
{
    public static class DependencyObjectAnimationExtensions
    {
        /// <summary>
        /// Fades the element in using the FadeInThemeAnimation.
        /// </summary>
        /// <remarks>
        /// Opacity property of the element is not affected.<br/>
        /// The duration of the visible animation itself is not affected by the duration parameter. It merely indicates how long the Storyboard will run.<br/>
        /// If FadeOutThemeAnimation was not used on the element before - nothing will happen.<br/>
        /// </remarks>
        /// <param name="dob"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task FadeIn(this DependencyObject dob, TimeSpan? duration = null)
        {
            ((FrameworkElement)dob).Visibility = Visibility.Visible;
            var fadeInStoryboard = new Storyboard();
            var fadeInAnimation = new FadeInThemeAnimation();

            if (duration != null)
            {
                fadeInAnimation.Duration = duration.Value;
            }
            
            Storyboard.SetTarget(fadeInAnimation, dob);
            fadeInStoryboard.Children.Add(fadeInAnimation);
            await fadeInStoryboard.BeginAsync();
        }

        /// <summary>
        /// Fades the element out using the FadeOutThemeAnimation.
        /// </summary>
        /// <remarks>
        /// Opacity property of the element is not affected.<br/>
        /// The duration of the visible animation itself is not affected by the duration parameter. It merely indicates how long the Storyboard will run.<br/>
        /// If FadeOutThemeAnimation was already run before and FadeInThemeAnimation was not run after that - nothing will happen.<br/>
        /// </remarks>
        /// <param name="dob"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task FadeOut(this DependencyObject dob, TimeSpan? duration = null)
        {
            var fadeOutStoryboard = new Storyboard();
            var fadeOutAnimation = new FadeOutThemeAnimation();

            if (duration != null)
            {
                fadeOutAnimation.Duration = duration.Value;
            }

            Storyboard.SetTarget(fadeOutAnimation, dob);
            fadeOutStoryboard.Children.Add(fadeOutAnimation);
            await fadeOutStoryboard.BeginAsync();
        }

        /// <summary>
        /// Fades the element in using a custom DoubleAnimation of the Opacity property.
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"> </param>
        /// <returns></returns>
        public static async Task FadeInCustom(this DependencyObject dob, TimeSpan? duration = null, EasingFunctionBase easingFunction = null)
        {
            var fadeInStoryboard = new Storyboard();
            var fadeInAnimation = new DoubleAnimation();

            if (duration == null)
                duration = TimeSpan.FromSeconds(0.4);

            fadeInAnimation.Duration = duration.Value;
            fadeInAnimation.To = 1.0;
            fadeInAnimation.EasingFunction = easingFunction;

            Storyboard.SetTarget(fadeInAnimation, dob);
            Storyboard.SetTargetProperty(fadeInAnimation, "Opacity");
            fadeInStoryboard.Children.Add(fadeInAnimation);
            await fadeInStoryboard.BeginAsync();
        }

        /// <summary>
        /// Fades the element out using a custom DoubleAnimation of the Opacity property.
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"> </param>
        /// <returns></returns>
        public static async Task FadeOutCustom(this DependencyObject dob, TimeSpan? duration = null, EasingFunctionBase easingFunction = null)
        {
            var fadeOutStoryboard = new Storyboard();
            var fadeOutAnimation = new DoubleAnimation();

            if (duration == null)
                duration = TimeSpan.FromSeconds(0.4);

            fadeOutAnimation.Duration = duration.Value;
            fadeOutAnimation.To = 0.0;
            fadeOutAnimation.EasingFunction = easingFunction;

            Storyboard.SetTarget(fadeOutAnimation, dob);
            Storyboard.SetTargetProperty(fadeOutAnimation, "Opacity");
            fadeOutStoryboard.Children.Add(fadeOutAnimation);
            await fadeOutStoryboard.BeginAsync();
        }
    }
}
