using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroTimer.Convertor
{
    public class SecondConvertor : Windows.UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Value cannot be null.");

            if (!typeof(int).Equals(value.GetType()))
                throw new ArgumentException("Value must be of type int.", "value");

            var totalSeconds = (int)value;
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;

            return string.Format("{0:00}:{1:00}", minutes, seconds);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var strVal = value as string;
            var vals = strVal.Split(':');

            var minutes = int.Parse(vals[0]);
            var seconds = int.Parse(vals[1]);
            
            return minutes * 60 + seconds;
        }
    }
}
