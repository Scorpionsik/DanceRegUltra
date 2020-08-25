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
        public int PlatformIncrement { get; set; }
        public int BlockIncrement { get; set; }

        public List<JsonSchemeArray> Platforms { get; set; }
        public List<JsonSchemeArray> Blocks { get; set; }
        public List<IdCheck> Styles { get; set; }


        public JsonScheme()
        {
            this.Platforms = new List<JsonSchemeArray>();
            this.Blocks = new List<JsonSchemeArray>();
            this.Styles = new List<IdCheck>();

            this.PlatformIncrement = 0;
            this.BlockIncrement = 0;
        }
        public JsonScheme(DanceScheme scheme)
        {
            this.Platforms = new List<JsonSchemeArray>();
            this.Blocks = new List<JsonSchemeArray>();
            this.Styles = new List<IdCheck>();

            this.PlatformIncrement = scheme.PlatformIncrement;
            this.BlockIncrement = scheme.BlockIncrement;

            foreach (SchemeArray platform in scheme.PlatformsCollection)
            {
                this.Platforms.Add(new JsonSchemeArray(platform));
            }

            foreach (SchemeArray block in scheme.BlocksCollection)
            {
                this.Blocks.Add(new JsonSchemeArray(block));
            }

            foreach (IdCheck style in scheme.SchemeStyles)
            {
                this.Styles.Add(new IdCheck(style.Id, style.IsChecked));
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
