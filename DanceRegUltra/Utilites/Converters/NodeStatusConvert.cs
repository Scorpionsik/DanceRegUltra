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
    public class NodeStatusConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeStatus v)
            {
                if (parameter == null || parameter.ToString() == "bool") return v == NodeStatus.Default ? false : true;
                else if (parameter.ToString() == "string")
                {
                    string result = "";
                    switch (v)
                    {
                        case NodeStatus.GetScores:
                            result = "Оценен";
                            break;
                        case NodeStatus.Print:
                            result = "Напечатан";
                            break;
                    }
                    return result;
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
