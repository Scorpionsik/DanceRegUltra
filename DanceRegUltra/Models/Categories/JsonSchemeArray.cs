using System.Collections.Generic;

namespace DanceRegUltra.Models.Categories
{
    public class JsonSchemeArray
    {
        public int IdArray { get; set; }

        public string Title { get; set; }

        public List<IdCheck> Values { get; set; }

        public JsonSchemeArray()
        {
            this.Title = "";
            this.Values = new List<IdCheck>();
        }

        public JsonSchemeArray(SchemeArray sArray)
        {
            this.IdArray = sArray.IdArray;
            this.Title = sArray.TitleSchemePart;
            this.Values = new List<IdCheck>();
            foreach(IdCheck value in sArray.SchemePartValues)
            {
                this.Values.Add(new IdCheck(value.Id, value.IsChecked));
            }
        }
    }
}
