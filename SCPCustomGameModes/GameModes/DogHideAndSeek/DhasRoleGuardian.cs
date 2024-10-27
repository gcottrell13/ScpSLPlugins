using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Pickups;
using Exiled.API.Structs;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace CustomGameModes.GameModes
{
    internal class DhasRoleGuardian : DhasRole
    {
        public const string name = "guardian";

        public override RoleTypeId RoleType => RoleTypeId.Scientist;

        public int RemainingLives;

        public override List<dhasTask> Tasks => new()
        {
            GetAKeycard,
            UpgradeKeycard,
            ToArmory,
            ProtectTeammates,
        };

        public DhasRoleGuardian(Player player, DhasRoleManager manager) : base(player, manager)
        {
            player.Role.Set(RoleType, RoleSpawnFlags.UseSpawnpoint);

            // since we have a never ending task, we can't accept any cooperative tasks.
            AlreadyAcceptedCooperativeTasks = player;
        }

        /// <summary>
        /// idempotent stop()
        /// </summary>
        public override void OnStop()
        {
            if (CurrentTask == ProtectTeammates)
            {
                // unbind
                PlayerEvent.Hurting -= Hurting;
                PlayerEvent.Dying -= OnDying;
            }
        }


        [CrewmateTask(TaskDifficulty.Easy)]
        private IEnumerator<float> GetAKeycard()
        {
            bool predicate(Pickup pickup) => pickup.Type == ItemType.KeycardScientist;
            void onFail() { player.CurrentItem = player.AddItem(ItemType.KeycardScientist); }

            while(GoGetPickup(predicate, onFail) && MyTargetPickup != null) {
                var compass = GetCompass(MyTargetPickup.Position);
                FormatTask("Pick up Your Scientist Keycard", compass);
                yield return Timing.WaitForSeconds(0.5f);
            }
            MyKeycardType = ItemType.KeycardScientist;

            // Assuming we have the keycard now
        }


        [CrewmateTask(TaskDifficulty.Medium)]
        private IEnumerator<float> ToArmory()
        {
            Room armory = Room.Get(RoomType.LczArmory);
            Door insideDoor = armory.Doors.FirstOrDefault(d => d.Rooms.Count == 1);
            Vector3 insideDirection = insideDoor.Position - armory.Doors.FirstOrDefault(d => d.Rooms.Count == 2).Position;
            insideDirection.Normalize();

            while (player.CurrentRoom != armory || Vector3.Dot(insideDirection, player.Position - insideDoor.Position) < 0 || DistanceTo(insideDoor.Position) < 1)
            {
                FormatTask("Go Inside the Armory", CompassToRoom(armory));
                yield return Timing.WaitForSeconds(1);
            }
        }

        [CrewmateTask(TaskDifficulty.Hard)]
        private IEnumerator<float> ProtectTeammates()
        {
            // bind event listeners
            PlayerEvent.Hurting += Hurting;
            PlayerEvent.Dying += OnDying;

            equipGuardian();

            if (OtherCrewmates.Count == 0)
            {
                yield return WaitHint("Avoid the Beast", 60);
                yield break;
            }

            yield return WaitHint("Get a Better Gun and Go Protect your Teammates!", 10f);

            var message = "Protect your Teammates";
            var teleports = "";
            bool didResetToOneLife = false;
            RemainingLives = 4;

            while (IsRunning)
            {
                if (!didResetToOneLife)
                {
                    if (OtherCrewmates.Count == 0)
                    {
                        didResetToOneLife = true;
                        message = "Your Teammates are all Dead!";
                        RemainingLives = 0;
                    }
                }

                if (RemainingLives > 0)
                {
                    var a = new List<string>();
                    for (int i = 0; i < RemainingLives; i++) a.Add("❤");
                    teleports = $"""
                        Remaining Emergency Teleports: 
                        <size=70><color=red>{strong(string.Join("", a))}</color></size>
                        """;
                }
                else
                {
                    teleports = "No More Emergency Teleports!";
                }

                FormatTask(message, teleports);
                yield return Timing.WaitForSeconds(1);
            }
        }


        #region Event Handlers

        private void Hurting(HurtingEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.SCPs && ev.Attacker == player)
            {
                ev.Player.EnableEffect(EffectType.SinkHole, duration: 2f);
            }
        }

        private void equipGuardian()
        {
            player.Health = 100;
            player.ArtificialHealth = 50;
            player.AddAmmo(AmmoType.Nato556, 50);
            player.AddAmmo(AmmoType.Ammo12Gauge, 50);
            player.AddAmmo(AmmoType.Ammo44Cal, 50);
            EnsureItem(ItemType.Flashlight);
            EnsureItem(ItemType.ArmorCombat);
            EnsureItem(ItemType.KeycardO5);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Player != player) return;

            RemainingLives--;

            if (RemainingLives < 0) return;

            ev.IsAllowed = false;

            player.EnableEffect(EffectType.Vitality, 5f);
            player.EnableEffect(EffectType.Flashed, 0.5f);

            equipGuardian();
            if (GetFarthestCrewmate() is Player teammate)
            {
                player.Position = teammate.Position;
            }
            else
            {
                player.Position = RoleTypeId.Scientist.GetRandomSpawnLocation().Position + Vector3.up;
            }
        }

        #endregion
    }
}
