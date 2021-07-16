namespace StardewDiscordRPC
{
    using System;
    using System.Linq;
    using DiscordRPC;
    using Microsoft.Xna.Framework;
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
                this.SetBase($"Playing as {Game1.player.Name}", "cat", locationName, $"Visiting {locationName}", timestamps);
            }
            string name = data?["name"].ToString();
            string image = data?["imageKey"].ToString();
            this.SetBase($"Playing as {Game1.player.Name}", image, name,
                locationType == "farms" ? $"In {Game1.player.farmName} Farm" : $"Visiting {name}", timestamps);
        }

        public void ChangeLocation(WarpedEventArgs args)
        {
            var location = args.NewLocation;
            if (location.IsFarm)
            {
                this.SetLocationData("farms", Game1.getFarm().mapPath.ToString());
            }
            else
            {
                this.SetLocationData("other", location.Name);
            }
        }
    }
}
