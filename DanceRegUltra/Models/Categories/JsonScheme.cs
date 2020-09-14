using DanceRegUltra.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public string GetSchemeTypeById(int id, SchemeType type)
        {
            switch (type)
            {
                case SchemeType.Platform:
                    foreach(JsonSchemeArray platform in this.Platforms)
                    {
                        if (platform.IdArray == id) return platform.Title;
                    }
                    break;
                case SchemeType.Block:
                    foreach(JsonSchemeArray block in this.Blocks)
                    {
                        if (block.IdArray == id) return block.Title;
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Проверка двух узлов, возвращает 1 если можно внедрять
        /// </summary>
        /// <param name="node1">Узел-опора</param>
        /// <param name="node2">УзелЮ который надо внедрить</param>
        /// <returns></returns>
        public int Compare(DanceNode node1, DanceNode node2)
        {
            int level1 = 0, level2 = 0, step = 1, count = 0;
            while(step < 5)
            {
                switch (step)
                {
                    /*
                    case 1:
                        if (this.Platforms[count].IdArray == node1.Platform.Id) level1 = count + 1;
                        if (this.Platforms[count].IdArray == node2.Platform.Id) level2 = count + 1;
                        if (level1 > 0 && level2 > 0)
                        {
                            if (level1 > level2) return 1;
                            else if (level1 < level2) return -1;
                            else
                            {
                                category_count = count;
                                step++;
                                level1 = 0;
                                level2 = 0;
                                count = -1;
                            }
                        }
                        count++;
                        break;
                        */
                    case 1:
                        if (this.Blocks[count].IdArray == node1.Block.Id) level1 = count + 1;
                        if (this.Blocks[count].IdArray == node2.Block.Id) level2 = count + 1;
                        if (level1 > 0 && level2 > 0)
                        {
                            if (level1 > level2) return 1;
                            else if (level1 < level2) return -1;
                            else
                            {
                                step++;
                                level1 = 0;
                                level2 = 0;
                                count = -1;
                            }
                        }
                        count++;
                        break;
                    case 2:
                        if (this.Platforms[0].Values[count].Id == node1.LeagueId) level1 = count + 1;
                        if (this.Platforms[0].Values[count].Id == node2.LeagueId) level2 = count + 1;
                        if (level1 > 0 && level2 > 0)
                        {
                            if (level1 > level2) return 1;
                            else if (level1 < level2) return -1;
                            else
                            {
                                step++;
                                level1 = 0;
                                level2 = 0;
                                count = -1;
                            }
                        }
                        count++;
                        break;
                    case 3:
                        if (this.Blocks[0].Values[count].Id == node1.AgeId) level1 = count + 1;
                        if (this.Blocks[0].Values[count].Id == node2.AgeId) level2 = count + 1;
                        if (level1 > 0 && level2 > 0)
                        {
                            if (level1 > level2) return 1;
                            else if (level1 < level2) return -1;
                            else
                            {
                                step++;
                                level1 = 0;
                                level2 = 0;
                                count = -1;
                            }
                        }
                        count++;
                        break;
                    case 4:
                        if (this.Styles[count].IsChecked)
                        {
                            if (this.Styles[count].Id == node1.StyleId) level1 = count + 1;
                            if (this.Styles[count].Id == node2.StyleId) level2 = count + 1;
                            if (level1 > 0 && level2 > 0)
                            {
                                if (level1 > level2) return 1;
                                else if (level1 < level2) return -1;
                                else
                                {
                                    step++;
                                }
                            }
                        }
                        count++;
                        break;

                }
            }
            return -1;
        }
    }
}
