using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ModManager.Common.Structs
{
    public class Facets
    {
        private List<List<string>> Items;

        public Facets()
        {
            Items = new List<List<string>>();
        }

        public void AddItem(string type, string value)
        {
            Items.Add(new List<string> { $"{type}:{value}" });
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(Items);
        }
    }
}
