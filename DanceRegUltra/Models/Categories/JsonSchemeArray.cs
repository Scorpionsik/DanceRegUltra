using System.Collections.Generic;

namespace DanceRegUltra.Models.Categories
{
    public class JsonSchemeArray
    {
        public string Title { get; private set; }

        public List<int> Values { get; private set; }

        public JsonSchemeArray(SchemeArray sArray)
        {
            this.Title = sArray.TitleSchemePart;
            this.Values = new List<int>();
            foreach(IdCheck value in sArray.SchemePartValues)
            {
                if (value.IsChecked) this.Values.Add(value.Id);
            }
        }
    }
}
