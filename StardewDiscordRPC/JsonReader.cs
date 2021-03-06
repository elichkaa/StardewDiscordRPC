namespace StardewDiscordRPC
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json.Linq;

    public class JsonReader
    {
        private string baseFilePath = Directory.GetCurrentDirectory() + "/Mods/StardewDiscordRPC/data/";

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

        public Dictionary<string, object> GetNpc(string key)
        {
            string json = System.IO.File.ReadAllText($"{baseFilePath}" + "npcs.json");

            var members = JArray.Parse(json);

            foreach (var member in members)
            {
                if (member["key"].ToString() == key)
                {
                    var values = JObject.FromObject(member).ToObject<Dictionary<string, object>>();
                    return values;
                }
            }

            return null;
        }

        public Dictionary<string, object> GetToolOrWeapon(string key, string specificName)
        {
            string json = System.IO.File.ReadAllText($"{baseFilePath}" + "toolsAndWeapons.json");

            var parsed = JObject.Parse(json);
            var root = parsed.GetValue(key);
            var members = root.Children();

            foreach (var member in members)
            {
                if (member["key"].ToString() == specificName)
                {
                    var values = JObject.FromObject(member).ToObject<Dictionary<string, object>>();
                    return values;
                }
            }

            return null;
        }
    }
}
