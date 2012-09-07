using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinRTXamlToolkit.AsyncUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinRTXamlToolkit.Controls
{
    [TemplatePart(Name = "PagePresentersPanelName", Type = typeof(Panel))]
    public class AlternativeFrame : ContentControl
    {
        private const string PagePresentersPanelName = "PART_PagePresentersPanel";

        private ContentPresenter _currentPagePresenter;
        private Dictionary<JournalEntry, ContentPresenter> _preloadedPageCache;
        private Panel _pagePresentersPanel;

        public Stack<JournalEntry> BackStack { get; private set; }
        //public Stack<JournalEntry> ForwardStack { get; private set; }

        #region PagePresenterStyle
        /// <summary>
        /// PagePresenterStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty PagePresenterStyleProperty =
            DependencyProperty.Register(
                "PagePresenterStyle",
                typeof(Style),
                typeof(AlternativeFrame),
                new PropertyMetadata(null, OnPagePresenterStyleChanged));

        /// <summary>
        /// Gets or sets the PagePresenterStyle property. This dependency property 
        /// indicates the style of the ContentPresenters used to host the pages.
        /// </summary>
        public Style PagePresenterStyle
        {
            get { return (Style)GetValue(PagePresenterStyleProperty); }
            set { SetValue(PagePresenterStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the PagePresenterStyle property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnPagePresenterStyleChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (AlternativeFrame)d;
            Style oldPagePresenterStyle = (Style)e.OldValue;
            Style newPagePresenterStyle = target.PagePresenterStyle;
            target.OnPagePresenterStyleChanged(oldPagePresenterStyle, newPagePresenterStyle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the PagePresenterStyle property.
        /// </summary>
        /// <param name="oldPagePresenterStyle">The old PagePresenterStyle value</param>
        /// <param name="newPagePresenterStyle">The new PagePresenterStyle value</param>
        protected virtual void OnPagePresenterStyleChanged(
            Style oldPagePresenterStyle, Style newPagePresenterStyle)
        {
        }
        #endregion

        #region PageTransition
        /// <summary>
        /// PageTransition Dependency Property
        /// </summary>
        public static readonly DependencyProperty PageTransitionProperty =
            DependencyProperty.Register(
                "PageTransition",
                typeof(PageTransition),
                typeof(AlternativeFrame),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the PageTransition property. This dependency property 
        /// indicates the PageTransition to use to transition between pages.
        /// </summary>
        public PageTransition PageTransition
        {
            get { return (PageTransition)GetValue(PageTransitionProperty); }
            set { SetValue(PageTransitionProperty, value); }
        }
        #endregion

        #region CanGoBack
        /// <summary>
        /// CanGoBack Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register(
                "CanGoBack",
                typeof(bool),
                typeof(AlternativeFrame),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the CanGoBack property. This dependency property 
        /// indicates whether you can go back in the navigation history.
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            private set { SetValue(CanGoBackProperty, value); }
        }
        #endregion

        #region CanNavigate
        /// <summary>
        /// CanNavigate Dependency Property
        /// </summary>
        public static readonly DependencyProperty CanNavigateProperty =
            DependencyProperty.Register(
                "CanNavigate",
                typeof(bool),
                typeof(AlternativeFrame),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the CanNavigate property. This dependency property 
        /// indicates whether the frame is in a state where a navigation call will be accepted.
        /// </summary>
        public bool CanNavigate
        {
            get { return (bool)GetValue(CanNavigateProperty); }
            private set { SetValue(CanNavigateProperty, value); }
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
                typeof(AlternativeFrame),
                new PropertyMetadata(true));

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

        public AlternativeFrame()
        {
            this.BackStack = new Stack<JournalEntry>();
            //this.ForwardStack = new Stack<JournalEntry>();
            _preloadedPageCache = new Dictionary<JournalEntry, ContentPresenter>();
            this.DefaultStyleKey = typeof (AlternativeFrame);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _pagePresentersPanel = (Panel)GetTemplateChild(PagePresentersPanelName);
        }

        // Allows to preload a page before user navigates to it, so if it does get navigated to - it is quick.
        public async Task Preload(Type sourcePageType, object parameter)
        {
            var je = new JournalEntry {Type = sourcePageType, Parameter = parameter};

            if (_preloadedPageCache.ContainsKey(je))
            {
                return;
            }

            var cp = new ContentPresenter {Style = PagePresenterStyle};
            var newPage = (AlternativePage)Activator.CreateInstance(sourcePageType);
            newPage.Frame = this;
            cp.Content = newPage;
            cp.Opacity = 0.005;
            Canvas.SetZIndex(cp, int.MinValue);
            _pagePresentersPanel.Children.Insert(0, cp);
            _preloadedPageCache.Add(je, cp);
            await newPage.PreloadInternal(parameter);
        }

        public async Task UnloadPreloaded(Type sourcePageType, object parameter)
        {
            var je = new JournalEntry { Type = sourcePageType, Parameter = parameter };

            if (!_preloadedPageCache.ContainsKey(je))
            {
                return;
            }

            var cp = _preloadedPageCache[je];
            var page = (AlternativePage)cp.Content;
            await page.UnloadPreloadedInternal();

            _pagePresentersPanel.Children.Remove(cp);
            _preloadedPageCache.Remove(je);
        }

        public async Task<bool> Navigate(Type sourcePageType)
        {
            return await Navigate(sourcePageType, null);
        }

        private bool _isNavigating;

        public async Task<bool> Navigate(Type sourcePageType, object parameter)
        {
            if (_isNavigating)
            {
                throw new InvalidOperationException("Navigation already in progress.");
            }

            _isNavigating = true;
            this.CanNavigate = false;
            this.CanGoBack = false;

            try
            {
                await this.WaitForLoadedAsync();
                AlternativePage currentPage = null;

                if (_currentPagePresenter != null)
                {
                    currentPage = (AlternativePage)_currentPagePresenter.Content;
                    var cancelArgs =
                        new AlternativeNavigatingCancelEventArgs(
                            NavigationMode.New,
                            sourcePageType);
                    await currentPage.OnNavigatingFromInternal(cancelArgs);

                    if (cancelArgs.Cancel)
                    {
                        return false;
                    }
                }

                var je = new JournalEntry {Type = sourcePageType, Parameter = parameter};
                AlternativePage newPage;
                ContentPresenter newPagePresenter;

                if (_preloadedPageCache.ContainsKey(je))
                {
                    newPagePresenter = _preloadedPageCache[je];
                    newPage = (AlternativePage)newPagePresenter.Content;
                    _preloadedPageCache.Remove(je);
                }
                else
                {
                    newPage = (AlternativePage)Activator.CreateInstance(je.Type);
                    newPage.Frame = this;
                    newPagePresenter = new ContentPresenter {Style = PagePresenterStyle};
                    newPagePresenter.Content = newPage;
                    _pagePresentersPanel.Children.Add(newPagePresenter);
                }

                newPagePresenter.Opacity = 0.005;

                // TODO: await?
                UnloadPreloadedPages();

                var args = new AlternativeNavigationEventArgs(
                    newPage.Content, NavigationMode.New, je.Parameter, je.Type);
                await newPage.OnNavigatingToInternal(args);

                await newPagePresenter.WaitForLoadedAsync();
                await newPagePresenter.WaitForNonZeroSizeAsync();

                if (this.ShouldWaitForImagesToLoad == true && newPage.ShouldWaitForImagesToLoad != false ||
                    newPage.ShouldWaitForImagesToLoad == true && this.ShouldWaitForImagesToLoad != false)
                {
                    await newPage.WaitForImagesToLoad();
                }

                newPagePresenter.Opacity = 1.0;

                if (this.PageTransition != null)
                {
                    await this.PageTransition.TransitionForward(_currentPagePresenter, newPagePresenter);
                }

                this.BackStack.Push(je);

                if (currentPage != null)
                {
                    await currentPage.OnNavigatedFromInternal(args);
                    _pagePresentersPanel.Children.Remove(_currentPagePresenter);
                }

                _currentPagePresenter = newPagePresenter;

                await newPage.OnNavigatedToInternal(args);

                return true;
            }
            finally
            {
                _isNavigating = false;
                this.CanNavigate = true;
                this.CanGoBack = this.BackStack.Count > 1;
            }
        }

        private async Task UnloadPreloadedPages()
        {
            foreach (var kvp in _preloadedPageCache)
            {
                _pagePresentersPanel.Children.Remove(kvp.Value);
                var page = (AlternativePage)kvp.Value.Content;
                await page.UnloadPreloadedInternal();
            }

            _preloadedPageCache.Clear();
        }

        public async Task<bool> GoBack()
        {
            if (_isNavigating)
            {
                throw new InvalidOperationException("Navigation already in progress.");
            }

            _isNavigating = true;
            this.CanNavigate = false;
            this.CanGoBack = false;

            try
            {
                await this.WaitForLoadedAsync();
                AlternativePage currentPage = null;
                var currentJe = this.BackStack.Pop();
                var je = BackStack.Peek();

                if (_currentPagePresenter != null)
                {
                    currentPage = (AlternativePage)_currentPagePresenter.Content;
                    var cancelArgs =
                        new AlternativeNavigatingCancelEventArgs(
                            NavigationMode.Back,
                            je.Type);
                    await currentPage.OnNavigatingFromInternal(cancelArgs);

                    if (cancelArgs.Cancel)
                    {
                        this.BackStack.Push(currentJe);
                        return false;
                    }
                }

                AlternativePage newPage;
                ContentPresenter newPagePresenter;

                if (_preloadedPageCache.ContainsKey(je))
                {
                    newPagePresenter = _preloadedPageCache[je];
                    newPage = (AlternativePage)newPagePresenter.Content;
                    _preloadedPageCache.Remove(je);
                }
                else
                {
                    newPage = (AlternativePage)Activator.CreateInstance(je.Type);
                    newPage.Frame = this;
                    newPagePresenter = new ContentPresenter { Style = PagePresenterStyle };
                    newPagePresenter.Content = newPage;
                    _pagePresentersPanel.Children.Add(newPagePresenter);
                }

                newPagePresenter.Opacity = 0.005;

                await UnloadPreloadedPages();

                var args = new AlternativeNavigationEventArgs(
                    newPage.Content, NavigationMode.New, je.Parameter, je.Type);
                await newPage.OnNavigatingToInternal(args);

                await newPagePresenter.WaitForLoadedAsync();
                await newPagePresenter.WaitForNonZeroSizeAsync();
                newPagePresenter.Opacity = 1.0;

                if (this.PageTransition != null)
                {
                    await this.PageTransition.TransitionBackward(_currentPagePresenter, newPagePresenter);
                }

                if (currentPage != null)
                {
                    await currentPage.OnNavigatedFromInternal(args);
                    _pagePresentersPanel.Children.Remove(_currentPagePresenter);
                }

                _currentPagePresenter = newPagePresenter;

                await newPage.OnNavigatedToInternal(args);

                return true;
            }
            finally
            {
                _isNavigating = false;
                this.CanNavigate = true;
                this.CanGoBack = this.BackStack.Count > 1;
            }
        }
    }

    public class JournalEntry
    {
        public Type Type { get; internal set; }
        public object Parameter { get; internal set; }

        public override bool Equals(object obj)
        {
            var je = obj as JournalEntry;

            if (je == null)
            {
                return false;
            }

            bool ret = 
                this.Type.Equals(je.Type) &&
                ((this.Parameter == null && je.Parameter == null) ||
                 (this.Parameter.Equals(je.Parameter)));

            return ret;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            if (this.Parameter != null)
            {
                hash = hash * 23 + this.Parameter.GetHashCode();
            }
            else
            {
                hash = hash * 23;
            }

            hash = hash * 23 + this.Type.GetHashCode();

            return hash;
        }
    }
}
