using CoreWPF.Utilites;
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
    /// <summary>
    /// Класс-конвертер, конвертирует Unix Timestamp в строкое представление времени (часовой пояс берёт из <see cref="App.Timezone"/>.).
    /// </summary>
    public class DancerTimeConvert : IValueConverter
    {
        /// <summary>
        /// Конвертация Unix Timestamp в строковое представление.
        /// </summary>
        /// <param name="value">Unix Timestamp в формате <see cref="double"/>.</param>
        /// <param name="targetType">Не используется в текущем методе.</param>
        /// <param name="parameter">Не используется в текущем методе.</param>
        /// <param name="culture">Не используется в текущем методе.</param>
        /// <returns>Возвращает строковое представление даты; если в <paramref name="value"/> передан не <see cref="double"/>, вернет пустую строку.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                if (d > 0)
                {
                    DateTimeOffset tmp = UnixTime.ToDateTimeOffset(d, App.Locality);
                    string m = "", w = "";
                    switch (tmp.Month)
                    {
                        case 1:
                            m = "января";
                            break;
                        case 2:
                            m = "февраля";
                            break;
                        case 3:
                            m = "марта";
                            break;
                        case 4:
                            m = "апреля";
                            break;
                        case 5:
                            m = "мая";
                            break;
                        case 6:
                            m = "июня";
                            break;
                        case 7:
                            m = "июля";
                            break;
                        case 8:
                            m = "августа";
                            break;
                        case 9:
                            m = "сентября";
                            break;
                        case 10:
                            m = "октября";
                            break;
                        case 11:
                            m = "ноября";
                            break;
                        case 12:
                            m = "декабря";
                            break;
                    }

                    switch (tmp.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            w = "понедельник";
                            break;
                        case DayOfWeek.Tuesday:
                            w = "вторник";
                            break;
                        case DayOfWeek.Wednesday:
                            w = "среда";
                            break;
                        case DayOfWeek.Thursday:
                            w = "четверг";
                            break;
                        case DayOfWeek.Friday:
                            w = "пятница";
                            break;
                        case DayOfWeek.Saturday:
                            w = "суббота";
                            break;
                        case DayOfWeek.Sunday:
                            w = "воскресенье";
                            break;
                    }
                    string minute = tmp.Minute.ToString();
                    if (minute.Length < 2) minute = "0" + minute;

                    return tmp.Day + " " + m + " " + tmp.Year + " (" + w + "), " + tmp.Hour + ":" + minute;
                }
            }
            return "";
        } //---метод Convert

        /// <summary>
        /// Не используется в текущем классе, необходим для реализации <see cref="IValueConverter"/>.
        /// </summary>
        /// <param name="value">Не используется в текущем методе.</param>
        /// <param name="targetType">Не используется в текущем методе.</param>
        /// <param name="parameter">Не используется в текущем методе.</param>
        /// <param name="culture">Не используется в текущем методе.</param>
        /// <returns>Вернет <see cref="DependencyProperty.UnsetValue"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        } //--метод ConvertBack
    } //---класс DancerTimeConverter
}
