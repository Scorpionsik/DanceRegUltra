using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DanceRegUltra.Utilites.Converters
{
    class VisibilityBoolConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool v)
            {
                if(parameter == null || parameter.ToString() != "inverse") return v ? Visibility.Visible : Visibility.Collapsed;
                else return v ? Visibility.Collapsed : Visibility.Visible;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
