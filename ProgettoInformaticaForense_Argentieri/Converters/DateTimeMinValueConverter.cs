using ProgettoInformaticaForense_Argentieri.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ProgettoInformaticaForense_Argentieri.Converters
{
    public class DateTimeMinValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var strDate = (string)value;
            var minDateTime = DateTime.MinValue.ToString();

            return strDate.Equals(minDateTime) || string.IsNullOrEmpty(strDate) ? string.Empty : DateBuilder.BuildFromString(strDate);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
