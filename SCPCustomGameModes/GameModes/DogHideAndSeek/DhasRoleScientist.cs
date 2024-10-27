using CustomGameModes.API;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace CustomGameModes.GameModes
{
    internal class DhasRoleScientist : DhasRole
    {
        public const string name = "scientist";

        public override RoleTypeId RoleType => RoleTypeId.Scientist;

        public ItemType SCP1;

        public override List<dhasTask> Tasks => new()
        {
            GetAKeycard,
            UpgradeKeycard,
            UseFirstSCP,
            UseSecondSCP,
        };

        public DhasRoleScientist(Player player, DhasRoleManager manager) : base(player, manager)
        {
            player.Role.Set(RoleType, RoleSpawnFlags.UseSpawnpoint);
        }

        /// <summary>
        /// idempotent stop()
        /// </summary>
        public override void OnStop()
        {
            if (CurrentTask == UseFirstSCP || CurrentTask == UseSecondSCP)
            {
                PlayerEvent.DroppedItem -= dropped;
                PlayerEvent.ThrownProjectile -= thrown;
                PlayerEvent.UsedItem -= used;
            }
        }

        [CrewmateTask(TaskDifficulty.Easy)]
        private IEnumerator<float> GetAKeycard()
        {
            bool predicate(Pickup pickup) => pickup.Type == ItemType.KeycardScientist;
            void onFail() { player.CurrentItem = player.AddItem(ItemType.KeycardScientist); }

            while (GoGetPickup(predicate, onFail) && MyTargetPickup != null)
            {
                var compass = GetCompass(MyTargetPickup.Position);
                FormatTask("Pick up Your Scientist Keycard", compass);
                yield return Timing.WaitForSeconds(0.5f);
            }

            MyKeycardType = ItemType.KeycardScientist;

        }

        [CrewmateTask(TaskDifficulty.Medium)]
        private IEnumerator<float> UseFirstSCP()
        {
            // First Fetch
            bool predicate(Pickup pickup) => pickup.Type.IsScp();
            void onFail() { player.AddItem(ItemType.SCP207); }
            ItemType scp = 0;

            Log.Debug("SCIENTIST SCP 1");

            while (GoGetPickup(predicate, onFail) && MyTargetPickup != null)
            {
                scp = MyTargetPickup.Type;
                var compass = GetCompass(MyTargetPickup.Position);
                FormatTask($"Retrieve {strong(MyTargetPickup.Type)}", compass);
                yield return Timing.WaitForSeconds(0.5f);
            }

            // Then Use

            if (scp != 0)
            {
                usedItem = false;
                CanDropItem(scp);
                targetSCP = scp;
                SCP1 = scp;

                PlayerEvent.DroppedItem += dropped;
                PlayerEvent.ThrownProjectile += thrown;
                PlayerEvent.UsedItem += used;

                while (!usedItem)
                {
                    FormatTask($"Use {strong(scp)}", "");
                    yield return Timing.WaitForSeconds(1);
                }

                PlayerEvent.DroppedItem -= dropped;
                PlayerEvent.ThrownProjectile -= thrown;
                PlayerEvent.UsedItem -= used;

                CannotDropItem(scp);
            }
            else
                Log.Warn($"No SCP found for scientist");

        }

        [CrewmateTask(TaskDifficulty.Medium)]
        private IEnumerator<float> UseSecondSCP()
        {
            bool predicate(Pickup pickup) => pickup.Type.IsScp() && pickup.Type != SCP1;
            void onFail() { player.AddItem(ItemType.AntiSCP207); }
            ItemType scp = 0;

            Log.Debug("SCIENTIST SCP 2");

            while (GoGetPickup(predicate, onFail) && MyTargetPickup != null)
            {
                scp = MyTargetPickup.Type;
                var compass = GetCompass(MyTargetPickup.Position);
                FormatTask($"Retrieve {strong(MyTargetPickup.Type)}", compass);
                yield return Timing.WaitForSeconds(0.5f);
            }

            // Then Use

            if (scp != 0)
            {
                usedItem = false;
                CanDropItem(scp);
                targetSCP = scp;
                SCP1 = scp;

                PlayerEvent.DroppedItem += dropped;
                PlayerEvent.ThrownProjectile += thrown;
                PlayerEvent.UsedItem += used;

                while (!usedItem)
                {
                    FormatTask($"Use {strong(scp)}", "");
                    yield return Timing.WaitForSeconds(1);
                }

                PlayerEvent.DroppedItem -= dropped;
                PlayerEvent.ThrownProjectile -= thrown;
                PlayerEvent.UsedItem -= used;

                CannotDropItem(scp);
            }
            else
                Log.Warn($"No SCP found for scientist");
        }

        bool playerEv(IPlayerEvent ev) => ev.Player == player;

        bool usedItem = false;
        ItemType targetSCP;
        void used(UsedItemEventArgs e) { if (playerEv(e) && e.Item.Type == targetSCP) usedItem = true; }
        void thrown(ThrownProjectileEventArgs e) { if (playerEv(e) && e.Item.Type == targetSCP) usedItem = true; }
        void dropped(DroppedItemEventArgs e) { if (playerEv(e) && e.Pickup.Type == targetSCP) usedItem = true; }

        /*
         * 
         * 
         * 
    SCP500,
    SCP207,
    SCP018,
    SCP268,
    SCP330,
    SCP2176,
    SCP244a,
    SCP244b,
    SCP1853,
    SCP1576,
    AntiSCP207,
         * 
         * 
         */
    }
}
