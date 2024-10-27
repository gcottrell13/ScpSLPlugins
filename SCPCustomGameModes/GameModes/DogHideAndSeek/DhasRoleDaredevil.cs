using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace CustomGameModes.GameModes
{
    internal class DhasRoleDaredevil : DhasRole
    {
        public const string name = "daredevil";

        private List<Door> ClassDDoorsTouched = new();

        private HashSet<Room> GhostlightsThrown = new();

        private Room? LastRoomThrownGhostlight;
        private float GhostLightCooldownPenalty = 0f;

        public override RoleTypeId RoleType => RoleTypeId.ClassD;

        bool givingGhostlight = false;

        public override List<dhasTask> Tasks => new()
        {
            ThrowGhostlights,
            TouchAllClassDDoors,
            FindAPlayer,
            BeNearBeast,
        };

        public DhasRoleDaredevil(Player player, DhasRoleManager manager) : base(player, manager)
        {
            player.Role.Set(RoleType, RoleSpawnFlags.UseSpawnpoint);
        }

        /// <summary>
        /// idempotent stop()
        /// </summary>
        public override void OnStop()
        {
            if (CurrentTask == TouchAllClassDDoors)
            {
                PlayerEvent.InteractingDoor -= doorTouch;
            }
            if (CurrentTask == ThrowGhostlights)
            {
                PlayerEvent.UsedItem -= ghostlightUse;
                PlayerEvent.ThrownProjectile -= ghostlightUse;
            }
        }

        private void doorTouch(InteractingDoorEventArgs e)
        {
            if (e.Door.Room.Type == RoomType.LczClassDSpawn 
                && ClassDDoorsTouched.Contains(e.Door) == false
                )
            {
                ClassDDoorsTouched.Add(e.Door);
            }
        }

        private void ghostlightUse(IItemEvent e)
        {
            if (CurrentTask == ThrowGhostlights && e is IPlayerEvent pe)
            {
                if (e.Item.Type != ItemType.SCP2176 || pe.Player != player) return;
                if (e is IPickupEvent pickupEvent && pickupEvent.Pickup.Room.Type == RoomType.Lcz330 && e is IDeniableEvent d) {
                    d.IsAllowed = false;
                    return;
                }

                GhostlightsThrown.Add(player.CurrentRoom);
                if (!givingGhostlight)
                {
                    givingGhostlight = true;
                    if (LastRoomThrownGhostlight == player.CurrentRoom)
                        GhostLightCooldownPenalty += 2f;
                    else
                        GhostLightCooldownPenalty = 0;

                    LastRoomThrownGhostlight = player.CurrentRoom;

                    Timing.CallDelayed(10f + GhostLightCooldownPenalty, () =>
                    {
                        givingGhostlight = false;
                        player.CurrentItem = player.AddItem(ItemType.SCP2176);
                    });
                }
            }
        }


        [CrewmateTask(TaskDifficulty.Medium)]
        private IEnumerator<float> TouchAllClassDDoors()
        {
            PlayerEvent.InteractingDoor += doorTouch;

            var allClassDDoors = Door.Get(door => door.Room.Type == RoomType.LczClassDSpawn).ToList();
            var diff = allClassDDoors.Count - ClassDDoorsTouched.Count;
            while (diff > 0)
            {
                diff = allClassDDoors.Count - ClassDDoorsTouched.Count;

                if (ClassDDoorsTouched.Count == 0)
                    FormatTask($"Touch all {allClassDDoors.Count} Class-D Doors.", "");
                else
                    FormatTask($"Touch the Remaining {diff} Class-D Doors.", "");
                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        [CrewmateTask(TaskDifficulty.Easy)]
        private IEnumerator<float> ThrowGhostlights()
        {
            var requiredRoomCount = 6;
            player.CurrentItem = player.AddItem(ItemType.SCP2176);

            PlayerEvent.UsedItem += ghostlightUse;
            PlayerEvent.ThrownProjectile += ghostlightUse;

            while (GhostlightsThrown.Count < requiredRoomCount)
            {
                var diff = requiredRoomCount - GhostlightsThrown.Count;
                var more = GhostlightsThrown.Count == 0 ? "" : " more";
                FormatTask($"Throw scp2176 from {strong(diff)}{more} unique rooms", "");
                yield return Timing.WaitForSeconds(1);
            }

            PlayerEvent.UsedItem -= ghostlightUse;
            PlayerEvent.ThrownProjectile -= ghostlightUse;
        }

        [CrewmateTask(TaskDifficulty.Medium)]
        private IEnumerator<float> BeNearBeast()
        {
            var mustRunSeconds = 3f;
            var timeElapsed = 0f;

            void hint()
            {
                if (timeElapsed == 0f)
                    FormatTask($"Be near the Beast for\n{mustRunSeconds} seconds", HotAndColdToBeast());
                else
                    FormatTask($"Be near for an additional\n{mustRunSeconds - timeElapsed} seconds", HotAndColdToBeast());
            }

            while (timeElapsed < mustRunSeconds && Beast != null)
            {
                if (IsNear(Beast, 10)) timeElapsed += 0.5f;
                hint();
                yield return Timing.WaitForSeconds(0.5f);
            }
        }

    }
}
