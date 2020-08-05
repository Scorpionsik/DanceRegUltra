using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreWPF.Utilites;
using Newtonsoft.Json;

namespace DanceRegUltra.Models
{
    public struct DanceScheme
    {
        public ListExt<int> LeagueValues { get; private set; }

        public ListExt<int> AgeValues { get; private set; }

        public ListExt<int> StyleValues { get; private set; }

        public ListExt<SchemeArray> PlatformsCollection { get; private set; }

        public ListExt<SchemeArray> BlocksCollection { get; private set; }

        public DanceScheme(IEnumerable<SchemeArray> platforms, IEnumerable<SchemeArray> blocks, IEnumerable<int> styles)
        {
            this.LeagueValues = new ListExt<int>();
            this.AgeValues = new ListExt<int>();
            this.StyleValues = new ListExt<int>(styles);

            this.PlatformsCollection = new ListExt<SchemeArray>();
            this.BlocksCollection = new ListExt<SchemeArray>();

            foreach(SchemeArray platform in platforms)
            {
                this.PlatformsCollection.Add(platform);
                foreach(int league in platform.SchemePartValues)
                {
                    if (!this.LeagueValues.Contains(league)) this.LeagueValues.Add(league);
                }
            }

            foreach(SchemeArray block in blocks)
            {
                this.BlocksCollection.Add(block);
                foreach(int age in block.SchemePartValues)
                {
                    if (!this.AgeValues.Contains(age)) this.AgeValues.Add(age);
                }
            }
        }

        public static string Serialize(DanceScheme scheme)
        {
            return JsonConvert.SerializeObject(scheme);
        }

        public static DanceScheme Deserialize(string jsonScheme)
        {
            return JsonConvert.DeserializeObject<DanceScheme>(jsonScheme);
        }
    }
}
