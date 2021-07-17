namespace StardewDiscordRPC
{
    using System;
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
        private JsonReader jsonReader;

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

        public void SetCommunicationPresence(LevelChangedEventArgs args)
        {
            var npcs = args.Player.currentLocation.characters;
            foreach (var npc in npcs)
            {
                var talked = args.Player.hasPlayerTalkedToNPC(npc.Name);
                if (talked)
                {
                    this.SetBase(npc.Name, "cat", npc.Name, "", timestamps);
                }
            }
        }

        public void OnUpdateTicked(UpdateTickingEventArgs e)
        {
            if (e.IsMultipleOf(60))
            {
                var count = Game1.player.currentLocation.characters.Count;
                if (count != 0)
                {
                    var npcs = Game1.player.currentLocation.characters.Select(x => x.Name);
                    foreach (var npc in npcs)
                    {
                        var talkedTo = Game1.player.hasPlayerTalkedToNPC(npc);
                        if (talkedTo)
                        {
                            this.SetBase($"Talking to: {npc}", "cat", npc, "djfsahk");
                        }
                    }
                }
            }
        }
    }
}
