using ProgettoInformaticaForense_Argentieri.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ProgettoInformaticaForense_Argentieri.Converters
{
    public class ViewerModeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = (ViewerMode)value;

            switch (name)
            {
                case ViewerMode.InstalledPrograms:
                    return Properties.Resources.MainWindow_ViewerMode_InstalledPrograms;
                case ViewerMode.Prefetch:
                    return Properties.Resources.MainWindow_ViewerMode_Prefetch;
                case ViewerMode.Recents:
                    return Properties.Resources.MainWindow_ViewerMode_Recents;
                case ViewerMode.ShellBags:
                    return Properties.Resources.MainWindow_ViewerMode_ShellBags;
                case ViewerMode.TimeIntervals:
                    return Properties.Resources.MainWindow_ViewerMode_TimeIntervals;
                case ViewerMode.Sessions:
                    return Properties.Resources.MainWindow_ViewerMode_Sessions;
                case ViewerMode.SystemTimeChanged:
                    return Properties.Resources.MainWindow_ViewerMode_SystemTimeChanged;
                case ViewerMode.Usb:
                    return Properties.Resources.MainWindow_ViewerMode_Usb;

                default: throw new ArgumentException($"Valore {nameof(name)} non previsto");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
