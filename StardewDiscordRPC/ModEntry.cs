﻿namespace StardewDiscordRPC
{
    using System.Collections.Generic;
    using DiscordRPC;
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
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
        private PresenceInvoker invoker;
        private bool saveLoaded;
        private JsonReader jsonReader = new JsonReader();

        public override void Entry(IModHelper helper)
        {
            this.invoker = new PresenceInvoker(rpcClient);

            //core events
            helper.Events.GameLoop.GameLaunched += (sender, args) => invoker.SetInitial();
            helper.Events.GameLoop.ReturnedToTitle += (sender, args) => invoker.SetInitial();
            helper.Events.GameLoop.Saved += (sender, args) => invoker.ClearTalkedTo();
            helper.Events.GameLoop.SaveLoaded += (sender, args) =>
            {
                invoker.SetLocationData("farms", Game1.getFarm().mapPath.ToString());
                this.saveLoaded = true;
            };
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicked;

            //location
            //helper.Events.World.ObjectListChanged
            helper.Events.Player.Warped += (sender, args) => invoker.ChangeLocation(args);

            //collection crops and minerals
            //helper.Events.Player.InventoryChanged += (sender, args) => invoker.CollectItems(args);
        }

        private void OnUpdateTicked(object sender, UpdateTickingEventArgs e)
        {
            if (e.IsMultipleOf(60) && saveLoaded)
            {
                if (Game1.currentLocation.characters.Count != 0)
                {
                    this.invoker.SetCommunication();
                }
                
                var currentItem = Game1.player.CurrentItem;
                Monitor.Log(currentItem.Name, LogLevel.Info);
            }

            if (e.IsMultipleOf(1500) && saveLoaded)
            {
                invoker.SetLocation(Game1.player.currentLocation);
            }
        }
    }
}