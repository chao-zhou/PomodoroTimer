using System.Threading.Tasks;
using WinRTXamlToolkit.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.AsyncUI;

namespace WinRTXamlToolkit.Controls
{
    [TemplatePart(Name = NormalStateImageName, Type = typeof(Image))]
    [TemplatePart(Name = HoverStateImageName, Type = typeof(Image))]
    [TemplatePart(Name = HoverStateRecycledNormalStateImageName, Type = typeof(Image))]
    [TemplatePart(Name = HoverStateRecycledPressedStateImageName, Type = typeof(Image))]
    [TemplatePart(Name = PressedStateImageName, Type = typeof(Image))]
    [TemplatePart(Name = DisabledStateImageName, Type = typeof(Image))]
    public class ImageButton : Button
    {
        private const string NormalStateImageName = "PART_NormalStateImage";
        private const string HoverStateImageName = "PART_HoverStateImage";
        private const string HoverStateRecycledNormalStateImageName = "PART_HoverStateRecycledNormalStateImage";
        private const string HoverStateRecycledPressedStateImageName = "PART_HoverStateRecycledPressedStateImage";
        private const string PressedStateImageName = "PART_PressedStateImage";
        private const string DisabledStateImageName = "PART_DisabledStateImage";
        private Image _normalStateImage;
        private Image _hoverStateImage;
        private Image _hoverStateRecycledNormalStateImage;
        private Image _hoverStateRecycledPressedStateImage;
        private Image _pressedStateImage;
        private Image _disabledStateImage;

        #region RecyclePressedStateImageForHover
        /// <summary>
        /// RecyclePressedStateImageForHover Dependency Property
        /// </summary>
        public static readonly DependencyProperty RecyclePressedStateImageForHoverProperty =
            DependencyProperty.Register(
                "RecyclePressedStateImageForHover",
                typeof(bool),
                typeof(ImageButton),
                new PropertyMetadata(false, OnRecyclePressedStateImageForHoverChanged));

        /// <summary>
        /// Gets or sets the RecyclePressedStateImageForHover property. This dependency property 
        /// indicates whether the PressedStateImageSource should also be used for hover state with 0.5 opacity.
        /// </summary>
        public bool RecyclePressedStateImageForHover
        {
            get { return (bool)GetValue(RecyclePressedStateImageForHoverProperty); }
            set { SetValue(RecyclePressedStateImageForHoverProperty, value); }
        }

        /// <summary>
        /// Handles changes to the RecyclePressedStateImageForHover property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnRecyclePressedStateImageForHoverChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            bool oldRecyclePressedStateImageForHover = (bool)e.OldValue;
            bool newRecyclePressedStateImageForHover = target.RecyclePressedStateImageForHover;
            target.OnRecyclePressedStateImageForHoverChanged(oldRecyclePressedStateImageForHover, newRecyclePressedStateImageForHover);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the RecyclePressedStateImageForHover property.
        /// </summary>
        /// <param name="oldRecyclePressedStateImageForHover">The old RecyclePressedStateImageForHover value</param>
        /// <param name="newRecyclePressedStateImageForHover">The new RecyclePressedStateImageForHover value</param>
        protected virtual void OnRecyclePressedStateImageForHoverChanged(
            bool oldRecyclePressedStateImageForHover, bool newRecyclePressedStateImageForHover)
        {
            UpdateRecycledHoverStateImages();
        }
        #endregion

        #region NormalStateImageSource
        /// <summary>
        /// NormalStateImageSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty NormalStateImageSourceProperty =
            DependencyProperty.Register(
                "NormalStateImageSource",
                typeof(ImageSource),
                typeof(ImageButton),
                new PropertyMetadata(null, OnNormalStateImageSourceChanged));

