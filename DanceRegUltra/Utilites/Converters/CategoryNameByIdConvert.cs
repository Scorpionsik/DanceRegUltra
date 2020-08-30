using DanceRegUltra.Enums;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
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
    class CategoryNameByIdConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id && id > 0)
            {
                switch (parameter as string)
                {
                    case "league":
                        foreach(CategoryString league in DanceRegCollections.Leagues.Value)
                        {
                            if (league.Id == id) return league.Name;
                        }
                        break;
                    case "age":
                        foreach(CategoryString age in DanceRegCollections.Ages.Value)
                        {
                            if (age.Id == id) return age.Name;
                        }
                        break;
                    case "style":
                        foreach(CategoryString style in DanceRegCollections.Styles.Value)
                        {
                            if (style.Id == id) return style.Name;
                        }
                        break;
                }
            }
            else if(value is CategoryType category)
            {
                switch (category)
                {
                    case CategoryType.League:
                        return "Лиги";
                    case CategoryType.Age:
                        return "Возрастные категории";
                    case CategoryType.Style:
                        return "Стили";
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public static string Convert(int category_id, CategoryType type)
        {
            if (category_id > 0)
            {
                switch (type)
                {
                    case CategoryType.League:
                        foreach (CategoryString league in DanceRegCollections.Leagues.Value)
                        {
                            if (league.Id == category_id) return league.Name;
                        }
                        break;
                    case CategoryType.Age:
                        foreach (CategoryString age in DanceRegCollections.Ages.Value)
                        {
                            if (age.Id == category_id) return age.Name;
                        }
                        break;
                    case CategoryType.Style:
                        foreach (CategoryString style in DanceRegCollections.Styles.Value)
                        {
                            if (style.Id == category_id) return style.Name;
                        }
                        break;
                }
                return null;
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}