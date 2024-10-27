using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects;
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
    internal class DhasRoleClassD : DhasRole
    {
        public const string name = "classd";

        public override RoleTypeId RoleType => RoleTypeId.ClassD;
        public override List<dhasTask> Tasks => new()
        {
            GetAKeycard,
            GetCandy,
            BeNearWhenTaskComplete,
            ShootSomeone,
        };

        public DhasRoleClassD(Player player, DhasRoleManager manager) : base(player, manager)
        {
            player.Role.Set(RoleType, RoleSpawnFlags.UseSpawnpoint);
        }

        public override void OnStop()
        {
            if (CurrentTask == ShootSomeone)
            {
                PlayerEvent.Hurting -= Hurting;
            }
        }


        Pickup myCard;

        [CrewmateTask(TaskDifficulty.Easy)]
        private IEnumerator<float> GetAKeycard()
        {
            bool predicate(Pickup pickup) => pickup.Type.IsKeycard();
            void onFail() { player.CurrentItem = player.AddItem(ItemType.KeycardScientist); }

            while (GoGetPickup(predicate, onFail) && MyTargetPickup != null)
            {
                myCard = MyTargetPickup;
                var compass = GetCompass(MyTargetPickup.Position);
                FormatTask("Pick up a Keycard", compass);
                yield return Timing.WaitForSeconds(0.5f);
            }
            MyKeycardType = myCard.Type;
        }

        [CrewmateTask(TaskDifficulty.Medium)]
        private IEnumerator<float> GetCandy()
        {
            while (player.Items.FirstOrDefault(x => x is Scp330) is not Scp330 scp330 || scp330.Candies.Count < 2)
            {
                FormatTask("Acquire 2 Candies", CompassToRoom(RoomType.Lcz330));
                yield return Timing.WaitForSeconds(1f);
            }
        }

        bool hurtBeast = false;

        [CrewmateTask(TaskDifficulty.Medium)]
        private IEnumerator<float> ShootSomeone()
        {
            Manager.PlayerCanHurtRoles(player, RoleTypeId.Scp939);
            PlayerEvent.Hurting += Hurting;

            while (!hurtBeast)
            {
                FormatTask("Shoot The Beast", HotAndColdToBeast());
                yield return Timing.WaitForSeconds(1);
            }

            PlayerEvent.Hurting -= Hurting;
            Manager.PlayerCannotHurt(player);
        }


        private void Hurting(HurtingEventArgs ev)
        {
            if (ev.Player.Role.Type == RoleTypeId.Scp939 && ev.Attacker == player && ev.DamageHandler?.Type.IsWeapon() == true) {
                hurtBeast = true;
            }
        }
    }
}
