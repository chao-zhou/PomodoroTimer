using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Controls.Extensions
{
    public static class ListViewExtensions
    {
        #region BindableSelection
        /// <summary>
        /// BindableSelection Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindableSelectionProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelection",
                typeof(ObservableCollection<object>),
                typeof(ListViewExtensions),
                new PropertyMetadata(null, OnBindableSelectionChanged));

        /// <summary>
        /// Gets the BindableSelection property. This dependency property 
        /// indicates the list of selected items that will be synchronized with the items selected in the ListView.
        /// </summary>
        public static ObservableCollection<object> GetBindableSelection(DependencyObject d)
        {
            return (ObservableCollection<object>)d.GetValue(BindableSelectionProperty);
        }

        /// <summary>
        /// Sets the BindableSelection property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetBindableSelection(DependencyObject d, ObservableCollection<object> value)
        {
            d.SetValue(BindableSelectionProperty, value);
        }

        /// <summary>
        /// Handles changes to the BindableSelection property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnBindableSelectionChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ObservableCollection<object> oldBindableSelection = (ObservableCollection<object>)e.OldValue;
            ObservableCollection<object> newBindableSelection = (ObservableCollection<object>)d.GetValue(BindableSelectionProperty);

            if (oldBindableSelection != null)
            {
                var handler = GetBindableSelectionHandler(d);
                SetBindableSelectionHandler(d, null);
                handler.Detach();
            }

            if (newBindableSelection != null)
            {
                var handler = new ListViewBindableSelectionHandler((ListViewBase)d, newBindableSelection);
                SetBindableSelectionHandler(d, handler);
            }
        }
        #endregion

        #region BindableSelectionHandler
        /// <summary>
        /// BindableSelectionHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindableSelectionHandlerProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectionHandler",
                typeof(ListViewBindableSelectionHandler),
                typeof(ListViewExtensions),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the BindableSelectionHandler property. This dependency property 
        /// indicates BindableSelectionHandler for a ListView - used to manage synchronization of BindableSelection and SelectedItems.
        /// </summary>
        public static ListViewBindableSelectionHandler GetBindableSelectionHandler(DependencyObject d)
        {
            return (ListViewBindableSelectionHandler)d.GetValue(BindableSelectionHandlerProperty);
        }

        /// <summary>
        /// Sets the BindableSelectionHandler property. This dependency property 
        /// indicates BindableSelectionHandler for a ListView - used to manage synchronization of BindableSelection and SelectedItems.
        /// </summary>
        public static void SetBindableSelectionHandler(DependencyObject d, ListViewBindableSelectionHandler value)
        {
            d.SetValue(BindableSelectionHandlerProperty, value);
        }
        #endregion

        public static void ScrollToBottom(this ListView listView)
        {
            var scrollViewer = listView.GetFirstDescendantOfType<ScrollViewer>();
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
        }
    }

    public class ListViewBindableSelectionHandler
    {
        private ListViewBase _listView;
        private ObservableCollection<object> _boundSelection;

        public ListViewBindableSelectionHandler(ListViewBase listView, ObservableCollection<object> boundSelection)
        {
            Attach(listView, boundSelection);
        }

        private void Attach(ListViewBase listView, ObservableCollection<object> boundSelection)
        {
            _listView = listView;
            _listView.Unloaded += OnListViewUnloaded;
            _listView.SelectionChanged += OnListViewSelectionChanged;
            _listView.SelectedItems.Clear();
            _boundSelection = boundSelection;
            _boundSelection.CollectionChanged += OnBoundSelectionChanged;
        }

        private void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                _boundSelection.Remove(item);
            }

            foreach (var item in e.AddedItems)
            {
                _boundSelection.Add(item);
            }
        }

        private void OnBoundSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _listView.SelectedItems.Clear();

                foreach (var item in _boundSelection)
                {
                    _listView.SelectedItems.Add(item);
                }

                return;
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    _listView.SelectedItems.Remove(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    _listView.SelectedItems.Add(item);
                }
            }
        }

        private void OnListViewUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        internal void Detach()
        {
            _listView.Unloaded -= OnListViewUnloaded;
            _listView.SelectionChanged -= OnListViewSelectionChanged;
            _listView = null;
            _boundSelection.CollectionChanged -= OnBoundSelectionChanged;
            _boundSelection = null;
        }
    }
}
