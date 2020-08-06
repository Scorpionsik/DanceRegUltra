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

        static DanceRegCollections()
        {
            Events = new ListExt<DanceEvent>();
            ClearCollections();
        }

        internal static void ClearCollections()
        {
            Leagues = new Lazy<ListExt<CategoryString>>();
            Ages = new Lazy<ListExt<CategoryString>>();
            Styles = new Lazy<ListExt<CategoryString>>();
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
