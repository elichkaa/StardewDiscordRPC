namespace StardewDiscordRPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DiscordRPC;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;

    public class PresenceInvoker
    {
        private static string applicationId = "864785422889779200";
        private static readonly string steamId = "413150";
        private DiscordRpcClient rpcClient;
        private Timestamps timestamps;
        private int day;
        private JsonReader jsonReader;
        private ICollection<NPC> TalkedToToday = new List<NPC>();

        public PresenceInvoker(DiscordRpcClient rpcClient)
        {
            this.rpcClient = new DiscordRpcClient(applicationId, autoEvents: true);
            this.rpcClient.RegisterUriScheme(steamId);
            this.rpcClient.Initialize();
            this.jsonReader = new JsonReader();
            this.timestamps = Timestamps.Now;
        }

        public void SetBase(string details, string largeImageKey, string imageDescription, string state = "", Timestamps timestamps = null)
        {
            this.rpcClient.SetPresence(new RichPresence()
            {
                Assets = new Assets
                {
                    LargeImageKey = largeImageKey,
                    LargeImageText = imageDescription
                },
                Details = details,
                State = state,
                Timestamps = timestamps
            });
        }

        public void SetInitial()
        {
            this.rpcClient.SetPresence(new RichPresence
            {
                Assets = new Assets()
                {
                    LargeImageKey = "stardew_icon",
                    LargeImageText = "Stardew Valley best game",
                    SmallImageKey = "chicken_small"
                },
                Details = "Just started playing",
                Timestamps = timestamps
            });
        }

        public void ClearTalkedTo()
        {
            this.TalkedToToday.Clear();
        }

        public void SetLocationData(string locationType, string locationName)
        {
            var data = jsonReader.GetLocationObject(locationType, locationName);
            if (data == null)
            {
                this.SetBase($"Playing as {Game1.player.Name}", "stardew_icon", locationName, $"Visiting {locationName}", timestamps);
            }
            string name = data?["name"].ToString();
            string image = data?["imageKey"].ToString();
            this.SetBase($"Playing as {Game1.player.Name}", image, name,
                locationType == "farms" ? $"In {Game1.player.farmName} Farm" : $"Visiting {name}", timestamps);
        }

        public void ChangeLocation(WarpedEventArgs args)
        {
            var location = args.NewLocation;
            this.SetLocation(location);
        }

        public void SetLocation(GameLocation location)
        {
            if (location.IsFarm)
            {
                this.SetLocationData("farms", Game1.getFarm().mapPath.ToString());
            }
            else
            {
                this.SetLocationData("other", location.Name);
            }
        }

        public void SetCommunication()
        {
            var count = Game1.currentLocation.characters.Count;
            if (count != 0)
            {
                var npcs = Game1.currentLocation.characters;
                foreach (var npc in npcs)
                {
                    var talkedTo = Game1.player.hasPlayerTalkedToNPC(npc.Name);
                    if (talkedTo && !this.TalkedToToday.Contains(npc))
                    {
                        this.TalkedToToday.Add(npc);
                        var obj = this.jsonReader.GetNpc(npc.Name);
                        if (obj != null)
                        {
                            this.SetBase($"Talking to {obj?["name"]}", $"{obj?["image"]}", obj?["name"].ToString(), $"Visiting {Game1.currentLocation.Name}");
                        }
                        else
                        {
                            this.SetBase($"Talking to {npc.Name}", "stardew_icon", npc.Name, $"Visiting {Game1.currentLocation.Name}");
                        }
                    }
                }
            }
        }

        public void SetCurrentItem()
        {
            var currentItem = Game1.player.CurrentItem.GetType();
        }
    }
}
