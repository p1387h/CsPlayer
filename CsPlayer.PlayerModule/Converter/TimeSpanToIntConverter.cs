using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CsPlayer.PlayerModule.Converter
{
    class TimeSpanToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = 0;

            if (value != null && value is TimeSpan timeSpan)
            {
                result = (int)timeSpan.TotalSeconds;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new TimeSpan();

            if(value != null && value is double seconds)
            {
                result = TimeSpan.FromSeconds(seconds);
            }

            return result;
        }
    }
}
