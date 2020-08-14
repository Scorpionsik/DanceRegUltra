﻿using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DanceRegUltra.Static
{
    internal static class DanceRegCollections
    {
        public static Lazy<ListExt<CategoryString>> Leagues { get; private set; }

        public static Lazy<ListExt<CategoryString>> Ages { get; private set; }

        public static Lazy<ListExt<CategoryString>> Styles { get; private set; }

        public static ListExt<DanceEvent> Events { get; private set; }
        public static Lazy<List<int>> Id_active_events { get; private set; }

        static DanceRegCollections()
        {
            Events = new ListExt<DanceEvent>();
            Id_active_events = new Lazy<List<int>>();
            Leagues = new Lazy<ListExt<CategoryString>>();
            Ages = new Lazy<ListExt<CategoryString>>();
            Styles = new Lazy<ListExt<CategoryString>>();
            ClearCategories();
        }

        internal async static void ClearCategories()
        {
            if (Id_active_events == null || Id_active_events.Value == null || Id_active_events.Value.Count == 0)
            {
                await Task.Run(() =>
                {
                    while (Leagues.Value.Count > 0)
                    {
                        UnloadLeague(Leagues.Value[0]);
                    }
                });

                await Task.Run(() =>
                {
                    while (Ages.Value.Count > 0)
                    {
                        UnloadAge(Ages.Value[0]);
                    }
                });

                await Task.Run(() =>
                {
                    while (Styles.Value.Count > 0)
                    {
                        UnloadStyle(Styles.Value[0]);
                    }
                });
            }
            else
            {
                List<int> del_league = new List<int>(), del_age = new List<int>(), del_style = new List<int>();

                foreach(int id_event in Id_active_events.Value)
                {
                    //code here
                }
            }
        }

        private static async void MethodUpdateCategoryString(int id, CategoryType type, string columnName, object value)
        {
            string table_name = type.ToString().ToLower();
            string value_str = value.ToString();
            if (value.GetType() == typeof(string)) value_str = "'" + value_str + "'";

            bool name_dublicate = false;
            if(columnName == "Name")
            {
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from " + table_name + "s where Name=" + value_str);
                name_dublicate = res.RowsCount == 0 ? false : true;
            }

            if(!name_dublicate) await DanceRegDatabase.ExecuteNonQueryAsync("update " + table_name + "s set " + columnName + "=" + value_str + " where Id_" + table_name + "=" + id);
        }

        internal static int LoadLeague(CategoryString league)
        {
            int insert_index = CheckEnableId(CategoryType.League, league.Id);
            if (league.Type == CategoryType.League && insert_index == -1)
            {
                int id_sort = 0;
                while (id_sort < Leagues.Value.Count && Leagues.Value[id_sort].CompareTo(league) >= 0) id_sort++;
                league.Event_UpdateCategoryString += MethodUpdateCategoryString;
                Leagues.Value.Insert(id_sort, league);
                return id_sort;
            }
            else
            {
                return insert_index;
            }
        }

        
        private static void UnloadLeague(CategoryString league)
        {
            if(league.Type == CategoryType.League)
            {
                league.Event_UpdateCategoryString -= MethodUpdateCategoryString;
                Leagues.Value.Remove(league);
            }
        }

        internal static int LoadAge(CategoryString age)
        {
            int insert_index = CheckEnableId(CategoryType.Age, age.Id);
            if (age.Type == CategoryType.Age && insert_index == -1)
            {
                int id_sort = 0;
                while (id_sort < Ages.Value.Count && Ages.Value[id_sort].CompareTo(age) >= 0) id_sort++;
                age.Event_UpdateCategoryString += MethodUpdateCategoryString;
                Ages.Value.Insert(id_sort, age);
                return id_sort;
            }
            else return insert_index;
        }

        private static void UnloadAge(CategoryString age)
        {
            if (age.Type == CategoryType.Age)
            {
                age.Event_UpdateCategoryString -= MethodUpdateCategoryString;
                Ages.Value.Remove(age);
            }
        }

        internal static int LoadStyle(CategoryString style)
        {
            int insert_index = CheckEnableId(CategoryType.Style, style.Id);
            if (style.Type == CategoryType.Style && insert_index == -1)
            {
                int id_sort = 0;
                while (id_sort < Styles.Value.Count && Styles.Value[id_sort].CompareTo(style) >= 0) id_sort++;
                style.Event_UpdateCategoryString += MethodUpdateCategoryString;
                Styles.Value.Insert(id_sort, style);
                return id_sort;
            }
            else return insert_index;
        }

        private static void UnloadStyle(CategoryString style)
        {
            if (style.Type == CategoryType.Style)
            {
                style.Event_UpdateCategoryString -= MethodUpdateCategoryString;
                Styles.Value.Remove(style);
            }
        }

        private static int CheckEnableId(CategoryType type, int id)
        {
            int index = 0;
            switch (type)
            {
                case CategoryType.League:
                    foreach(CategoryString league in Leagues.Value)
                    {
                        if (league.Id == id) return index;
                        else index++;
                    }
                    break;
                case CategoryType.Age:
                    foreach(CategoryString age in Ages.Value)
                    {
                        if (age.Id == id) return index;
                        else index++;
                    }
                    break;
                case CategoryType.Style:
                    foreach(CategoryString style in Styles.Value)
                    {
                        if (style.Id == id) return index;
                        else index++;
                    }
                    break;
            }
            return -1;
        }

        internal static DanceEvent GetEventById(int id_event)
        {
            foreach(DanceEvent ev in Events)
            {
                if (ev.IdEvent == id_event) return ev;
            }
            return null;
        }

        internal static int GetPosition(CategoryString category)
        {
            int position = 0;
            switch (category.Type)
            {
                case CategoryType.League:
                    foreach(CategoryString league in Leagues.Value)
                    {
                        if (league.Id == category.Id) return position;
                        else position++;
                    }
                    break;
                case CategoryType.Age:
                    foreach (CategoryString age in Ages.Value)
                    {
                        if (age.Id == category.Id) return position;
                        else position++;
                    }
                    break;
                case CategoryType.Style:
                    foreach (CategoryString style in Styles.Value)
                    {
                        if (style.Id == category.Id) return position;
                        else position++;
                    }
                    break;
            }
            return -1;
        }
    }
}
