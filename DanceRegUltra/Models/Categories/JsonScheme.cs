using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models.Categories
{
    public class JsonScheme
    {
        public List<JsonSchemeArray> Platforms { get; private set; }
        public List<JsonSchemeArray> Blocks { get; private set; }
        public List<int> Styles { get; private set; }

        public JsonScheme(DanceScheme scheme)
        {
            this.Platforms = new List<JsonSchemeArray>();
            this.Blocks = new List<JsonSchemeArray>();
            this.Styles = new List<int>();

            foreach(SchemeArray platform in scheme.PlatformsCollection)
            {
                this.Platforms.Add(new JsonSchemeArray(platform));
            }

            foreach(SchemeArray block in scheme.BlocksCollection)
            {
                this.Blocks.Add(new JsonSchemeArray(block));
            }

            foreach(IdCheck style in scheme.SchemeStyles)
            {
                if (style.IsChecked) this.Styles.Add(style.Id);
            }
        }

        public static string Serialize(JsonScheme scheme)
        {
            return JsonConvert.SerializeObject(scheme);
        }

        public static JsonScheme Deserialize(string jsonScheme)
        {
            return JsonConvert.DeserializeObject<JsonScheme>(jsonScheme);
        }
    }
}
