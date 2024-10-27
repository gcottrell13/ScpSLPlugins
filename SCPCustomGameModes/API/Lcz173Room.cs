using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Toys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomGameModes.API
{
    internal class Lcz173Room
    {
        private static Room thisroom;
        public static Door gate;
        private static Door otherdoor;
        private static Vector3 behindHere;

        public static Vector3 GetCubeSpawn()
        {
            thisroom = Room.Get(Exiled.API.Enums.RoomType.Lcz173);

            Log.Debug($"LCZ 173 room at: {thisroom.Position}");

            foreach (var door in thisroom.Doors)
            {
                if (door.Rooms.Any(room => room.RoomName != MapGeneration.RoomName.Lcz173)) continue; // this is a door to the outside, we don't want to go out there!
                if (door.IsGate)
                {
                    // we don't want to spawn by the gate, but let's open it
                    door.IsOpen = true;
                    gate = door;
                    continue;
                }
                otherdoor = door;
            }

            var dx = otherdoor.Position.x - gate.Position.x;
            var dz = otherdoor.Position.z - gate.Position.z;
            if (Math.Abs(dx) < Math.Abs(dz))
            {
                behindHere = new Vector3(0, 0, -Math.Sign(dz));
            }
            else
            {
                behindHere = new Vector3(-Math.Sign(dx), 0, 0);
            }

            var cubePosition = gate.Position + behindHere * 6;

            return cubePosition;
        }

        public static (Primitive CUBE, Primitive littleCube) MakeCube()
        {
            var spawn = GetCubeSpawn();

            var littleSpawn = spawn + behindHere;

            var THE_CUBE = Primitive.Create(new(PrimitiveType.Cube, Color.red, spawn, Vector3.zero, Vector3.one * 2f, true));
            THE_CUBE.MovementSmoothing = 60;

            var littleCube = Primitive.Create(new(PrimitiveType.Cube, Color.red, littleSpawn, Vector3.zero, Vector3.one, true));
            littleCube.MovementSmoothing = 60;

            return (THE_CUBE, littleCube);
        }
    }
}
