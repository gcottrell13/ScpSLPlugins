using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using MEC;
using Exiled.API.Enums;
using UnityEngine;

namespace CustomGameModes.GameModes.Normal
{
    internal class CellGuard
    {
        public CellGuard()
        {
        }

        ~CellGuard()
        {
            UnsubscribeEventHandlers();
        }

        public void SubscribeEventHandlers()
        {
            Timing.CallDelayed(0.5f, () =>
            {
                var guards = Player.Get(RoleTypeId.FacilityGuard).ToList();
                var numCellGuards = Player.Get(RoleTypeId.ClassD).Count() switch
                {
                    <= 5 => 0,
                    <= 10 => 1,
                    _ => 2,
                };
                for (var i = 0; i < numCellGuards; i++)
                {
                    var classDDoors = Room.Get(RoomType.LczClassDSpawn).Doors.Where(door => door.Rooms.Count == 2).First();
                    guards[i].Position = Vector3.up + classDDoors.Position - classDDoors.Transform.forward * i * 3;
                }
            });
        }

        public void UnsubscribeEventHandlers()
        {
        }

    }
}
