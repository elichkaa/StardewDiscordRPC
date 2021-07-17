namespace StardewDiscordRPC
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Timers;
    using DiscordRPC;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using Timer = System.Timers.Timer;

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
        private PresenceInvoker invoker;
        private bool saveLoaded;
        private ICollection<NPC> TalkedToToday = new List<NPC>();
        private JsonReader jsonReader = new JsonReader();

        public override void Entry(IModHelper helper)
        {
            this.invoker = new PresenceInvoker(rpcClient);

            //core events
            helper.Events.GameLoop.GameLaunched += (sender, args) => invoker.SetInitial();
            helper.Events.GameLoop.ReturnedToTitle += (sender, args) => invoker.SetInitial();
            helper.Events.GameLoop.SaveLoaded += (sender, args) =>
            {
                invoker.SetLocationData("farms", Game1.getFarm().mapPath.ToString());
                this.saveLoaded = true;
            };
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicked;

            //location
            helper.Events.Player.Warped += (sender, args) => invoker.ChangeLocation(args);
        }

        private void OnUpdateTicked(object sender, UpdateTickingEventArgs e)
        {
            if (e.IsMultipleOf(60) && saveLoaded)
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
                                this.invoker.SetBase($"Talking to {obj?["name"]}", $"{obj?["image"]}", obj?["name"].ToString(), $"{Game1.currentLocation.Name}");
                            }
                            else
                            {
                                this.invoker.SetBase($"Talking to {npc.Name}", "cat", npc.Name, $"{Game1.currentLocation.Name}");
                            }
                        }
                    }
                }
            }

            if (e.IsMultipleOf(1500) && saveLoaded)
            {
                invoker.SetLocation(Game1.player.currentLocation);
            }
        }
    }
}