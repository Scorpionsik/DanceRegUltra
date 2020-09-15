using DanceRegUltra.Enums;
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
    public class JudgeTypeConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JudgeType type)
            {
                switch (type)
                {
                    case JudgeType.ThreeD:
                        return "3-D";
                    case JudgeType.FourD:
                        return "4-D";
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
