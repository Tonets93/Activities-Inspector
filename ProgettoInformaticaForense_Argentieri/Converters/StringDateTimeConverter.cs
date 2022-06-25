using ProgettoInformaticaForense_Argentieri.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ProgettoInformaticaForense_Argentieri.Converters
{
    public class StringDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var strDate = (string)value;

            return DateBuilder.BuildFromString(strDate);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
