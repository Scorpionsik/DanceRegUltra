using CoreWPF.Utilites;
using DanceRegUltra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            ClearCategories();
        }

        internal static void ClearCategories()
        {
            Leagues = new Lazy<ListExt<CategoryString>>();
            Ages = new Lazy<ListExt<CategoryString>>();
            Styles = new Lazy<ListExt<CategoryString>>();
        }

        internal static void LoadLeague(CategoryString league)
        {
            if(league.Type == CategoryType.League && !CheckEnableId(CategoryType.League, league.Id))
            {
                int id_sort = 0;
                while (id_sort < Leagues.Value.Count && Leagues.Value[id_sort].CompareTo(league) >= 0) id_sort++;

                Leagues.Value.Insert(id_sort, league);
            }
        }

        internal static void UnloadLeague(CategoryString league)
        {
            if(league.Type == CategoryType.League)
            {
                Leagues.Value.Remove(league);
            }
        }

        internal static void LoadAge(CategoryString age)
        {
            if (age.Type == CategoryType.Age && !CheckEnableId(CategoryType.Age, age.Id))
            {
                int id_sort = 0;
                while (id_sort < Ages.Value.Count && Ages.Value[id_sort].CompareTo(age) >= 0) id_sort++;

                Ages.Value.Insert(id_sort, age);
            }
        }

        internal static void UnloadAge(CategoryString age)
        {
            if (age.Type == CategoryType.Age)
            {
                Ages.Value.Remove(age);
            }
        }

        internal static void LoadStyle(CategoryString style)
        {
            if (style.Type == CategoryType.Style && !CheckEnableId(CategoryType.Style, style.Id))
            {
                int id_sort = 0;
                while (id_sort < Styles.Value.Count && Styles.Value[id_sort].CompareTo(style) >= 0) id_sort++;

                Styles.Value.Insert(id_sort, style);
            }
        }

        internal static void UnloadStyle(CategoryString style)
        {
            if (style.Type == CategoryType.Style)
            {
                Styles.Value.Remove(style);
            }
        }

        private static bool CheckEnableId(CategoryType type, int id)
        {
            switch (type)
            {
                case CategoryType.League:
                    foreach(CategoryString league in Leagues.Value)
                    {
                        if (league.Id == id) return true;
                    }
                    break;
                case CategoryType.Age:
                    foreach(CategoryString age in Ages.Value)
                    {
                        if (age.Id == id) return true;
                    }
                    break;
                case CategoryType.Style:
                    foreach(CategoryString style in Styles.Value)
                    {
                        if (style.Id == id) return true;
                    }
                    break;
            }
            return false;
        }

        internal static DanceEvent GetEventById(int id_event)
        {
            try
            {
                return Events.FindFirst(new Func<DanceEvent, bool>(e =>
                {
                    if (e.IdEvent == id_event) return true;
                    else return false;
                }));
            }
            catch { return null; }
        }
    }
}
