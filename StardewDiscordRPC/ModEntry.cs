namespace StardewDiscordRPC
{
    using System;
    using DiscordRPC;
    using StardewModdingAPI;

    public class ModEntry : Mod
    {
        #region infos
        /*
         * static means no need for initialization
         * Game1 - static
         * Context - static 
         * helper - events and stuff
         */
        #endregion

        private static string applicationId = "864785422889779200";
        private static readonly string steamId = "413150";
        private DiscordRpcClient rpcClient;

        public override void Entry(IModHelper helper)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            rpcClient = new DiscordRpcClient(applicationId);
            rpcClient.RegisterUriScheme(steamId);

            rpcClient.OnReady += (sender, e) =>
            {
                Monitor.Log($"Connected to user {e.User.Username}");
            };

            rpcClient.Initialize();

            rpcClient.SetPresence(new RichPresence()
            {
                Details = "In game",
                State = "Just started playing",
                Assets = new Assets()
                {
                    LargeImageKey = "stardew_icon",
                    LargeImageText = "Stardew Valley best game",
                    SmallImageKey = "chicken_small"
                }
            });
        }
    }
}