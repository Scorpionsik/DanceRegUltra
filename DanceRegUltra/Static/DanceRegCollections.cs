using CoreWPF.Utilites;
using CoreWPF.Windows;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Views;
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

        private static Lazy<ListExt<MemberDancer>> Group_dancers { get; set; }

        public static Lazy<ListExt<IdTitle>> Schools { get; private set; }

        public static ListExt<DanceEvent> Events { get; private set; }
        public static Lazy<Dictionary<int, EventManagerView>> Active_events_windows { get; private set; }

        static DanceRegCollections()
        {
            Group_dancers = new Lazy<ListExt<MemberDancer>>();
            Events = new ListExt<DanceEvent>();
            Active_events_windows = new Lazy<Dictionary<int, EventManagerView>>();
            Leagues = new Lazy<ListExt<CategoryString>>();
            Ages = new Lazy<ListExt<CategoryString>>();
            Styles = new Lazy<ListExt<CategoryString>>();

            ClearCategories();
        }

        internal static void AddGroupDancer(MemberDancer dancer)
        {
            if (!IsExistGroupDancer(dancer.MemberId))
            {
                Group_dancers.Value.Add(dancer);
            }
        }

        internal static MemberDancer GetGroupDancerById(int id_dancer)
        {
            foreach (MemberDancer dancer in Group_dancers.Value)
            {
                if (dancer.MemberId == id_dancer) return dancer;
            }
            return null;
        }
        
        private static bool IsExistGroupDancer(int id_dancer)
        {
            foreach(MemberDancer dancer in Group_dancers.Value)
            {
                if (dancer.MemberId == id_dancer) return true;
            }
            return false;
        }
        

        internal async static void ClearCategories()
        {
            if (Active_events_windows == null || Active_events_windows.Value == null || Active_events_windows.Value.Count == 0)
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
                Group_dancers = new Lazy<ListExt<MemberDancer>>();
            }
            /*
            else
            {
                List<int> del_league = new List<int>(), del_age = new List<int>(), del_style = new List<int>();

                foreach(int id_event in Active_events_windows.Value.Keys)
                {
                    //code here
                }
            }*/
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

        internal async static Task LoadSchools()
        {
            if(Schools == null || Schools.Value == null || Schools.Value.Count == 0)
            {
                Schools = new Lazy<ListExt<IdTitle>>();

                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from schools");

                foreach(DbRow row in res)
                {
                    Schools.Value.Add(new IdTitle(row["Id_school"].ToInt32(), row["Name"].ToString()));
                }
            }
        }

        internal static IdTitle GetSchoolById(int id_school)
        {
            foreach(IdTitle school in Schools.Value)
            {
                if (school.Id == id_school) return school;
            }
            return null;
        }

        internal static bool LoadEvent(DanceEvent eventLoad)
        {
            if (Active_events_windows.Value.ContainsKey(eventLoad.IdEvent))
            {
                //Active_events_windows.Value[eventLoad.IdEvent].Activate();
                return false;
            }
            else
            {
                EventManagerView eventWindow = new EventManagerView(eventLoad.IdEvent);
                Active_events_windows.Value.Add(eventLoad.IdEvent, eventWindow);
                return true;
            }
        }

        internal static void UnloadEvent(DanceEvent eventUnload)
        {
            if (Active_events_windows.Value.ContainsKey(eventUnload.IdEvent))
            {
                eventUnload.UnloadEvent();
                Active_events_windows.Value.Remove(eventUnload.IdEvent);
                ClearCategories();
            }
        }

        /*
        internal async static Task LoadDancersAsync()
        {
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from dancers");
            foreach(DbRow row in res)
            {
                Dancers.Value.Add(new MemberDancer(-1, row["Id_dancer"].ToInt32(), row["Firstname"].ToString(), row["Surname"].ToString()));
            }
        }

        internal async static Task<int> AddDancerAsync(string name, string surname)
        {
            await DanceRegDatabase.ExecuteNonQueryAsync("insert into dancers (Firstname, Surname) values ('" + name + "', '" + surname + "')");
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_dancer from dancers order by Id_dancer");
            return res["Id_dancer", res.RowsCount - 1].ToInt32();
        }

        internal async static Task RemoveDancerAsync(MemberDancer dancer)
        {
            await DanceRegDatabase.ExecuteNonQueryAsync("delete * from dancers where Id_dancer=" + dancer.MemberId);
            await Task.Run(() =>
            {
                foreach (MemberDancer d in Dancers.Value)
                {
                    if (d.MemberId == dancer.MemberId)
                    {
                        Dancers.Value.Remove(d);
                        break;
                    }
                }
            });
        }*/

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
