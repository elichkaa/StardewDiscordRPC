namespace Playground
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JsonReaderTest
    {
        private string baseFilePath = "../../../";

        public Dictionary<string, object> GetLocationObject(string key, string specificName)
        {
            string json = System.IO.File.ReadAllText($"{baseFilePath}" + "locations.json");

            var parsed = JObject.Parse(json);
            var root = parsed.GetValue(key);
            var members = root.Children();

            foreach (var member in members)
            {
                if (member["type"].ToString() == specificName)
                {
                    var values = JObject.FromObject(member).ToObject<Dictionary<string, object>>();
                    return values;
                }
            }

            return null;
        }
    }
}
