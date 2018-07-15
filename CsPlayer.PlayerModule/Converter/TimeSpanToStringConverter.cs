using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CsPlayer.PlayerModule.Converter
{
    class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = "00:00:00";

            if(value != null && value is TimeSpan timeSpan)
            {
                // Total hours since days are converted as well. Allows for playlists
                // longer than 24h.
                var hours = ((int)timeSpan.TotalHours).ToString().PadLeft(2, '0');
                var minutes = timeSpan.Minutes.ToString().PadLeft(2, '0');
                var seconds = timeSpan.Seconds.ToString().PadLeft(2, '0');

                result = String.Format("{0}:{1}:{2}", hours, minutes, seconds); 
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
