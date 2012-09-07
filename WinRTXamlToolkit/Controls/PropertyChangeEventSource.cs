using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WinRTXamlToolkit.Controls
{
    /// <summary>
    /// Allows raise an event when the value of a dependency property changes when a view model is otherwise not necessary.
    /// </summary>
    /// <typeparam name="TPropertyType"></typeparam>
    public class PropertyChangeEventSource<TPropertyType>
        : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<TPropertyType> ValueChanged;

        private FrameworkElement _source;

        private TPropertyType _value;
        public TPropertyType Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));

                if (ValueChanged != null)
                {
                    ValueChanged(_source, _value);
                }
            }
        }

        public PropertyChangeEventSource(
            FrameworkElement source, DependencyProperty property)
        {
            _source = source;

            // Bind to the property to be able to get its changes relayed as events throug the ValueChanged event.
            source.SetBinding(
                property,
                new Binding
                {
                    Source = this,
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.TwoWay
                });
        }
    }
}