        /// <summary>
        /// Gets or sets the NormalStateImageSource property. This dependency property 
        /// indicates the ImageSource for the normal state.
        /// </summary>
        public ImageSource NormalStateImageSource
        {
            get { return (ImageSource)GetValue(NormalStateImageSourceProperty); }
            set { SetValue(NormalStateImageSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the NormalStateImageSource property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnNormalStateImageSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            ImageSource oldNormalStateImageSource = (ImageSource)e.OldValue;
            ImageSource newNormalStateImageSource = target.NormalStateImageSource;
            target.OnNormalStateImageSourceChanged(oldNormalStateImageSource, newNormalStateImageSource);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the NormalStateImageSource property.
        /// </summary>
        /// <param name="oldNormalStateImageSource">The old NormalStateImageSource value</param>
        /// <param name="newNormalStateImageSource">The new NormalStateImageSource value</param>
        protected virtual void OnNormalStateImageSourceChanged(
            ImageSource oldNormalStateImageSource, ImageSource newNormalStateImageSource)
        {
            UpdateNormalStateImage();
            UpdateHoverStateImage();
            UpdateRecycledHoverStateImages();
            UpdatePressedStateImage();
            UpdateDisabledStateImage();
        }
        #endregion

        #region HoverStateImageSource
        /// <summary>
        /// HoverStateImageSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty HoverStateImageSourceProperty =
            DependencyProperty.Register(
                "HoverStateImageSource",
                typeof(ImageSource),
                typeof(ImageButton),
                new PropertyMetadata(null, OnHoverStateImageSourceChanged));

        /// <summary>
        /// Gets or sets the HoverStateImageSource property. This dependency property 
        /// indicates the ImageSource to use when the pointer is over the button.
        /// </summary>
        public ImageSource HoverStateImageSource
        {
            get { return (ImageSource)GetValue(HoverStateImageSourceProperty); }
            set { SetValue(HoverStateImageSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the HoverStateImageSource property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnHoverStateImageSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            ImageSource oldHoverStateImageSource = (ImageSource)e.OldValue;
            ImageSource newHoverStateImageSource = target.HoverStateImageSource;
            target.OnHoverStateImageSourceChanged(oldHoverStateImageSource, newHoverStateImageSource);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the HoverStateImageSource property.
        /// </summary>
        /// <param name="oldHoverStateImageSource">The old HoverStateImageSource value</param>
        /// <param name="newHoverStateImageSource">The new HoverStateImageSource value</param>
        protected virtual void OnHoverStateImageSourceChanged(
            ImageSource oldHoverStateImageSource, ImageSource newHoverStateImageSource)
        {
            UpdateHoverStateImage();
        }
        #endregion

        #region PressedStateImageSource
        /// <summary>
        /// PressedStateImageSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty PressedStateImageSourceProperty =
            DependencyProperty.Register(
                "PressedStateImageSource",
                typeof(ImageSource),
                typeof(ImageButton),
                new PropertyMetadata(null, OnPressedStateImageSourceChanged));

        /// <summary>
        /// Gets or sets the PressedStateImageSource property. This dependency property 
        /// indicates the ImageSource to use when the button is pressed.
        /// </summary>
        public ImageSource PressedStateImageSource
        {
            get { return (ImageSource)GetValue(PressedStateImageSourceProperty); }
            set { SetValue(PressedStateImageSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the PressedStateImageSource property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnPressedStateImageSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            ImageSource oldPressedStateImageSource = (ImageSource)e.OldValue;
            ImageSource newPressedStateImageSource = target.PressedStateImageSource;
            target.OnPressedStateImageSourceChanged(oldPressedStateImageSource, newPressedStateImageSource);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the PressedStateImageSource property.
        /// </summary>
        /// <param name="oldPressedStateImageSource">The old PressedStateImageSource value</param>
        /// <param name="newPressedStateImageSource">The new PressedStateImageSource value</param>
        protected virtual void OnPressedStateImageSourceChanged(
            ImageSource oldPressedStateImageSource, ImageSource newPressedStateImageSource)
        {
            UpdatePressedStateImage();
            UpdateRecycledHoverStateImages();
        }
        #endregion

        #region DisabledStateImageSource
        /// <summary>
        /// DisabledStateImageSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty DisabledStateImageSourceProperty =
            DependencyProperty.Register(
                "DisabledStateImageSource",
                typeof(ImageSource),
                typeof(ImageButton),
                new PropertyMetadata(null, OnDisabledStateImageSourceChanged));

        /// <summary>
        /// Gets or sets the DisabledStateImageSource property. This dependency property 
        /// indicates the ImageSource to use when the button is Disabled.
        /// </summary>
        public ImageSource DisabledStateImageSource
        {
            get { return (ImageSource)GetValue(DisabledStateImageSourceProperty); }
            set { SetValue(DisabledStateImageSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DisabledStateImageSource property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnDisabledStateImageSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            ImageSource oldDisabledStateImageSource = (ImageSource)e.OldValue;
            ImageSource newDisabledStateImageSource = target.DisabledStateImageSource;
            target.OnDisabledStateImageSourceChanged(oldDisabledStateImageSource, newDisabledStateImageSource);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the DisabledStateImageSource property.
        /// </summary>
        /// <param name="oldDisabledStateImageSource">The old DisabledStateImageSource value</param>
        /// <param name="newDisabledStateImageSource">The new DisabledStateImageSource value</param>
        protected virtual void OnDisabledStateImageSourceChanged(
            ImageSource oldDisabledStateImageSource, ImageSource newDisabledStateImageSource)
        {
            if (_disabledStateImage != null)
                _disabledStateImage.Source = newDisabledStateImageSource;
        }
        #endregion

        #region GenerateMissingImages
        /// <summary>
        /// GenerateMissingImages Dependency Property
        /// </summary>
        public static readonly DependencyProperty GenerateMissingImagesProperty =
            DependencyProperty.Register(
                "GenerateMissingImages",
                typeof(bool),
                typeof(ImageButton),
                new PropertyMetadata(false, OnGenerateMissingImagesChanged));

        /// <summary>
        /// Gets or sets the GenerateMissingImages property. This dependency property 
        /// indicates whether the missing images should be generated from the normal state image.
        /// </summary>
        public bool GenerateMissingImages
        {
            get { return (bool)GetValue(GenerateMissingImagesProperty); }
            set { SetValue(GenerateMissingImagesProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GenerateMissingImages property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnGenerateMissingImagesChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            bool oldGenerateMissingImages = (bool)e.OldValue;
            bool newGenerateMissingImages = target.GenerateMissingImages;
            target.OnGenerateMissingImagesChanged(oldGenerateMissingImages, newGenerateMissingImages);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the GenerateMissingImages property.
        /// </summary>
        /// <param name="oldGenerateMissingImages">The old GenerateMissingImages value</param>
        /// <param name="newGenerateMissingImages">The new GenerateMissingImages value</param>
        protected virtual void OnGenerateMissingImagesChanged(
            bool oldGenerateMissingImages, bool newGenerateMissingImages)
        {
            UpdateHoverStateImage();
            UpdatePressedStateImage();
            UpdateDisabledStateImage();
        }
        #endregion

        #region GeneratedHoverStateLightenAmount
        /// <summary>
        /// GeneratedHoverStateLightenAmount Dependency Property
        /// </summary>
        public static readonly DependencyProperty GeneratedHoverStateLightenAmountProperty =
            DependencyProperty.Register(
                "GeneratedHoverStateLightenAmount",
                typeof(double),
                typeof(ImageButton),
                new PropertyMetadata(0.25, OnGeneratedHoverStateLightenAmountChanged));

        /// <summary>
        /// Gets or sets the GeneratedHoverStateLightenAmount property. This dependency property 
        /// indicates the lightening amount to use when generating the hover state image.
        /// </summary>
        public double GeneratedHoverStateLightenAmount
        {
            get { return (double)GetValue(GeneratedHoverStateLightenAmountProperty); }
            set { SetValue(GeneratedHoverStateLightenAmountProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GeneratedHoverStateLightenAmount property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnGeneratedHoverStateLightenAmountChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            double oldGeneratedHoverStateLightenAmount = (double)e.OldValue;
            double newGeneratedHoverStateLightenAmount = target.GeneratedHoverStateLightenAmount;
            target.OnGeneratedHoverStateLightenAmountChanged(oldGeneratedHoverStateLightenAmount, newGeneratedHoverStateLightenAmount);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the GeneratedHoverStateLightenAmount property.
        /// </summary>
        /// <param name="oldGeneratedHoverStateLightenAmount">The old GeneratedHoverStateLightenAmount value</param>
        /// <param name="newGeneratedHoverStateLightenAmount">The new GeneratedHoverStateLightenAmount value</param>
        protected virtual void OnGeneratedHoverStateLightenAmountChanged(
            double oldGeneratedHoverStateLightenAmount, double newGeneratedHoverStateLightenAmount)
        {
            UpdateHoverStateImage();
        }
        #endregion

        #region GeneratedPressedStateLightenAmount
        /// <summary>
        /// GeneratedPressedStateLightenAmount Dependency Property
        /// </summary>
        public static readonly DependencyProperty GeneratedPressedStateLightenAmountProperty =
            DependencyProperty.Register(
                "GeneratedPressedStateLightenAmount",
                typeof(double),
                typeof(ImageButton),
                new PropertyMetadata(0.5, OnGeneratedPressedStateLightenAmountChanged));

        /// <summary>
        /// Gets or sets the GeneratedPressedStateLightenAmount property. This dependency property 
        /// indicates the lightening amount to use when generating the pressed state image.
        /// </summary>
        public double GeneratedPressedStateLightenAmount
        {
            get { return (double)GetValue(GeneratedPressedStateLightenAmountProperty); }
            set { SetValue(GeneratedPressedStateLightenAmountProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GeneratedPressedStateLightenAmount property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnGeneratedPressedStateLightenAmountChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            double oldGeneratedPressedStateLightenAmount = (double)e.OldValue;
            double newGeneratedPressedStateLightenAmount = target.GeneratedPressedStateLightenAmount;
            target.OnGeneratedPressedStateLightenAmountChanged(oldGeneratedPressedStateLightenAmount, newGeneratedPressedStateLightenAmount);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the GeneratedPressedStateLightenAmount property.
        /// </summary>
        /// <param name="oldGeneratedPressedStateLightenAmount">The old GeneratedPressedStateLightenAmount value</param>
        /// <param name="newGeneratedPressedStateLightenAmount">The new GeneratedPressedStateLightenAmount value</param>
        protected virtual void OnGeneratedPressedStateLightenAmountChanged(
            double oldGeneratedPressedStateLightenAmount, double newGeneratedPressedStateLightenAmount)
        {
            UpdatePressedStateImage();
        }
        #endregion

        #region GeneratedDisabledStateGrayscaleAmount
        /// <summary>
        /// GeneratedDisabledStateGrayscaleAmount Dependency Property
        /// </summary>
        public static readonly DependencyProperty GeneratedDisabledStateGrayscaleAmountProperty =
            DependencyProperty.Register(
                "GeneratedDisabledStateGrayscaleAmount",
                typeof(double),
                typeof(ImageButton),
                new PropertyMetadata(1.0, OnGeneratedDisabledStateGrayscaleAmountChanged));

        /// <summary>
        /// Gets or sets the GeneratedDisabledStateGrayscaleAmount property. This dependency property 
        /// indicates the grayscale amount to use when generating the disabled state image.
        /// </summary>
        public double GeneratedDisabledStateGrayscaleAmount
        {
            get { return (double)GetValue(GeneratedDisabledStateGrayscaleAmountProperty); }
            set { SetValue(GeneratedDisabledStateGrayscaleAmountProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GeneratedDisabledStateGrayscaleAmount property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnGeneratedDisabledStateGrayscaleAmountChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageButton)d;
            double oldGeneratedDisabledStateGrayscaleAmount = (double)e.OldValue;
            double newGeneratedDisabledStateGrayscaleAmount = target.GeneratedDisabledStateGrayscaleAmount;
            target.OnGeneratedDisabledStateGrayscaleAmountChanged(oldGeneratedDisabledStateGrayscaleAmount, newGeneratedDisabledStateGrayscaleAmount);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the GeneratedDisabledStateGrayscaleAmount property.
        /// </summary>
        /// <param name="oldGeneratedDisabledStateGrayscaleAmount">The old GeneratedDisabledStateGrayscaleAmount value</param>
        /// <param name="newGeneratedDisabledStateGrayscaleAmount">The new GeneratedDisabledStateGrayscaleAmount value</param>
        protected virtual void OnGeneratedDisabledStateGrayscaleAmountChanged(
            double oldGeneratedDisabledStateGrayscaleAmount, double newGeneratedDisabledStateGrayscaleAmount)
        {
            UpdateDisabledStateImage();
        }
        #endregion

        #region GenerateHoverStateImage()
        private async void GenerateHoverStateImage()
        {
            var wb = new WriteableBitmap(1, 1);
            await wb.FromBitmapImage((BitmapImage)NormalStateImageSource);
            await wb.WaitForLoaded();
            wb.Lighten(GeneratedHoverStateLightenAmount);
            _hoverStateImage.Source = wb;
        } 
        #endregion

        #region GeneratePressedStateImage()
        private async void GeneratePressedStateImage()
        {
            var wb = new WriteableBitmap(1, 1);
            await wb.FromBitmapImage((BitmapImage)NormalStateImageSource);
            await wb.WaitForLoaded();
            wb.Lighten(GeneratedPressedStateLightenAmount);
            _pressedStateImage.Source = wb;
        } 
        #endregion

        #region GenerateDisabledStateImage()
        private async void GenerateDisabledStateImage()
        {
            var wb = new WriteableBitmap(1, 1);
            await wb.FromBitmapImage((BitmapImage)NormalStateImageSource);
            await wb.WaitForLoaded();
            wb.Grayscale(GeneratedDisabledStateGrayscaleAmount);
            _disabledStateImage.Source = wb;
        } 
        #endregion

        public ImageButton()
        {
            DefaultStyleKey = typeof (ImageButton);
        }

        #region OnApplyTemplate()
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _normalStateImage = GetTemplateChild(NormalStateImageName) as Image;
            _hoverStateImage = GetTemplateChild(HoverStateImageName) as Image;
            _hoverStateRecycledNormalStateImage = GetTemplateChild(HoverStateRecycledNormalStateImageName) as Image;
            _hoverStateRecycledPressedStateImage = GetTemplateChild(HoverStateRecycledPressedStateImageName) as Image;
            _pressedStateImage = GetTemplateChild(PressedStateImageName) as Image;
            _disabledStateImage = GetTemplateChild(DisabledStateImageName) as Image;

            UpdateNormalStateImage();
            UpdateHoverStateImage();
            UpdateRecycledHoverStateImages();
            UpdatePressedStateImage();
            UpdateDisabledStateImage();
        }
        #endregion

        #region UpdateNormalStateImage()
        private void UpdateNormalStateImage()
        {
            if (_normalStateImage != null)
                _normalStateImage.Source = NormalStateImageSource;
        }
        #endregion

        #region UpdateHoverStateImage()
        private void UpdateHoverStateImage()
        {
            if (_hoverStateImage == null)
                return;

            if (HoverStateImageSource != null)
            {
                _hoverStateImage.Source = HoverStateImageSource;
            }
            else if (
                GenerateMissingImages &&
                NormalStateImageSource != null)
            {
#pragma warning disable 4014
                GenerateHoverStateImage();
#pragma warning restore 4014
            }

            // If hover state is still not set - need to use normal state at least to avoid missing image
            if (_hoverStateImage.Source == null)
                _hoverStateImage.Source = NormalStateImageSource;
        } 
        #endregion

        #region UpdateRecycledHoverStateImages()
        private void UpdateRecycledHoverStateImages()
        {
            if (_hoverStateRecycledNormalStateImage != null)
            {
                if (RecyclePressedStateImageForHover &&
                    NormalStateImageSource != null)
                    _hoverStateRecycledNormalStateImage.Source = NormalStateImageSource;
                else
                    _hoverStateRecycledNormalStateImage.Source = null;
            }

            if (_hoverStateRecycledPressedStateImage != null)
            {
                if (RecyclePressedStateImageForHover &&
                    PressedStateImageSource != null)
                    _hoverStateRecycledPressedStateImage.Source = PressedStateImageSource;
                else
                    _hoverStateRecycledPressedStateImage.Source = null;
            }
        } 
        #endregion

        #region UpdatePressedStateImage()
        private void UpdatePressedStateImage()
        {
            if (_pressedStateImage != null)
            {
                if (PressedStateImageSource != null)
                {
                    _pressedStateImage.Source = PressedStateImageSource;
                }
                else if (
                    GenerateMissingImages &&
                    NormalStateImageSource != null)
                {
#pragma warning disable 4014
                    GeneratePressedStateImage();
#pragma warning restore 4014
                }
            }
        } 
        #endregion

        #region UpdateDisabledStateImage()
        private void UpdateDisabledStateImage()
        {
            if (_disabledStateImage != null)
            {
                if (DisabledStateImageSource != null)
                {
                    _disabledStateImage.Source = DisabledStateImageSource;
                }
                else if (
                    GenerateMissingImages &&
                    NormalStateImageSource != null)
                {
#pragma warning disable 4014
                    GenerateDisabledStateImage();
#pragma warning restore 4014
                }
            }
        } 
        #endregion
    }
}
