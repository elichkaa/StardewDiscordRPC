namespace StardewDiscordRPC
{
    using System.IO;
    using System.Runtime.InteropServices;
    using DiscordRPC;
    using StardewModdingAPI;
    using StardewValley;

    public class ModEntry : Mod
    {
        #region infos
        /*
         * static means no need for initialization
         * Game1 - static
         * Context - static 
         * helper - events and stuff
         * sdate 
         */
        #endregion

        private static string applicationId = "864785422889779200";
        private static readonly string steamId = "413150";
        private DiscordRpcClient rpcClient;

        public override void Entry(IModHelper helper)
        {
            var invoker = new PresenceInvoker(rpcClient);

            //core events
            helper.Events.GameLoop.GameLaunched += (sender, args) => invoker.SetInitial();
            helper.Events.GameLoop.ReturnedToTitle += (sender, args) => invoker.SetInitial();
            helper.Events.GameLoop.SaveLoaded += (sender, args) =>
            {
                invoker.SetLocationData("farms", Game1.getFarm().mapPath.ToString());
            };

            
            //location
            helper.Events.Player.Warped += (sender, args) => invoker.ChangeLocation(args);
        }
    }
}